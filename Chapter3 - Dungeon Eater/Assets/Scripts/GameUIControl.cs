using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIControl : MonoBehaviour {

    public Image readyImage;
    public Image gameOverImage;
    public Image stageClearImage;

    public GameObject lifeImagePrefab;
    public int lifeImageOffset = 32;

    public Text scoreText;
    public Text stageText;
    private List<GameObject> lifeImages = new List<GameObject>();
    private int lifeRemain;

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

    public void SetLife(int life)
    {
        if(life> lifeImages.Count)
        {
            for(int i = lifeImages.Count; i<life;i++)
            {
                var lifeImage = Instantiate(lifeImagePrefab, transform, false);
                lifeImage.transform.Translate(lifeImageOffset * i, 0, 0);
                lifeImages.Add(lifeImage);
            }
        }
        int index = 0;
        foreach(var lifeImage in lifeImages)
        {
            if (index < life) lifeImage.gameObject.SetActive(true);
            else lifeImage.SetActive(false);
            index++;
        }
        lifeRemain = life;
    }

    public void LoseLife()
    {
        if (lifeRemain == 0) return;

        lifeRemain--;
        lifeImages[lifeRemain].SetActive(false);
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
