using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniStillBodyControl : MonoBehaviour {

    public OniEmitterControl emitterControl;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "OniHill")
            emitterControl.PlayHitSound();

        if (collision.gameObject.tag == "Floor")
            Destroy(GetComponent<Rigidbody>());
    }
}
