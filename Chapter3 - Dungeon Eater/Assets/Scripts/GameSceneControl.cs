using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneControl : MonoBehaviour
{

    private const int StartStage = 0;

    public Map map;

    public const int Retry = 2;

    public GameObject enemyPrefab;
    public GameObject treasureGenerator;

    public AudioClip bgmAudioClip;
    public AudioClip stageClearSound;
    public AudioClip gameOverSound;

    private int retryRemain;
    private List<GameObject> objList = new List<GameObject>();
    private int stage = StartStage;

    // Use this for initialization
    void Start()
    {
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {
        retryRemain = Retry;

        OnStageStart();
    }

    public void OnEatAll()
    {

    }

    public void OnStageStart()
    {
        GameObject.FindGameObjectWithTag("Map").SendMessage("OnStageStart", stage);
        GameObject.FindGameObjectWithTag("Player").SendMessage("OnStageStart");
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
}
