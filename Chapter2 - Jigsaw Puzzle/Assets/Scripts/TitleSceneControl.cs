using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneControl : MonoBehaviour
{

    enum State
    {
        None = -1,
        Wait,
        PlayJigsaw
    }

    private State state = State.None;
    private State nextState = State.None;
    private float timer = 0.0f;

    // Use this for initialization
    void Start()
    {
        nextState = State.Wait;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case State.Wait:
                if (Input.GetMouseButton(0))
                    nextState = State.PlayJigsaw;
                break;
            case State.PlayJigsaw:
                if (timer > GetComponent<AudioSource>().clip.length + 0.5f)
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                break;
        }

        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.PlayJigsaw:
                    GetComponent<AudioSource>().Play();
                    break;
            }

            state = nextState;
            nextState = State.None;
            timer = 0.0f;
        }
    }
}
