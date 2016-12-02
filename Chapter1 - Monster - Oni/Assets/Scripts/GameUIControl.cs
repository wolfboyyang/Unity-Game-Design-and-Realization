using UnityEngine;
using System.Collections;

public class GameUIControl : MonoBehaviour {

    public GameSceneControl gameSceneControl;
    public ScoreControl scoreControl;

    public GameObject startImage;
    public GameObject returnImage;

    public RankDisplay defeatRank;
    public RankDisplay evaluationRank;
    public RankDisplay totalRank;

    public Sprite[] gradeSmallSprites;
    public Sprite[] gradeSprites;

    void Awake()
    {
        gameSceneControl = GameSceneControl.Instance;
        scoreControl = GetComponent<ScoreControl>();

        scoreControl.SetScoreForce(gameSceneControl.result.oniDefeatNum);

        defeatRank.rankSprites = gradeSmallSprites;
        evaluationRank.rankSprites = gradeSmallSprites;
        totalRank.rankSprites = gradeSprites;
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        scoreControl.SetScore(gameSceneControl.result.oniDefeatNum);
	}

    public void SetStartVisible(bool visible)
    {
        startImage.SetActive(visible);
    }

    public void SetReturnVisible(bool visible)
    {
        returnImage.SetActive(visible);
    }

    public void DisplayDefeatRank(bool visible)
    {
        if (visible)
        {
            int rank = gameSceneControl.resultControl.GetDefeatRank();
            defeatRank.StartDisplay(rank);
        }
        else
            defeatRank.Hide();
        
    }

    public void DisplayEvaluationRank(bool visible)
    {
        if (visible)
        {
            int rank = gameSceneControl.resultControl.GetEvaluationRank();
            evaluationRank.StartDisplay(rank);
        }
        else
            evaluationRank.Hide();
    }

    public void DisplayTotalRank(bool visible)
    {
        if (visible)
        {
            int rank = gameSceneControl.resultControl.GetTotalRank();
            totalRank.StartDisplay(rank);
        }
        else
            totalRank.Hide();
    }

    private static GameUIControl _instance;
    public static GameUIControl Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = GameObject.Find("GameCanvas");
                if (go != null)
                    _instance = go.GetComponent<GameUIControl>();
                else
                    Debug.LogError("Cannot find GameObject \"GameCanvas\".");
            }
            return _instance;
        }
    }
}
