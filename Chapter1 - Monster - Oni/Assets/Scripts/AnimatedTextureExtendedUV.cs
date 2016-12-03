using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTextureExtendedUV : MonoBehaviour {
    
    public int rowCount = 2;
    public int columnCount = 4;

    public int rowNumber = 0;
    public int columnNumber = 0;
    public int totalCells = 8;
    public int fps = 30;

    private Vector2 offset;
    private float timer = 0.0f;
    private bool isPlaying = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update()
    {
        if (!isPlaying) return;

        if (timer * fps >= totalCells)
        {
            Stop();
            return;
        }
        else
        {
            SetSpriteAnimation(rowCount, columnCount, rowNumber, columnNumber, totalCells, fps);
            SetVisible(true);

            timer += Time.deltaTime;
        }
		
	}

    private void SetSpriteAnimation(int rowCount, int columnCount, int rowNumber, int columnNumber, int totalCells, int fps)
    {
        // Calculate index
        int index = (int)(timer * fps);
        // Repeat when exhausting all cells
        index %= totalCells;
        // Size of every cell
        float sizeX = 1.0f / columnCount;
        float sizeY = 1.0f / rowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        // split into horizontal and vertical index
        var uIndex = index % columnCount;
        var vIndex = index / columnCount;
        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        float offsetX = (uIndex + columnNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);

        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", size);
    }

    private void SetVisible(bool visible)
    {
        GetComponent<Renderer>().enabled = visible;
    }

    public void Play(float startTime)
    {
        timer = startTime;
        isPlaying = true;
    }

    public void Stop()
    {
        timer = 0.0f;
        isPlaying = false;
        SetVisible(false);
    }

    public bool IsPlaying { get { return IsPlaying; } }
}
