using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneControl : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayButtonPressed()
    {
        GetComponent<AudioSource>().Play();

        Invoke("StartGame", 1.0f);
    }

    void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene");
    }

}
