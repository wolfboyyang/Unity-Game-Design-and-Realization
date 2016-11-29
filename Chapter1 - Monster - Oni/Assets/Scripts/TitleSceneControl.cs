using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneControl : MonoBehaviour {

    enum State
    {
        None = -1,

        Title,              // Show Title & wait for touch
        WaitSoundEffectEnd, // Wait for sound effect
        WaitFade,            // Wait for fade
        LoadGameScene
    }

    private State state = State.None;
    private State nextState = State.None;
    private float stateTime = 0.0f;

    private FadeControl fadeControl;

    [SerializeField]
    private Image startImage;
    private AudioSource startAudio;

    public const float TitleAnimationTime = 0.1f;
    public const float FadeTime = 1.0f;

	// Use this for initialization
	void Start () {
        // set player uncontrollable
        PlayerControl player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        player.Playable = false;

        fadeControl = FadeControl.Instance;
        fadeControl.Fade(FadeTime, new Color(0, 0, 0, 1.0f), new Color(0, 0, 0, 0));

        startAudio = GetComponent<AudioSource>();

        nextState = State.Title;

	}
	
	// Update is called once per frame
	void Update () {
        stateTime += Time.deltaTime;

        switch (state)
        {
            case State.Title:
                {
                    if (Input.GetMouseButton(0))
                        nextState = State.WaitSoundEffectEnd;

                    break;
                }
            case State.WaitSoundEffectEnd:
                {
                    // Scale Animation for start image.
                    float rate = stateTime / TitleAnimationTime;
                    float scale = Mathf.Lerp(2.0f, 1.0f, rate);

                    startImage.rectTransform.localScale = Vector3.one * scale;

                    // Check whether start audio is over
                    if(!startAudio.isPlaying || startAudio.time >= startAudio.clip.length)
                        nextState = State.WaitFade;

                    break;
                }
            case State.WaitFade:
                {
                    if (!fadeControl.IsActive)
                        nextState = State.LoadGameScene;

                    break;
                }
        }

        if(nextState != State.None)
        {
            switch (nextState)
            {
                case State.WaitSoundEffectEnd:
                    {
                        startAudio.Play();
                        break;
                    }
                case State.WaitFade:
                    {
                        fadeControl.Fade(FadeTime, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1.0f));
                        break;
                    }
                case State.LoadGameScene:
                    {
                        SceneManager.LoadScene("GameScene");
                        break;
                    }
            }

            state = nextState;
            nextState = State.None;
            stateTime = 0;
        }
	}
}
