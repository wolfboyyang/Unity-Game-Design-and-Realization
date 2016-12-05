using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleControl : MonoBehaviour
{

    public GameSceneControl gameSceneControl;

    private int pieceNum;
    private int pieceFinishedNum;

    private List<PieceControl> allPieces;
    private List<PieceControl> activePieces;

    private Bounds shuffleZone;
    private float puzzleRotation = 37.0f;
    private int shuffleGridNum = 1;
    private bool isDisplayCleared = false;

    private const float ShuffleZoneOffsetX = -5.0f;
    private const float ShuffleZoneOffsetY = 1.0f;
    private const float ShuffleZoneScale = 1.1f;

    enum State
    {
        None,
        Play,
        Clear
    }

    private State state = State.None;
    private State nextState = State.None;
    private float timer = 0.0f;
    private float preTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        //gameSceneControl = GameObject.FindGameObjectWithTag("GameController")
        //    .GetComponent<GameSceneControl>();

        allPieces = new List<PieceControl>();
        activePieces = new List<PieceControl>();

        pieceNum = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            var piece = transform.GetChild(i).gameObject;
            if (IsPieceObject(piece))
            {
                var pieceControl = piece.AddComponent<PieceControl>();
                pieceControl.gameSceneControl = gameSceneControl;
                pieceControl.puzzleControl = this;
                allPieces.Add(pieceControl);

                piece.GetComponent<Renderer>().material.renderQueue = GetDrawPriorityBase();
                pieceNum++;
            }
        }

        SetHeightOffsetToPieces();

        CalcShuffleZone();

        pieceFinishedNum = 0;
        isDisplayCleared = false;
    }

    // Update is called once per frame
    void Update()
    {
        preTimer = timer;
        timer += Time.deltaTime;

        switch (state)
        {
            case State.None:
                nextState = State.Play;
                break;
            case State.Play:
                if (pieceFinishedNum == pieceNum)
                    nextState = State.Clear;
                    
                break;
            case State.Clear:
                {
                    const float playSEDelay = 0.4f;
                    if(preTimer<playSEDelay&& playSEDelay <= timer)
                    {
                        gameSceneControl.PlaySound(GameSceneControl.SoundEffect.Complete);
                        isDisplayCleared = true;
                    }
                }
                break;
        }

        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Play:
                    {
                        activePieces.Clear();
                        activePieces.AddRange(allPieces);
                        pieceFinishedNum = 0;
                        ShufflePieces();
                        foreach (var piece in activePieces)
                            piece.Restart();

                        SetHeightOffsetToPieces();
                        break;
                    }

            }

            state = nextState;
            nextState = State.None;
            timer = 0.0f;
        }
    }

    private bool IsPieceObject(GameObject gameObject)
    {
        if (gameObject.name.Contains("base")) return false;
        else return true;
    }

    private int GetDrawPriorityBase()
    {
        return 0;
    }

    private int GetDrawPriorityFinishedPiece()
    {
        return GetDrawPriorityBase() + 1;
    }

    public int GetDrawPriorityRetryButton()
    {
        return GetDrawPriorityFinishedPiece() + 1;
    }

    private int GetDrawPriorityPiece(int priorityInPiece)
    {
        int priority = GetDrawPriorityRetryButton() + 1;
        priority += pieceNum - 1 - priorityInPiece;
        return priority;
    }

    private void CalcShuffleZone()
    {
        Vector3 center = Vector3.zero;
        foreach (var piece in allPieces)
            center += piece.finishedPositon;

        center /= pieceNum;
        center.x += ShuffleZoneOffsetX;
        center.y += ShuffleZoneOffsetY;

        // square grid to put piece in
        shuffleGridNum = Mathf.CeilToInt(Mathf.Sqrt(pieceNum));

        var pieceMaxBounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (var piece in allPieces)
            pieceMaxBounds.Encapsulate(piece.GetBounds(Vector3.zero));
        // I Rotate the model -90.0f by Axis x
        pieceMaxBounds.size = transform.localRotation * pieceMaxBounds.size;

        pieceMaxBounds.size *= ShuffleZoneScale;
        shuffleZone = new Bounds(center, pieceMaxBounds.size * shuffleGridNum);
    }

    private void ShufflePieces()
    {
        int[] pieceIndex = new int[shuffleGridNum * shuffleGridNum];
        for (int i = 0; i < pieceNum; i++)
            pieceIndex[i] = i;
        for (int i = pieceNum; i < pieceIndex.Length; i++)
            pieceIndex[i] = -1;
        // modified Knuth-Durstenfeld Shuffle
        for (int i=0;i<pieceIndex.Length-1;i++)   
        {
            int j = Random.Range(i+1, pieceIndex.Length);
            int temp = pieceIndex[j];
            pieceIndex[j] = pieceIndex[i];
            pieceIndex[i] = temp;
        }

        var gridSize = shuffleZone.size / (float)shuffleGridNum;
        var offsetCycle = gridSize / 2.0f;
        var offsetAdd = gridSize / 5.0f;
        var offset = Vector3.zero;
        for (int i = 0; i < pieceIndex.Length; i++)
        {
            int index = pieceIndex[i];
            if (index < 0) continue;

            var piece = activePieces[index];
            var position = piece.transform.position;
            int x = i % shuffleGridNum;
            int y = i / shuffleGridNum;
            position.x = x * gridSize.x;
            position.y = y * gridSize.y;

            // start from top left of shuffle zone
            position.x += shuffleZone.center.x - gridSize.x * (shuffleGridNum / 2.0f - 0.5f);
            position.y += shuffleZone.center.y - gridSize.y * (shuffleGridNum / 2.0f - 0.5f);
            // height/depth
            position.z = piece.finishedPositon.z;

            // add cycle offset
            position.x += offset.x;
            position.y += offset.y;
            // Rotate the shuffle zone
            position -= shuffleZone.center;
            position = Quaternion.AngleAxis(puzzleRotation, Vector3.forward) * position;
            position += shuffleZone.center;

            piece.startPositon = position;

            offset.x += offsetAdd.x;
            if (offset.x > offsetCycle.x / 2.0f)
                offset.x -= offsetCycle.x;
            offset.x += offsetAdd.x;
            if (offset.y > offsetCycle.y / 2.0f)
                offset.y -= offsetCycle.y;
        }
        // for next try
        puzzleRotation += 90;
    }

    private void SetHeightOffsetToPieces()
    {
        int i = 0;
        float offset = 0.01f;
        foreach(var piece in activePieces)
        {
            piece.GetComponent<Renderer>().material.renderQueue = GetDrawPriorityPiece(i);

            offset -= 0.01f / pieceNum;
            piece.SetHeightOffset(offset);
            i++;
        }
    }

    public bool IsCleared()
    {
        return state == State.Clear;
    }

    public bool IsDisplayCleared()
    {
        return isDisplayCleared;
    }

    public void Restart()
    {
        nextState = State.Play;
    }
    public void PickPiece(PieceControl piece)
    {
        activePieces.Remove(piece);
        activePieces.Insert(0, piece);
        SetHeightOffsetToPieces();
    }

    public void FinishPiece(PieceControl piece)
    {
        piece.GetComponent<Renderer>().material.renderQueue = GetDrawPriorityFinishedPiece();
        activePieces.Remove(piece);
        pieceFinishedNum++;
        SetHeightOffsetToPieces();
    }

}
