using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneControl : MonoBehaviour
{
    private bool DisableMonsterSpawn = false;
    private const int StartStage = 0;

    public Map map;

    public const int RetryMax = 2;

    public GameObject enemyPrefab;
    public GameObject treasureGeneratorPrefab;

    public AudioClip bgmAudioClip;
    public AudioClip stageClearSound;
    public AudioClip gameOverSound;

    public GameUIControl gameUIControl;

    private int retryRemain;
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
        retryRemain = RetryMax;
        gameUIControl.SetLife(retryRemain);

        score = 0;
        gameUIControl.SetScore(0);

        OnStageStart();
    }

    public void OnStageStart()
    {
        // clean stage
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
            Destroy(enemy);

        // start stage
        map.SendMessage("OnStageStart", stage);

        if (!DisableMonsterSpawn)
        {
            for(int i=1;i < 5; i++)
            {
                var position = map.GetSpawnPoint((Map.SpawnPointType)i);
                if (position == Vector3.zero) continue;

                var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                enemy.SendMessage("SetSpawnPosition", position);
                enemy.SendMessage("OnStageStart");
            }
        }


        GameObject.FindGameObjectWithTag("Player").SendMessage("OnStageStart");

        gameUIControl.SetStage(stage);
        gameUIControl.DrawStageClear(false);

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

        // Let player & ghost move
        HitStop(false);

        GetComponent<AudioSource>().Play();
        gameUIControl.DrawStageStart(true);
        StartCoroutine("StageStartWait");    
    }

    IEnumerator StageStartWait()
    {
        yield return new WaitForSeconds(1.0f);
        gameUIControl.DrawStageStart(false);
    }

    public void OnEatAll()
    {
        gameUIControl.DrawStageClear(true);

        // stop player & ghost move
        HitStop(true);

        StartCoroutine("StageClear");
    }

    public void HitStop(bool enable)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
            enemy.SendMessage("HitStop", enable);
        GameObject.FindGameObjectWithTag("Player").SendMessage("HitStop", enable);
    }

    public void OnAttackBegin()
    {
        HitStop(true);
    }

    public void OnAttackEnd()
    {
        HitStop(false);
    }

    public void AddScore(int score)
    {
        this.score += score;
        gameUIControl.SetScore(this.score);
    }

    private IEnumerator StageClear()
    {
        GetComponent<AudioSource>().PlayOneShot(stageClearSound);
        yield return new WaitForSeconds(3.0f);
        stage++;
        OnStageStart();
    }

    public void PlayerIsDead()
    {
        if (retryRemain == 0)
            StartCoroutine("GameOver");
        else
            StartCoroutine("Retry");
    }

    IEnumerator Retry()
    {
        retryRemain--;
        gameUIControl.LoseLife();
        yield return new WaitForSeconds(3.0f);

        Restart();
    }

    IEnumerator GameOver()
    {
        GetComponent<AudioSource>().PlayOneShot(gameOverSound);
        gameUIControl.DrawGameOver(true);
        yield return new WaitForSeconds(5.0f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

    public void Restart()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
            enemy.SendMessage("OnRestart");

        GameObject.FindGameObjectWithTag("Player").SendMessage("OnRestart");

        if (treasureGenerator != null)
            treasureGenerator.SendMessage("OnRestart");

        // Let player & ghost move
        HitStop(false);

        GetComponent<AudioSource>().Play();
        gameUIControl.DrawStageStart(true);
        StartCoroutine("StageStartWait");
    }
}
