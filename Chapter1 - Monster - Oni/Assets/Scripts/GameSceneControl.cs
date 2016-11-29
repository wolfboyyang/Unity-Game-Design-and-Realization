using UnityEngine;
using System.Collections;

public class GameSceneControl : MonoBehaviour {

    public const float OniAppearDistance = 50;
    private PlayerControl player;

    public GameObject oniPrefab;

    private bool canDispatch = false;
    private int OniCount = 0;
    private float oniLastAppearTime = 0;
    public const int OniMaxCount = 3;
    public const float OniAppearTime = 2.0f;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () {

        OniAppearControl();
        oniLastAppearTime += Time.deltaTime;

    }

    private void OniAppearControl()
    {
        if (player.runSpeed == PlayerControl.RunSpeedMax && oniLastAppearTime > OniAppearTime)
        {
            Vector3 appearPosition = player.transform.position;
            appearPosition.x += OniAppearDistance;

            GameObject go = Instantiate(oniPrefab, appearPosition, Quaternion.identity) as GameObject;

            oniLastAppearTime = 0;
        }
    }
}
