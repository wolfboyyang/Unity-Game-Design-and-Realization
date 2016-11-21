using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    private GameObject player;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.transform.position.x + offset.x, transform.position.y, transform.position.z);
	}
}
