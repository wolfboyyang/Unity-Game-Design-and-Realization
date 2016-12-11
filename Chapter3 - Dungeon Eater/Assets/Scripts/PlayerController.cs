using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Map map;

    public AudioClip stepSound;

    public const float Threshold = 0.1f;
    private Vector3 lastInput = Vector3.zero;
    private float lastInputTime = 0.0f;

    private GridMove gridMove;

    public enum State
    {
        Normal,
        Dead
    }

    // Use this for initialization
    void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();

        gridMove = GetComponent<GridMove>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // keep current y and set x & z
    private void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    public void OnStageStart()
    {
        SetPosition(map.GetSpawnPoint(Map.SpawnPointType.Player));
        lastInput = Vector3.zero;
        lastInputTime = 0.0f;
    }

    private Vector3 GetMoveDirection()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        var absX = Mathf.Abs(x);
        var absZ = Mathf.Abs(z);


        if (absX < Threshold && absZ < Threshold)
        {
            if (lastInputTime < 0.2f)
            {
                lastInputTime += Time.deltaTime;
                x = lastInput.x;
                z = lastInput.z;
                absX = Mathf.Abs(lastInput.x);
                absZ = Mathf.Abs(lastInput.z);
            }
            return Vector3.zero;
        }
        else
        {
            lastInputTime = 0.0f;
            lastInput.x = x;
            lastInput.z = z;
        }

        if (absX > absZ)
            return new Vector3(x / absX, 0, 0);
        else
            return new Vector3(0, 0, z / absZ);
    }

    public void OnGrid(Vector3 position)
    {
        map.PickupItem(position);

        var direction = GetMoveDirection();

        // no input
        if (direction == Vector3.zero) return;

        if (!gridMove.CheckWall(direction))
        {
            gridMove.Direction = direction;
        }
    }

    public void OnDamage()
    {

    }

    // Called by knight_wall anmiation
    // Need to check the animation event
    public void PlayStepSound(float float_param)
    {
        GetComponent<AudioSource>().PlayOneShot(stepSound);
    }

}
