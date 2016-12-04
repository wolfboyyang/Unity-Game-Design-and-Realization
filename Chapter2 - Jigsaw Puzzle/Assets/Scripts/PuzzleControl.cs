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
    private bool isCleared = false;

    private const float ShuffleZoneOffsetX = -5.0f;
    private const float ShuffleZoneOffsetY = 1.0f;
    private const float ShuffleZoneScale = 1.1f;

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
    }

    // Update is called once per frame
    void Update()
    {

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
    }

    private void ShufflePieces()
    {

    }

    private void SetHeightOffsetToPieces()
    {

    }

    public bool IsCleared()
    {
        return true;
    }
}
