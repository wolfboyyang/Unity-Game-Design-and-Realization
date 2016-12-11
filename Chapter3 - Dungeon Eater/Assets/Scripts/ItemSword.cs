using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSword : MonoBehaviour {

    public float rotationSpeed = 360;
    public GameObject getEffect;
    public AudioClip getSound;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.SendMessage("OnGetSword");
            transform.parent.GetComponent<AudioSource>().PlayOneShot(getSound);

            var effect = Instantiate(getEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1.0f);
            Destroy(gameObject);
        }
    }
}
