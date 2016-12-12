using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public GameObject sword;
    public AudioClip attackSound;

    private bool equiped = false;
    private Transform attackTarget;

    private const int AttackPoint = 500;
    private const int ComboBonus = 200;

    private int combo = 0;
    private Animator animator;
    private GameObject gameController;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        gameController = GameObject.FindGameObjectWithTag("GameController");

        equiped = false;
        sword.GetComponent<Renderer>().enabled = false;
        combo = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStageStart()
    {
        equiped = false;
        sword.GetComponent<Renderer>().enabled = false;
    }

    public void OnGetSword()
    {
        if (!equiped)
        {
            sword.GetComponent<Renderer>().enabled = true;
            equiped = true;
            animator.SetTrigger("BeginIdleSword");
        }
        else
        {
            var point = AttackPoint + ComboBonus * combo;
            gameController.SendMessage("AddScore", point);
        }
    }

    public void AutoAttack(Transform other)
    {
        if (equiped)
        {
            attackTarget = other;
            StartCoroutine("SwordAutoAttack");
        }
    }

    public bool CanAutoAttack()
    {
        return equiped;
    }

    IEnumerator SwordAutoAttack()
    {
        gameController.SendMessage("OnAttackBegin");
        var targetPosition = attackTarget.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        yield return null;

        animator.SetTrigger("BeginAttack");
        yield return new WaitForSeconds(0.3f);

        GetComponent<AudioSource>().PlayOneShot(attackSound);
        yield return new WaitForSeconds(0.2f);

        attackTarget.SendMessage("OnDamage");

        yield return null;

        RemoveSword();
        gameController.SendMessage("OnAttackEnd");
    }

    private void RemoveSword()
    {
        sword.GetComponent<Renderer>().enabled = false;
        equiped = false;
        combo = 0;

        SendMessage("OnMonsterDead");
    }
}
