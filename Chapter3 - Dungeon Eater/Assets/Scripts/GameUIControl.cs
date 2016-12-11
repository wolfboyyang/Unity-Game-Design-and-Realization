using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIControl : MonoBehaviour {

    public Text scoreText;
    public Text stageText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStage(int stage)
    {
        stageText.text = "Stage " + stage;
    }

    public void SetScore(int score)
    {
        scoreText.text = "SCORE: "+score;
    }
}
