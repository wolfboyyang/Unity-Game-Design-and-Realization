using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureGenerator : MonoBehaviour {

    public GameObject treasure;

    private const int DefaultGenerateTime = 0;
    public float[] generateTime;
    private GameObject[] treasureInstances;

    private float timer;
    private int generateCount;
    private bool allGenerated;

	// Use this for initialization
	void Start () {
        timer = 0;
        if (generateTime == null)
        {
            generateTime = new float[DefaultGenerateTime];
            for (int i = 0; i < DefaultGenerateTime; i++)
                generateTime[i] = 20 + 30 * i;
        }
        treasureInstances = new GameObject[generateTime.Length];

    }
	
	// Update is called once per frame
	void Update () {
        if (allGenerated) return;

        timer += Time.deltaTime;

        if (timer > generateTime[generateCount])
        {
            treasureInstances[generateCount] = Instantiate(treasure, transform, false);

            generateCount++;
            if (generateCount == generateTime.Length)
                allGenerated = true;
        }
	}

    public void OnRestart()
    {
        for(int i=0;i< treasureInstances.Length; i++)
        {
            if (treasureInstances[i] != null)
            {
                Destroy(treasureInstances[i]);
                treasureInstances[i] = null;
            }
        }

        timer = 0.0f;
        generateCount = 0;
        allGenerated = false;
    }
}
