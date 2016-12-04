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
        nextState = State.Idle;
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.Idle:
                if (isDragging)
                    nextState = State.Dragging;
                break;
            case State.Dragging:
                {
                    DoDragging();
                    if (IsInAttachRange())
                    {

                    }
                    else
                    {
                        if (!isDragging)
                        {
                            nextState = State.Idle;
                            gameSceneControl.PlaySound(GameSceneControl.SoundEffect.Release);
                        }
                    }
                    break;
                }
            case State.Attaching:
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

                    gameSceneControl.PlaySound(GameSceneControl.SoundEffect.Grab);
                    break;
                case State.Finished:
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

    // as I use z axis as the height, smaller z is higher
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
        return false;
        if (Vector3.Distance(transform.position, finishedPositon) < attachDistance)
            return true;
        else
            return false;
    }
}
