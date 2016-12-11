using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneControl : MonoBehaviour
{

    private const int StartStage = 0;

    public Map map;

    public const int Retry = 2;

    public GameObject enemyPrefab;
    public GameObject treasureGeneratorPrefab;

    public AudioClip bgmAudioClip;
    public AudioClip stageClearSound;
    public AudioClip gameOverSound;

    public GameUIControl gameUIControl;

    private int retryRemain;
    private List<GameObject> objList = new List<GameObject>();
    private int stage = StartStage;
    private GameObject treasureGenerator;

    private int score;

    enum State
    {
        None,
        Start,
        Play,
        GameOver,
    }

    private State state = State.None;
    private State nextState = State.None;

    // Use this for initialization
    void Start()
    {
        nextState = State.Start;
    }

    // Update is called once per frame
    void Update()
    {
        if(nextState != State.None)
        {
            switch (nextState)
            {
                case State.Start:
                    GameStart();
                    break;
            }

            state = nextState;
            nextState = State.None;
        }
    }

    public void GameStart()
    {
        retryRemain = Retry;
        score = 0;
        gameUIControl.SetScore(0);

        OnStageStart();
    }

    public void OnEatAll()
    {
        Debug.Log("Eat All");
    }

    public void OnStageStart()
    {
        map.SendMessage("OnStageStart", stage);
        GameObject.FindGameObjectWithTag("Player").SendMessage("OnStageStart");

        gameUIControl.SetStage(stage);
        var treasurePosition = map.GetSpawnPoint(Map.SpawnPointType.Treasure);
        if (treasurePosition != Vector3.zero)
        {
            if (treasureGenerator == null)
                treasureGenerator = Instantiate(treasureGeneratorPrefab, treasurePosition, Quaternion.identity);
            else
                treasureGenerator.transform.position = treasurePosition;
        }
        else if (treasureGenerator != null)
            Destroy(treasureGenerator);
            
    }

    public void StopHit(bool enable)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
            enemy.SendMessage("StopHit", enable);

        GameObject.FindGameObjectWithTag("Player").SendMessage("StopHit", enable);
    }

    public void OnAttack()
    {

    }

    public void OnEndAttack()
    {

    }

    public void AddScore(int score)
    {
        this.score += score;
        gameUIControl.SetScore(this.score);
    }

}
