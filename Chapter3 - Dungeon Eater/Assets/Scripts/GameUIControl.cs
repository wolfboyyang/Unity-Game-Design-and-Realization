using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIControl : MonoBehaviour {

    public Image readyImage;
    public Image gameOverImage;
    public Image stageClearImage;

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
        stageText.text = "Stage " + (stage+1);
    }

    public void SetScore(int score)
    {
        scoreText.text = "SCORE: "+score;
    }

    public void DrawStageStart(bool visible)
    {
        readyImage.gameObject.SetActive(visible);
    }

    public void DrawGameOver(bool visible)
    {
        gameOverImage.gameObject.SetActive(visible);
    }

    public void DrawStageClear(bool visible)
    {
        stageClearImage.gameObject.SetActive(visible);
    }

}
