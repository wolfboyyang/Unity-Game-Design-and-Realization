using UnityEngine;
using System.Collections;

public class FloorControl : MonoBehaviour {

    private Vector3 initPosition;

    public const float Width = 40.0f;
    public const int FloorCount = 3;
    public const float TotalWith = Width * FloorCount;

    // Use this for initialization
    void Start () {
        initPosition = transform.position;

	}
	
	// Update is called once per frame
	void Update () {

#if TestSimpleImplement
        // Simple Implement
        // Cannot follow up if player moved a large distance
        float distance = Camera.main.transform.position.x - transform.position.x;
        if (distance > TotalWith/2.0f)
        {
            // Move to the Front
            transform.Translate(TotalWith, 0, 0);
        }
#else
        float distance = Camera.main.transform.position.x - initPosition.x;

        transform.position = new Vector3(Mathf.Round(distance / TotalWith) * TotalWith + initPosition.x, transform.position.y, transform.position.z);

#endif



    }
}
