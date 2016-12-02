using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeControl : MonoBehaviour {

    private float timer;// Current time
    private float fadeTime; // Fade duration
    private Color colorStart; // Color when fade starts
    private Color colorEnd; // Color when fade ends

    [SerializeField]
    private Image fadeImage;

	void Awake ()
    {
        timer = 0.0f;
        fadeTime = 0.0f;
        colorStart = new Color(0, 0, 0, 0);
        colorEnd = new Color(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(timer < fadeTime)
        {
            float rate = 1.0f;
            if(fadeTime!=0) rate = timer / fadeTime;

            rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
            Color color = Color.Lerp(colorStart, colorEnd, rate);

            fadeImage.color = color;

            timer += Time.deltaTime;
        }
	}

    /// <summary>
    /// Fade Image from start color to end color in time
    /// </summary>
    /// <param name="time">fade duration</param>
    /// <param name="start">fade start color</param>
    /// <param name="end">fade end color</param>
    public void Fade(float time, Color start, Color end)
    {
        fadeImage.gameObject.SetActive(true);

        fadeTime = time;
        colorStart = start;
        colorEnd = end;

        this.timer = 0;
    }

    public bool IsActive
    {
        get { return timer < fadeTime; }
    }

    private static FadeControl _instance;
    public static FadeControl Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = GameObject.Find("FadeControl");
                if (go != null)
                    _instance = go.GetComponent<FadeControl>();
                else
                    Debug.LogError("Cannot find GameObject \"FadeControl\".");
            }
            return _instance;
        }
    }
}
