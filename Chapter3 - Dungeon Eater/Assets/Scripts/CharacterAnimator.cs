using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    private Animator animator;
    private GridMove gridMove;
    private bool dead = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Cannot find Animator!");

        gridMove = GetComponent<GridMove>();
        dead = false;
	}
	
	// Update is called once per frame
	void Update () {
        var targetRotation = Quaternion.LookRotation(gridMove.Direction);
        float t = 1.0f - Mathf.Pow(0.75f, Time.deltaTime * 30.0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
        animator.SetBool("isWalking", gridMove.IsWalking);
	}

    public void OnStageStart()
    {
        dead = false;
        animator.Play("Idle");
    }
}
