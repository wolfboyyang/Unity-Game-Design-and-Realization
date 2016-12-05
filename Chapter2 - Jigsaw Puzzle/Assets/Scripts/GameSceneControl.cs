using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneControl : MonoBehaviour
{

    enum State
    {
        None,
        Play,
        Clear
    }

    private State state = State.None;
    private State nextState = State.None;
    private float timer = 0.0f;

    public GameObject puzzlePrefab;
    public PuzzleControl puzzleControl;
    public GameObject retryButton;
    public GameObject CompleteImage;

    public enum SoundEffect
    {
        Grab,
        Release,
        Attach,
        Complete,
        Button
    }
    public AudioClip[] sounds;

    // Use this for initialization
    void Start()
    {
        var go = Instantiate<GameObject>(puzzlePrefab);
        puzzleControl = go.GetComponent<PuzzleControl>();
        puzzleControl.gameSceneControl = this;
        nextState = State.Play;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case State.Play:
                if (puzzleControl.IsCleared())
                    nextState = State.Clear;
                break;

            case State.Clear:
                if(timer > sounds[(int)SoundEffect.Complete].length + 0.5f)
                {
                    if (Input.GetMouseButtonDown(0))
                        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
                }
                break;
        }

        if (nextState != State.None)
        {
            switch (nextState)
            {
                case State.Play:
                    break;
                case State.Clear:
                    {
                        retryButton.SetActive(false);
                        CompleteImage.SetActive(true);
                        break;
                    }
            }

            state = nextState;
            nextState = State.None;
            timer = 0.0f;
        }
    }

    public void OnRetryButtonPressed()
    {
        if (!puzzleControl.IsCleared()) {
            puzzleControl.Restart();
            PlaySound(SoundEffect.Button);
        }
    }

    public void PlaySound(SoundEffect se)
    {
        GetComponent<AudioSource>().PlayOneShot(sounds[(int)se]);
    }
}
