using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour {

    public GameObject pickupEffect;
    public AudioClip pickupSound;
    public AudioClip apearSound;

    public const int PickupPoint = 1000;

    private float lifeTime = 10.0f;
    private GameObject gameController;

	// Use this for initialization
	void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        transform.parent.GetComponent<AudioSource>().PlayOneShot(apearSound);

        Destroy(gameObject, lifeTime);
	}

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            gameController.SendMessage("AddScore", PickupPoint);

            var effect = Instantiate(pickupEffect, transform.position + Vector3.up, Quaternion.identity);

            transform.parent.GetComponent<AudioSource>().PlayOneShot(pickupSound);
            Destroy(effect, 3.0f);
            Destroy(gameObject);
        }
    }
}
