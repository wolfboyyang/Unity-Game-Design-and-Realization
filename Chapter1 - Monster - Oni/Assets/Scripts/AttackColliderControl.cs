using UnityEngine;
using System.Collections;

public class AttackColliderControl : MonoBehaviour {

    public PlayerControl player;

    private bool isPowered = false;

	// Use this for initialization
	void Start () {
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPowered || other.tag != "OniGroup") return;

        var oniGroup = other.GetComponent<OniGroupControl>();

        oniGroup.OnAttackFromPlayer();
        player.ResetAttackDisableTimer();
        player.PlayHitEffect(oniGroup.transform.position);
        player.PlayHitSound();
    }

    public void SetPowered(bool power)
    {
        isPowered = power;
    }
}
