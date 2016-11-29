using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeControl : MonoBehaviour {

    private float time;// Current time
    private float fadeTime; // Fade duration
    private Color colorStart; // Color when fade starts
    private Color colorEnd; // Color when fade ends

    [SerializeField]
    private Image fadeImage;

	void Awake ()
    {
        time = 0.0f;
        fadeTime = 0.0f;
        colorStart = new Color(0, 0, 0, 0);
        colorEnd = new Color(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(time < fadeTime)
        {
            float rate = 1.0f;
            if(fadeTime!=0) rate = time / fadeTime;

            rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
            Color color = Color.Lerp(colorStart, colorEnd, rate);

            fadeImage.color = color;

            time += Time.deltaTime;
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

        this.time = 0;
    }

    public bool IsActive
    {
        get { return time < fadeTime; }
    }

    private static FadeControl instance;
    public static FadeControl Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find("FadeControl");
                if (go != null)
                    instance = go.GetComponent<FadeControl>();
                else
                    Debug.LogError("Cannot find GameObject \"FadeControl\".");
            }
            return instance;
        }
    }
}
