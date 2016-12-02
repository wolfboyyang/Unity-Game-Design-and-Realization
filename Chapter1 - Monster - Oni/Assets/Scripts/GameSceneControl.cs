using UnityEngine;
using System.Collections;

public class GameSceneControl : MonoBehaviour {

    public GameObject oniGroupPrefab;
    public GameObject oniPrefab;
    public GameObject oniEmitterPrefab;
    public GameObject[] OniHillPrefab;

    public AudioClip gameStartSound;
    public AudioClip evaluationSound;
    public AudioClip returnSound;

    public float OniAppearDistance = 10;

    public PlayerControl player;
    public ScoreControl scoreControl;
    public LevelControl levelControl;
    public ResultControl resultControl;
    public OniEmitterControl oniEmitter;

    private GameUIControl gameUIControl;
    private FadeControl fadeControl;

    // -------------------------------------------------------------------------------- //

    private bool canDispatch = false;
    private int OniCount = 0;
    private float oniLastAppearTime = 0;
    public const int OniMaxCount = 3;
    public const float OniAppearTime = 2.0f;

    enum State
    {
        None = -1,

        Start,
        Game,
        WaitOniVanish,
        LastRun,
        WaitPlayerStop,

        Goal,
        WaitOniFall,
        ResultDefeat,
        ResultEvaluation,
        ResultTotal,

        GameOver,
        GotoTitle
    }

    [SerializeField]
    private State state = State.None;
    private State nextState = State.None;
    private float timer = 0.0f;
    private float preStateTimer = 0.0f;

    public enum Evaluation
    {
        None = -1,

        Okay = 0,
        Good,
        Great,

        Miss,
        Num
    }
    public float evaluationOkayRate = 1.0f;
    public float evaluationGoodRate = 1.0f;
    public float evaluationGreatRate = 1.0f;
    public int evaluationRate = 1;

    public Evaluation evaluation = Evaluation.None;

    // -------------------------------------------------------------------------------- //

    public struct Result
    {
        public int oniDefeatNum;
        public int[] evaluationCount;

        public int rank;

        public float score;
        public float scoreMax;
    }

    public Result result;

    // -------------------------------------------------------------------------------- //

    public const int OniAppearNumMax = 10;
    public int oniGroupAppearMax = 50;

    public const int OniGroupPenalty = 1;
    public const float ScoreHideNum = 40;

    public int oniGroupNum = 0;
    // defeat or miss
    public int oniGroupComplite = 0;
    public int oniGroupDefeatNum = 0;
    public int oniGroupMissNum = 0;

    private const float StartTime = 2.0f;
    private const float GoalStopDistance = 8.0f;

    public const float AttackTimeGreeat = 0.05f;
    public const float AttackTimeGood = 0.10f;

    private int backupOniDefeatNum = -1;


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        player.gameSceneControl = this;

        levelControl = new LevelControl();
        levelControl.gameSceneControl = this;
        levelControl.player = player;
        levelControl.oniGroupPrefab = oniGroupPrefab;
        levelControl.Create();

        resultControl = new ResultControl();

        gameUIControl = GameUIControl.Instance;
        scoreControl = gameUIControl.scoreControl;

        result.oniDefeatNum = 0;
        result.evaluationCount = new int[(int)Evaluation.Num];
        result.rank = 0;
        result.score = 0;
        result.scoreMax = 0;

        for (int i = 0; i < result.evaluationCount.Length; i++)
            result.evaluationCount[i] = 0;

        fadeControl = FadeControl.Instance;
        fadeControl.Fade(3.0f, new Color(0, 0, 0, 1.0f), new Color(0, 0, 0, 0));

        nextState = State.Start;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Debug.Break();

        preStateTimer = timer;
        timer += Time.deltaTime;

        switch (state)
        {
            case State.Start:
                {
                    if (timer > StartTime)
                    {
                        gameUIControl.SetStartVisible(false);
                        nextState = State.Game;
                    }
                    break;
                }
            case State.Game:
                {
                    levelControl.OniAppearControl();

                    if (oniGroupComplite >= oniGroupAppearMax)
                        nextState = State.WaitOniVanish;

                    if (oniGroupComplite >= ScoreHideNum && backupOniDefeatNum == -1)
                        backupOniDefeatNum = result.oniDefeatNum;

                    break;
                }
            case State.WaitOniVanish:
                {
                    if (GameObject.FindGameObjectsWithTag("OniGroup").Length == 0 && player.GetSpeedRate() > 0.5f)
                        nextState = State.LastRun;

                    break;
                }
            case State.LastRun:
                {
                    if (timer > 2.0f)
                        nextState = State.WaitPlayerStop;

                    break;
                }
            case State.WaitPlayerStop:
                {
                    if (player.IsStopped)
                    {
                        scoreControl.SetScoreForce(backupOniDefeatNum);
                        scoreControl.SetScore(result.oniDefeatNum);
                        nextState = State.Goal;
                    }
                    break;
                }
            case State.Goal:
                {
                    if (oniEmitter.oniNum == 0)
                        nextState = State.WaitOniFall;

                    break;
                }
            case State.WaitOniFall:
                {
                    if (!scoreControl.IsActive && timer > 1.5f)
                        nextState = State.ResultDefeat;

                    break;
                }
            case State.ResultDefeat:
                {
                    if (timer >= 0.4f && preStateTimer < 0.4f)
                        GetComponent<AudioSource>().PlayOneShot(evaluationSound);
                    if (timer > 0.5f)
                        nextState = State.ResultEvaluation;

                    break;
                }
            case State.ResultEvaluation:
                {
                    if (timer >= 0.4f && preStateTimer < 0.4f)
                        GetComponent<AudioSource>().PlayOneShot(evaluationSound);
                    if (timer > 2.0f)
                        nextState = State.ResultTotal;

                    break;
                }
            case State.ResultTotal:
                {
                    if (timer >= 0.4f && preStateTimer < 0.4f)
                        GetComponent<AudioSource>().PlayOneShot(evaluationSound);
                    if (timer > 2.0f)
                        nextState = State.GameOver;

                    break;
                }
            case State.GameOver:
                {
                    if (Input.GetMouseButton(0))
                    {
                        fadeControl.Fade(1.0f, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1.0f));
                        GetComponent<AudioSource>().PlayOneShot(returnSound);
                        nextState = State.GotoTitle;
                    }
                    break;
                }
            case State.GotoTitle:
                {
                    if (fadeControl.IsActive)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");

                    break;
                }

        }

        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Start:
                    {
                        gameUIControl.SetStartVisible(true);
                        break;
                    }
                case State.WaitPlayerStop:
                    {
                        player.Stop();
                        int totalRank = resultControl.GetTotalRank();
                        if (totalRank > 0)
                        {
                            var oniHill = Instantiate<GameObject>(OniHillPrefab[totalRank]);
                            var oniHillPosition = player.transform.position;
                            oniHillPosition.x += player.DistanceToStop();
                            oniHillPosition.x += GoalStopDistance;
                            oniHillPosition.y = 0.0f;
                            oniHill.transform.position = oniHillPosition;
                        }

                        break;
                    }
                case State.Goal:
                    {
                        var go = Instantiate<GameObject>(oniEmitterPrefab);
                        oniEmitter = go.GetComponent<OniEmitterControl>();
                        var emitterPosition = oniEmitter.transform.position;

                        emitterPosition.x = player.transform.position.x;
                        emitterPosition.x += player.DistanceToStop();
                        emitterPosition.x += GameSceneControl.GoalStopDistance;

                        oniEmitter.transform.position = emitterPosition;

                        int oniNum = 0;
                        switch (resultControl.GetTotalRank())
                        {
                            // bad rank
                            case 0: oniNum = Mathf.Min(result.oniDefeatNum, 10); break;
                            // normal rank
                            case 1: oniNum = 6; break;
                            // good rank
                            case 2: oniNum = 10; break;
                            // excellent rank
                            case 3: oniNum = 20; break;
                        }
                        oniEmitter.oniNum = oniNum;
                        if (oniNum == 0) oniEmitter.isEnableHitSound = false;
                        break;
                    }
                case State.ResultDefeat:
                    {
                        oniEmitter.isEnableHitSound = false;
                        gameUIControl.DisplayDefeatRank(true);
                        break;
                    }
                case State.ResultEvaluation:
                    {
                        gameUIControl.DisplayEvaluationRank(true);
                        break;
                    }
                case State.ResultTotal:
                    {
                        gameUIControl.DisplayDefeatRank(false);
                        gameUIControl.DisplayEvaluationRank(false);

                        gameUIControl.DisplayTotalRank(true);
                        break;
                    }
                case State.GameOver:
                    {
                        gameUIControl.SetReturnVisible(true);
                        break;
                    }
            }

            state = nextState;
            nextState = State.None;
            timer = 0.0f;
            preStateTimer = -1.0f;
        }
    }

    public void OnPlayerMissed()
    {
        oniGroupMissNum++;
        oniGroupComplite++;
        oniGroupAppearMax -= OniGroupPenalty;

        levelControl.OnPlayerMissed();
        evaluation = Evaluation.Miss;
        result.evaluationCount[(int)evaluation]++;

        var oniGroups = GameObject.FindGameObjectsWithTag("OniGroup");
        foreach(var oniGroup in oniGroups)
        {
            oniGroupNum--;
            oniGroup.GetComponent<OniGroupControl>().BeginLeave();
        }
    }

    public void AddDefeatNum(int num)
    {
        oniGroupDefeatNum++;
        oniGroupComplite++;
        result.oniDefeatNum += num;

        float attackTime = player.GetAttackTime();

        if (attackTime < AttackTimeGreeat)
            evaluation = Evaluation.Great;
        else if (attackTime < AttackTimeGood)
            evaluation = Evaluation.Good;
        else
            evaluation = Evaluation.Okay;

        result.evaluationCount[(int)evaluation] += num;

        // caculate score
        float[] scores = { evaluationOkayRate, evaluationGoodRate, evaluationGreatRate, 0 };
        result.scoreMax += num * evaluationGreatRate;
        result.score += num * scores[(int)evaluation];

        resultControl.AddOniDefeatScore(num);
        resultControl.AddEvaluationScore(evaluation);
    }

    public bool ShouldDrawScore()
    {
        if (oniGroupComplite >= ScoreHideNum)
            return false;
        else
            return true;
    }

    public void AddScoreMax(int oniNum)
    {
        result.scoreMax += evaluationOkayRate * OniMaxCount * evaluationRate; 
    }

    private static GameSceneControl _instance;
    public static GameSceneControl Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = GameObject.Find("GameSceneControl");
                if (go != null)
                    _instance = go.GetComponent<GameSceneControl>();
                else
                    Debug.LogError("Cannot find GameObject \"GameSceneControl\".");
            }
            return _instance;
        }
    }
}
