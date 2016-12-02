using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

    private Animator animator;

    // -------------------------------------------------------------------------------- //
    
    // sound effect
    public AudioClip swordSound;
    public AudioClip swordHitSound;
    public AudioClip[] attackSounds;
    public AudioClip missSound;
    public AudioClip runSound;

    public AudioSource attackVoiceAudio;
    public AudioSource swordAudio;
    public AudioSource missAudio;
    public AudioSource runAudio;

    private int attackSoundIndex = 0;
    public float MinRunAudioRate = 0.2f;
    public float MaxRunAudioRate = 0.5f;

    // -------------------------------------------------------------------------------- //

    public float runSpeed = 5.0f;

    public const float RunSpeedMax = 20.0f;

    private const float Acceleration = 5.0f;

    private const float Deceleration = 5.0f * 4.0f;

    private const float MissGravity = 9.8f * 2.0f;

    private AttackColliderControl attackCollider;
    public GameSceneControl gameSceneControl;

    private float attackTimer = 0.0f;
    private float attackDisableTimer = 0.0f;

    public const float AttackTime = 0.3f;
    public const float AttackDisableTime = 1.0f;

    private bool isRunning = true;
    private bool isContactFloor = false;

    private bool isPlayable = true;
    public bool IsPlayable
    {
        get { return isPlayable; }
        set { isPlayable = value; }
    }

    public float stopPosition = -1.0f;

    enum State
    {
        None = -1,
        Run,
        Attack,
        Idle,
        Miss
    }

    public enum AttackMotion
    {
        Left = 0,
        Right
    }
    private List<string> animationTriggers = new List<string>()
        { "AttackLeft",
          "AttackRight" };

    public AttackMotion attackMotion = AttackMotion.Left;

    public ParticleSystem hitFX;
    public ParticleSystem runFX;

    [SerializeField]
    private State state = State.None;
    private State nextState = State.None;
    

	// Use this for initialization
	void Start () {

        animator = GetComponentInChildren<Animator>();

        attackCollider = GameObject.FindGameObjectWithTag("AttackCollider").GetComponent<AttackColliderControl>();
        attackCollider.player = this;

        attackVoiceAudio = gameObject.AddComponent<AudioSource>();
        swordAudio = gameObject.AddComponent<AudioSource>();
        missAudio = gameObject.AddComponent<AudioSource>();
        runAudio = gameObject.AddComponent<AudioSource>();
        runAudio.clip = runSound;
        runAudio.loop = true;
        runAudio.Play();

        runSpeed = 0.0f;
        nextState = State.Run;

    }
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case State.Run:
                {
                    if (isRunning)
                        runSpeed += Acceleration * Time.deltaTime;
                    else
                        runSpeed -= Deceleration * Time.deltaTime;

                    runSpeed = Mathf.Clamp(runSpeed, 0.0f, RunSpeedMax);

                    Vector3 velocity = GetComponent<Rigidbody>().velocity;
                    velocity.x = runSpeed;
                    // keep player stay on the ground
                    if (velocity.y > 0) velocity.y = 0;
                    GetComponent<Rigidbody>().velocity = velocity;

                    float rate = runSpeed / RunSpeedMax;
                    runAudio.pitch = Mathf.Lerp(MinRunAudioRate, MaxRunAudioRate, rate);

                    AttackControl();
                    SwordFXControl();

                    if (!isRunning && runSpeed <= 0.0f)
                    {
                        runFX.Stop();
                        nextState = State.Idle;
                    }
                    break;
                }
            case State.Miss:
                GetComponent<Rigidbody>().velocity += Vector3.down * MissGravity * Time.deltaTime;

                if (isContactFloor)
                {
                    runFX.Play();
                    GetComponent<Rigidbody>().useGravity = true;
                    nextState = State.Run;
                }
                break;
        }

        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Idle:
                    animator.SetTrigger("Idle");
                    break;
                case State.Miss:
                    {
                        Vector3 velocity = Vector3.zero;
                        float jumpHeight = 1.0f;
                        velocity.x = -2.5f;
                        velocity.y = Mathf.Sqrt(MissGravity * jumpHeight * 2);
                        velocity.z = 0.0f;

                        GetComponent<Rigidbody>().velocity = velocity;
                        GetComponent<Rigidbody>().useGravity = false;

                        runSpeed = 0.0f;
                        animator.SetTrigger("Beaten");

                        missAudio.PlayOneShot(missSound);
                        runFX.Stop();
                        break;
                    }
            }

            state = nextState;
            nextState = State.None;
        }

#if UNITY_EDITOR
        // Move a large distance to check the floor movement
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(100.0f * FloorControl.Width * FloorControl.FloorCount, 0, 0);
        }
#endif


        // move back player to avoid overflow
        //if (transform.position.x > FloorControl.TotalWith* 100.0f)
        //{
        //    transform.position = new Vector3(transform.position.x - FloorControl.TotalWith * 100.0f, transform.position.y, transform.position.z);
        //}

        
    }
    
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "OniGroup")
        {
            if(attackTimer<=0.0f && state!= State.Miss)
            {
                nextState = State.Miss;
                gameSceneControl.OnPlayerMissed();

                var oniGroup = other.gameObject.GetComponent<OniGroupControl>();
                oniGroup.OnPlayerHitted();
            }
        }

        if (other.gameObject.tag == "Floor" && other.relativeVelocity.y > Physics.gravity.y * Time.deltaTime)
            isContactFloor = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        OnCollisionStay(collision);
    }

    public void PlayHitEffect(Vector3 position)
    {
        hitFX.transform.position = position;
        hitFX.Play();
    }

    public void PlayHitSound()
    {
        swordAudio.PlayOneShot(swordSound);
    }

    public void ResetAttackDisableTimer()
    {
        attackDisableTimer = 0.0f;
    }

    public float GetAttackTime()
    {
        return AttackTime - attackTimer;
    }

    public float GetSpeedRate()
    {
        return Mathf.InverseLerp(0.0f, RunSpeedMax, runSpeed);
    }

    public void Stop()
    {
        isRunning = false;
    }

    public bool IsStopped { get { return !isRunning && runSpeed == 0.0f; }}

    public float DistanceToStop()
    {
        return runSpeed * runSpeed / (2.0f * Deceleration);
    }

    private bool IsAttacking()
    {
        bool isAttacking = false;

        if (Input.GetMouseButton(0))
            isAttacking = true;

        return isAttacking;
    }

    private void AttackControl()
    {
        if (!isPlayable) return;

        if (attackTimer > 0.0f)
        {
            // Attacking
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                attackCollider.SetPowered(false);
        }
        else if (attackDisableTimer <= 0.0f)
        {
            if (IsAttacking())
            {
                attackCollider.SetPowered(true);
                attackTimer = AttackTime;
                attackDisableTimer = AttackDisableTime;

                animator.SetTrigger(animationTriggers[(int)attackMotion]);
                attackMotion = (AttackMotion)(((int)attackMotion + 1) % 2);

                attackVoiceAudio.PlayOneShot(attackSounds[attackSoundIndex]);
                attackSoundIndex = (attackSoundIndex + 1) % attackSounds.Length;
                swordAudio.PlayOneShot(swordSound);
            }
        }
        else
            attackDisableTimer -= Time.deltaTime;
    }

    private void SwordFXControl()
    {

    }

    
}
