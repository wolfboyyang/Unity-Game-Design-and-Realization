using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    private Transform player;
    private GameObject gameController;

    private Vector3 spwanPosition;

    private Animator animator;
    private GridMove gridMove;
    
    private const int MonsterPoint = 300;

    public AudioClip attackSound;

    public enum State
    {
        Normal,
        EncounterPlayer,
        Dead
    }
    private State state;

    public enum AIType
    {
        Tracer,
        Ambush,
        Pincer,
        Random
    }

    public AIType aiType = AIType.Tracer;

    // difficulty
    public const float BaseSpeed = 2.1f;
    public const float SpeedUpPerLevel = 0.3f;
    private const int MaxLevel = 5;

	// Use this for initialization
	void Start () {
        gridMove = GetComponent<GridMove>();
        animator = GetComponent<Animator>();

        gameController = GameObject.FindGameObjectWithTag("GameController");

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (state != State.Normal) return;

        // Animator control
        var targetRotation = Quaternion.LookRotation(gridMove.Direction);
        float t = 1.0f - Mathf.Pow(0.75f, Time.deltaTime * 30.0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spwanPosition = position;
    }

    public void OnStageStart()
    {
        state = State.Normal;
        transform.position = spwanPosition;
    }

    public void OnRestart()
    {
        state = State.Normal;
        transform.position = spwanPosition;

        // Animator Control
        animator.Play("Idle");
    }

    public void OnGrid(Vector3 position)
    {
        if (state != State.Normal) return;

        switch (aiType)
        {
            case AIType.Tracer:
                Tracer(position);
                break;
            case AIType.Ambush:
                Ambush(position);
                break;
            case AIType.Pincer:
                Pincer(position);
                break;
            case AIType.Random:
                RandomAI(position);
                break;
        }
    }

    // Check the given directions and reverse directions
    // to get a proper move direction
    // return zero if moster cannot move to none of them.
    private Vector3 ChooseDirection(Vector3 first, Vector3 second)
    {
        // check order
        // 1. first
        // 2. second
        // 3. -second
        // 4. -first

        if (!gridMove.IsReverseDirection(first) && !gridMove.CheckWall(first))
            return first;

        if (!gridMove.IsReverseDirection(second) && !gridMove.CheckWall(second))
            return second;

        first *= -1.0f;
        second *= -1.0f;

        if (!gridMove.IsReverseDirection(second) && !gridMove.CheckWall(second))
            return second;

        if (!gridMove.IsReverseDirection(first) && !gridMove.CheckWall(first))
            return first;

        return Vector3.zero;
    }

    private void Tracer(Vector3 position)
    {
        var diff = player.position - position;
        
        Vector3 first, second;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            first = Vector3.right * Mathf.Sign(diff.x);
            second = Vector3.forward * Mathf.Sign(diff.z);
        }
        else
        {
            first = Vector3.forward * Mathf.Sign(diff.z);
            second = Vector3.right * Mathf.Sign(diff.x);
        }

        var direction = ChooseDirection(first, second);
        if (direction == Vector3.zero)
            gridMove.Direction = -gridMove.Direction;
        else
            gridMove.Direction = direction;
    }

    private void Ambush(Vector3 position)
    {
        throw new NotImplementedException();
    }

    private void Pincer(Vector3 position)
    {
        throw new NotImplementedException();
    }

    private void RandomAI(Vector3 position)
    {
        throw new NotImplementedException();
    }

    public void AttackPlayer()
    {
        StartCoroutine("Attack");
    }

    private IEnumerator Attack()
    {
        gameController.SendMessage("OnAttackBegin");
        transform.LookAt(player);
        yield return null;

        animator.SetTrigger("BeginAttack");
        GetComponent<AudioSource>().PlayOneShot(attackSound);
        yield return new WaitForSeconds(0.5f);

        player.SendMessage("OnDamage");

        state = State.Normal;
        animator.SetTrigger("BeginIdle");

        gameController.SendMessage("OnAttackEnd");
    }

    public void OnDamage()
    {
        state = State.Dead;
        SendMessage("OnDead");

        gameController.SendMessage("AddScore", MonsterPoint);

        StartCoroutine("Dead");
    }

    IEnumerator Dead()
    {
        animator.SetTrigger("BeginDead");
        yield return new WaitForSeconds(3.0f);

        // reborn
        SendMessage("OnReborn");
        
    }

    public void OnReborn()
    {
        state = State.Normal;
        animator.SetTrigger("BeginIdle");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Monster in Normal State can encounter player
        if (state != State.Normal) return;
        
        if(other.tag == "Player")
        {
            state = State.EncounterPlayer;
            player.SendMessage("EncountMonster", transform);
        }
    }

    
}
