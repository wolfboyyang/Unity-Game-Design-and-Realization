using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	protected float jumpSpeed = 12.0f;
    public bool     isLanding = false;
    public float    jumpHeight = 4.0f;

    // Use this for initialization
    void Start () {
        isLanding = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isLanding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Left Button Pressed
                // Let player jump up.
                isLanding = false;
                jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpHeight);
                GetComponent<Rigidbody>().velocity = Vector3.up * jumpSpeed;
            }
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        // Player touched the floor
        if (collision.gameObject.tag == "Floor")
        {
            isLanding = true;
        }
    }
}
