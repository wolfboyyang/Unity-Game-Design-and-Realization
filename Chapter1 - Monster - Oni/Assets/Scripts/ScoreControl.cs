using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour {
    
    // display scale effect for zero
    public bool drawZero;

    public GameObject scoreUI;
    public Image[] digitImages;
    public Sprite[] numberSprites;

    public AudioClip[] countUpSounds;
    public AudioSource countUpAudio;

    private int targetScore;
    private int currentScore;
    private float timer;

	// Use this for initialization
	void Start () {
        countUpAudio = gameObject.AddComponent<AudioSource>();
        timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	    if(targetScore > currentScore)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                int index = Random.Range(0, countUpSounds.Length);
                countUpAudio.PlayOneShot(countUpSounds[index]);

                timer = 0.0f;

                if (targetScore - currentScore > 10)
                    currentScore += 5;
                else
                    currentScore++;
            }

            float scale = 1.0f;
            // Scale the Number From big to normal to give an impact effect.
            if (targetScore != currentScore)
                scale = 2.5f - 1.5f * timer * 10.0f;

            int displayScore = Mathf.Max(0, currentScore);

            for(int i = 0; i < digitImages.Length; i++)
            {
                int digit = displayScore % 10;
                digitImages[i].sprite = numberSprites[digit];

                if (digit!=0 || drawZero)
                    digitImages[i].rectTransform.localScale = Vector3.one * scale;

                displayScore /= 10;
            }
        }
	}

    public void SetScore(int score)
    {
        if (targetScore == currentScore)
            timer = 0.0f;

        targetScore = score;
    }

    public void SetScoreForce(int score)
    {
        targetScore = score;
        currentScore = score;
    }

    public bool IsActive { get { return targetScore != currentScore; } }
}
