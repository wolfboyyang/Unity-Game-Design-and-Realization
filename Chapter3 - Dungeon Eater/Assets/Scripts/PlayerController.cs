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

    private Animator animator;
    private GridMove gridMove;
    private Weapon weapon;

    public enum State
    {
        Normal,
        EncountMosnter,
        Dead
    }
    private State state = State.Normal;

    // Use this for initialization
    void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        animator = GetComponent<Animator>();
        gridMove = GetComponent<GridMove>();
        weapon = GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.Normal) return;
        // Animator control
        var targetRotation = Quaternion.LookRotation(gridMove.Direction);
        float t = 1.0f - Mathf.Pow(0.75f, Time.deltaTime * 30.0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
        animator.SetBool("isWalking", gridMove.IsWalking);
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

        animator.Play("Idle");
        state = State.Normal;
    }

    public void OnRestart()
    {
        SetPosition(map.GetSpawnPoint(Map.SpawnPointType.Player));
        lastInput = Vector3.zero;
        lastInputTime = 0.0f;

        animator.Play("Idle");
        state = State.Normal;
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

    public void EncountMonster(Transform other)
    {
        if (state != State.Normal) return;
        // Only normal state can encount monsters
        state = State.EncountMosnter;
        animator.SetBool("isWalking", false);
        if (weapon.CanAutoAttack())
            weapon.AutoAttack(other);
        else
            other.SendMessage("AttackPlayer");

    }

    public void OnDamage()
    {
        StartCoroutine("Dead");
    }

    IEnumerator Dead()
    {
        state = State.Dead;
        animator.SetTrigger("BeginDead");

        SendMessage("OnDead");
        GameObject.FindGameObjectWithTag("GameController").SendMessage("PlayerIsDead");
        yield return null;
    }

    public void OnMonsterDead()
    {
        // path clear, let's go
        state = State.Normal;
        animator.Play("Idle");
    }

    // Called by knight_wall anmiation
    // Need to check the animation event
    public void PlayStepSound(float float_param)
    {
        GetComponent<AudioSource>().PlayOneShot(stepSound);
    }

}
