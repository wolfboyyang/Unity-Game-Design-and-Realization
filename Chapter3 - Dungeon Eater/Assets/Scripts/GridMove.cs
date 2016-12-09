using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour
{

    private enum MoveState
    {
        None,
        Pause,
        HitStop
    }

    private MoveState state = MoveState.None;

    public float speed = 1.0f;

    private Vector3 direction;
    private float moveDistance;
    private Vector3 currentGrid;

    private const float HitCheckHeight = 0.5f;
    private const int HitCheckLayerMask = 1;

    // Use this for initialization
    void Start()
    {
        moveDistance = 0.0f;
        direction = Vector3.forward;
        state = MoveState.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (state != MoveState.None)
        {
            moveDistance = 0.0f;
        }
        else
        {
            if (Time.deltaTime <= 0.1f)
                Move(Time.deltaTime);
            else
            {
                int n = (int)(Time.deltaTime / 0.1f) + 1;
                float time = Time.deltaTime / n;
                for (int i = 0; i < n; i++)
                    Move(time);
            }
        }
    }

    public void OnStageStart()
    {
        moveDistance = 0.0f;
        state = MoveState.None;
    }

    public void Move(float time)
    {
        var position = transform.position + direction * speed * time;

        bool across = false;
        currentGrid = new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));

        if (currentGrid.x != (int)transform.position.x)
            across = true;
        else if (currentGrid.z != (int)transform.position.z)
            across = true;

        var forwardPosition = position + direction * 0.5f;
        if (Mathf.Round(forwardPosition.x) != currentGrid.x ||
                Mathf.Round(forwardPosition.z) != currentGrid.z)
        {
            if (CheckWall(position, direction))
            {
                position = currentGrid;
                across = true;
            }
        }

        if (across || (position - currentGrid).magnitude < 0.01f)
        {
            SendMessage("OnGrid", position);
        }

        moveDistance = (position - transform.position).magnitude / time;
        transform.position = position;
    }

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public bool IsReverseDirection(Vector3 direction)
    {
        // grid move only have 4 direction
        // so the dot result only in 
        //  1: same, 
        //  0: left or right
        // -1: reverse
        return Vector3.Dot(direction, this.direction) < 0;
    }

    public bool CheckWall(Vector3 direction)
    {
        var position = currentGrid;
        position.y += HitCheckHeight;
        return Physics.Raycast(position, direction, 1.0f, HitCheckLayerMask);
    }

    public bool CheckWall(Vector3 position, Vector3 direction)
    {
        position.y += HitCheckHeight;
        return Physics.Raycast(position, direction, 1.0f, HitCheckLayerMask);
    }

    public bool IsWalking { get { return moveDistance > 0.01f; } }

    public void StopHit(bool enable)
    {
        if (enable)
            state |= MoveState.HitStop;
        else
            state &= ~MoveState.HitStop;
    }

}
