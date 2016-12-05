using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// need collider to reveice mouse event
[RequireComponent(typeof(MeshCollider))]
public class PieceControl : MonoBehaviour {

    public GameSceneControl gameSceneControl;
    public PuzzleControl puzzleControl;

    public bool enableGrabOffset = true;

    // I use z axis as height
    private const float HeightOffsetBase = 0.1f;
    private const float AttachSpeedMin = 0.01f * 60.0f;
    private const float AttachSpeedMax = 0.8f * 60.0f;

    private Vector3 grabOffset = Vector3.zero;
    private bool isDragging = false;
    public Vector3 finishedPositon;
    public Vector3 startPositon;
    public float heightOffset = 0.0f;
    public float roll = 0.0f;

    public float attachDistance = 0.5f;

    enum State
    {
        None,
        Idle,
        Dragging,
        Finished,
        Restart,
        Attaching
    }

    private State state = State.None;
    private State nextState = State.None;

    private Vector3 attachTarget;

    void Awake()
    {
        finishedPositon = transform.position;

        startPositon = finishedPositon;
    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.None:
                nextState = State.Restart;
                break;
            case State.Idle:
                if (isDragging)
                    nextState = State.Dragging;
                break;
            case State.Dragging:
                {
                    DoDragging();
                    var color = Color.white;
                    if (IsInAttachRange())
                    {
                        color = Color.green;
                        if (!isDragging)
                        {
                            nextState = State.Attaching;
                            attachTarget = finishedPositon;
                            gameSceneControl.PlaySound(GameSceneControl.SoundEffect.Attach);
                        }
                    }
                    else
                    {
                        if (!isDragging)
                        {
                            nextState = State.Idle;
                            gameSceneControl.PlaySound(GameSceneControl.SoundEffect.Release);
                        }
                    }
                    GetComponent<Renderer>().material.color = color;
                    break;
                }
            case State.Attaching:
                var distance = attachTarget - transform.position;
                distance *= 0.5f;
                float minMoveDistance = AttachSpeedMin * Time.deltaTime;
                float maxMoveDistacne = AttachSpeedMax * Time.deltaTime;
                if (distance.magnitude < minMoveDistance)
                    transform.position = attachTarget;
                else if (distance.magnitude > maxMoveDistacne)
                    distance *= maxMoveDistacne / distance.magnitude;

                transform.position = transform.position + distance;

                if (Vector3.Distance(transform.position, attachTarget) < 0.0001f)
                    nextState = State.Finished;
                break;
        }

        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Idle:
                    SetHeightOffset(heightOffset);
                    break;
                case State.Restart:
                    transform.position = startPositon;
                    nextState = State.Idle;
                    break;
                case State.Dragging:
                    BeginDragging();
                    puzzleControl.PickPiece(this);
                    gameSceneControl.PlaySound(GameSceneControl.SoundEffect.Grab);
                    break;
                case State.Finished:
                    transform.position = finishedPositon;
                    puzzleControl.FinishPiece(this);
                    GetComponent<Renderer>().material.color = Color.white;
                    break;
            }
            state = nextState;
            nextState = State.None;
        }
	}

    private void BeginDragging()
    {
        if (enableGrabOffset)
        {
            Vector3 worldPosition;
            if (UnprojectMousePosition(Input.mousePosition, out worldPosition))
            {
                grabOffset = transform.position - worldPosition;
            }
        }
    }

    private void DoDragging()
    {
        Vector3 worldPosition;
        if(UnprojectMousePosition(Input.mousePosition, out worldPosition))
        {
            transform.position = worldPosition + grabOffset;
        }
    }

    public Bounds GetBounds(Vector3 center)
    {
        var bounds = GetComponent<MeshFilter>().mesh.bounds;
        bounds.center = center;
        return bounds;
    }

    public void Restart()
    {
        nextState = State.Restart;
    }

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    // in 2D mode, the front coodination is x,y.
    // z is the depth, the bigger z the deeper into the screen 
    public void SetHeightOffset(float offset)
    {
        var position = transform.position;
        heightOffset = 0.0f;
        if(state!=State.Finished && nextState != State.Finished)
        {
            heightOffset = offset;
            position.z = finishedPositon.z - HeightOffsetBase;
            position.z -= heightOffset;

            transform.position = position;
        }
    }
    private bool UnprojectMousePosition(Vector3 mousePosition,out Vector3 worldPosition)
    {
        bool result = false;
        var plane = new Plane(Vector3.forward, new Vector3(0, 0, transform.position.z));
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        float distance = 0.0f;
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
            result = true;
        }
        else
        {
            worldPosition = Vector3.zero;
            result = false;
        }

        return result;
    }

    private bool IsInAttachRange()
    {
        if (Vector3.Distance(transform.position, finishedPositon) < attachDistance)
            return true;
        else
            return false;
    }
}
