using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

    public float runSpeed = 5.0f;

    public const float RunSpeedMax = 20.0f;

    private const float RunSpeedAdd = 5.0f;

    private const float RunSpeedSub = 5.0f * 4.0f;

    private float attackDisableTime = 0.5f;
    private float attackTime = 0.0f;

    enum State
    {
        None = -1,
        Run,
        Attack,
        Beaten
    }

    enum AttackMode
    {
        Left = 0,
        Right
    }

    private List<string> animationTriggers = new List<string>(){"AttackLeft","AttackRight"};

    private State state = State.None;
    private State nextState = State.None;
    private AttackMode attackMode = AttackMode.Left;

	// Use this for initialization
	void Start () {
        Vector3 v = GetComponent<Rigidbody>().velocity;
        v.x = runSpeed;
        GetComponent<Rigidbody>().velocity = v;

        state = State.Run;

    }
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case State.None:
                {
                    break;
                }
            case State.Run:
                {
                    runSpeed += RunSpeedAdd * Time.deltaTime;
                    runSpeed = Mathf.Clamp(runSpeed, 0, RunSpeedMax);

                    Vector3 v = GetComponent<Rigidbody>().velocity;
                    v.x = runSpeed;
                    GetComponent<Rigidbody>().velocity = v;
                    break;
                }
            case State.Attack:
                {
                    attackTime += Time.deltaTime;

                    if (attackTime > 0.5f)
                    {
                        state = State.Run;
                    }
                    break;
                }
            case State.Beaten:
                {

                    break;
                }
        }

        switch (nextState)
        {
            case State.Run:
                {
                    break;
                }
            case State.Attack:
                {
                    break;
                }
            case State.Beaten:
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(1,5,0);
                    state = State.Run;
                    nextState = State.None;
                    break;
                }
        }
        

#if UNITY_EDITOR
        // Move a large distance to check the floor movement
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(100.0f * FloorControl.Width * FloorControl.FloorCount, 0, 0);
        }
#endif


        // move back player to avoid overflow
        if (transform.position.x > FloorControl.TotalWith* 100.0f)
        {
            transform.position = new Vector3(transform.position.x - FloorControl.TotalWith * 100.0f, transform.position.y, transform.position.z);
        }

        if (attackDisableTime < 0)
        {
            // Check Attack
            if (Input.GetMouseButton(0))
            {
                attackTime = 0;
                GetComponentInChildren<Animator>().SetTrigger(animationTriggers[(int)attackMode]);
                attackMode = (AttackMode)(((int)attackMode+1)%2);
                state = State.Attack;
                attackDisableTime = 1.0f;
            }

        } else
        {
            attackDisableTime -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Oni")
        {
            if (state == State.Attack)
            {
                collision.gameObject.GetComponent<OniControl>().AttackedByPlayer(new Vector3(40, 20, 10), new Vector3(0, 90, 45));
            }
            else
                nextState = State.Beaten;
        }
    }
}
