using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Shot the ball at start
        GetComponent<Rigidbody>().velocity = new Vector3(-10.0f, 9.0f, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnBecameInvisible()
    {
        // Destroy the ball if it fell out of camera.
        Destroy(gameObject);
    }
}
