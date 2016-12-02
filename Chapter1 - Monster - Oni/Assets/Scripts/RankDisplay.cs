using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RankDisplay : MonoBehaviour {

    private const float ZoomTime = 0.4f;

    private float timer = 0.0f;
    private float scale = 1.0f;
    private float alpha = 0.0f;

    public Image gradeImage;
    public Sprite[] rankSprites;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        UpdateRank();

        timer += Time.deltaTime;
    }

    private void UpdateRank()
    {
        float zoomInTime = ZoomTime;
        float rate = 0.0f;
        if (timer < zoomInTime)
        {
            rate = timer / zoomInTime;
            rate = Mathf.Pow(rate, 2.5f);
            scale = Mathf.Lerp(1.5f, 1.0f, rate);
            alpha = Mathf.Lerp(0.0f, 1.0f, rate);
        }
        else
        {
            scale = 1.0f;
            alpha = 1.0f;
        }

        var images = GetComponentsInChildren<Image>();
        foreach(var image in images)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        GetComponent<RectTransform>().localScale = Vector3.one * scale;    
    }

    public void StartDisplay(int rank)
    {
        gradeImage.sprite = rankSprites[rank];
        gameObject.SetActive(true);
        timer = 0.0f;

        UpdateRank();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
