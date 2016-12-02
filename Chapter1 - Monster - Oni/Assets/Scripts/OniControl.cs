using UnityEngine;
using System.Collections;

public class OniControl : MonoBehaviour {

    public PlayerControl player;

    public const float collisionSize = 0.5f;

    private bool isAlive = true;

    private Vector3 initialPosition;

    public float waveAngleOffset = 0.0f;
    public float waveAmplitude = 0.0f;

    enum State
    {
        None = -1,
        Run = 0,
        Defeated
    }

    private State state = State.None;
    private State nextState = State.None;
    private float timer = 0.0f;

    private Vector3 blowoutVector;
    private Vector3 blowoutAngularVelocity;

	// Use this for initialization
	void Start () {
        initialPosition = transform.position;

        GetComponent<Collider>().enabled = false;

        GetComponent<Rigidbody>().maxAngularVelocity = float.PositiveInfinity;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        var position = transform.position;
        float lowLimit = initialPosition.y;
        switch (state)
        {
            case State.None:
                {
                    nextState = State.Run;
                    break;
                }
            case State.Run:
                {
                    if (position.y < lowLimit)
                        position.y = lowLimit;

                    float waveAngle = 2.0f * Mathf.PI * Mathf.Repeat(timer, 1.0f) + waveAngleOffset;
                    float waveOffset = waveAmplitude * Mathf.Sin(waveAngle);
                    position.z = initialPosition.z + waveOffset;

                    if (waveAmplitude > 0.0f)
                        transform.rotation = Quaternion.AngleAxis(180.0f-30.0f * Mathf.Sin(waveAngle + 90.0f), Vector3.up);
                    break;
                }
            case State.Defeated:
                {
                    if (position.y < lowLimit)
                        if (GetComponent<Rigidbody>().velocity.y > 0.0f)
                            position.y = lowLimit;

                    if (transform.parent != null)
                        GetComponent<Rigidbody>().velocity += -3.0f * Vector3.right * Time.deltaTime;
                   
                    break;
                }
        }

        transform.position = position;

        // Set State to Next
        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Defeated:
                    {
                        GetComponent<Rigidbody>().velocity = blowoutVector;
                        GetComponent<Rigidbody>().rotation = Quaternion.Euler(blowoutAngularVelocity);

                        transform.parent = null;

                        transform.GetChild(0).GetComponent<Animation>().Play("oni_yarare");
                        isAlive = false;

                        foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                        break;
                    }
            }
            state = nextState;
            nextState = State.None;
            timer = 0.0f;
        }

        if (!GetComponent<MeshRenderer>().isVisible && !isAlive)
        {
            if(!GetComponent<AudioSource>().isPlaying || GetComponent<AudioSource>().time >= GetComponent<AudioSource>().clip.length)
                Destroy(gameObject);
        }
    }
    
    public void SetMotionSpeed(float speed)
    {
        transform.GetChild(0).GetComponent<Animation>()["oni_run1"].speed = speed;
        transform.GetChild(0).GetComponent<Animation>()["oni_run2"].speed = speed;
    }

    public void AttackedFromPlayer(Vector3 blowout, Vector3 angularVelocity)
    {
        blowoutVector = blowout;
        blowoutAngularVelocity = angularVelocity;

        transform.parent = null;

        nextState = State.Defeated;
    }
}
