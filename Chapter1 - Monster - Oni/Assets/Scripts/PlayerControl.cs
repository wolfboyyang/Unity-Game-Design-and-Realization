using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float runSpeed = 5.0f;

    public const float RunSpeedMax = 20.0f;

    private const float RunSpeedAdd = 5.0f;

    private const float RunSpeedSub = 5.0f * 4.0f;

    private float attackDisableTime = 1.0f;

	// Use this for initialization
	void Start () {
        Vector3 v = GetComponent<Rigidbody>().velocity;
        v.x = runSpeed;
        GetComponent<Rigidbody>().velocity = v;
	}
	
	// Update is called once per frame
	void Update () {
        runSpeed += RunSpeedAdd * Time.deltaTime;

        runSpeed = Mathf.Clamp(runSpeed, 0, RunSpeedMax);

        Vector3 v = GetComponent<Rigidbody>().velocity;
        v.x = runSpeed;
        GetComponent<Rigidbody>().velocity = v;

#if UNITY_EDITOR
        // Move a large distance to check the floor movement
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(100.0f * FloorControl.Width * FloorControl.FloorCount, 0, 0);
        }
#endif


        // move back player to avoid overflow
        if (transform.position.x > FloorControl.TotalWith* 100.0f)
        {
            transform.position = new Vector3(transform.position.x - FloorControl.TotalWith * 100.0f, transform.position.y, transform.position.z);
        }

        if (attackDisableTime < 0)
        {
            // Check Attack
            if (Input.GetMouseButton(0))
            {
                GetComponentInChildren<Animator>().SetTrigger("AttackLeft");
                attackDisableTime = 1.0f;
            }

            if (Input.GetMouseButton(1))
            {
                GetComponentInChildren<Animator>().SetTrigger("AttackRight");
                attackDisableTime = 1.0f;
            }
        } else
        {
            attackDisableTime -= Time.deltaTime;
        }

    }
}
