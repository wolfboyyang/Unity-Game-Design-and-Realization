using UnityEngine;
using System.Collections;

public class OniEmitterControl : MonoBehaviour {

    public GameObject[] oniPrefabs;

    public AudioClip emitSound;
    public AudioClip hitSound;

    private GameObject lastCreatedOni;
    private const float collisionRadius = 0.25f;

    public int oniNum = 2;
    public bool isEnableHitSound = true;

	// Use this for initialization
	void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(emitSound);
	}
	
	// Update is called once per frame
	void Update()
    {
        if (oniNum <= 0) return;

        if (lastCreatedOni != null)
        {
            if (Vector3.Distance(transform.position, lastCreatedOni.transform.position) <= collisionRadius * 2.0f)
                return;
        }

        var position = transform.position;
        position.y += Random.Range(-0.5f, 0.5f);
        position.z += Random.Range(-0.5f, 0.5f);

        var rotation = Quaternion.identity;
        rotation *= Quaternion.AngleAxis(oniNum * 50.0f, Vector3.forward);
        rotation *= Quaternion.AngleAxis(oniNum * 30.0f, Vector3.right);

        var oni = Instantiate(oniPrefabs[oniNum % 2], position, rotation);
        oni.GetComponent<Rigidbody>().velocity = Vector3.down;
        oni.GetComponent<Rigidbody>().angularVelocity = rotation * Vector3.forward * 5.0f * (oniNum % 3);

        lastCreatedOni = oni;

        oniNum--;
    }

    public void PlayHitSound()
    {
        if (isEnableHitSound)
        {
            if (!GetComponent<AudioSource>().isPlaying
                || GetComponent<AudioSource>().time >= GetComponent<AudioSource>().clip.length * 0.75f)
            {
                GetComponent<AudioSource>().clip = hitSound;
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
