using UnityEngine;
using System.Collections;

public class OniControl : MonoBehaviour {

    enum State
    {
        None = -1,
        Run = 0,
        Defeated
    }

    private State state = State.None;
    private State nextState = State.None;

    private Vector3 blowoutVector;
    private Vector3 blowoutAngularVelocity;

	// Use this for initialization
	void Start () {
        

	}
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case State.None:
                {
                    nextState = State.Run;
                    break;
                }
            case State.Run:
                {
                    break;
                }
            case State.Defeated:
                {
                    if (!GetComponent<MeshRenderer>().isVisible)
                    {
                        Destroy(gameObject);
                    }
                    break;
                }
        }

        // Set State to Next
        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Defeated:
                    {
                        GetComponent<Rigidbody>().velocity = blowoutVector;
                        GetComponent<Rigidbody>().rotation = Quaternion.Euler(blowoutAngularVelocity);
                        break;
                    }
            }

            state = nextState;
            nextState = State.None;
        }
	}

    public void AttackedByPlayer(Vector3 blowout, Vector3 angularVelocity)
    {
        blowoutVector = blowout;
        blowoutAngularVelocity = angularVelocity;

        nextState = State.Defeated;
    }
}
