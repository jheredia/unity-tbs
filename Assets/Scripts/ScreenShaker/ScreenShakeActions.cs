using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{


    private void Start()
    {
        AttackAction.OnAnyAttack += AttackAction_OnAnyAttack;
        GrenadeProjectile.OnAnyGrenadeExplosion += GrenadeAction_OnAnyGrenadeExplosion;
        MeleeAction.OnAnySwordHit += MeleeAction_OnAnySwordHit;
    }

    private void MeleeAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.ShakeScreen();
    }

    private void AttackAction_OnAnyAttack(object sender, AttackAction.OnAttackEventArgs e)
    {
        ScreenShake.Instance.ShakeScreen();
    }

    private void GrenadeAction_OnAnyGrenadeExplosion(object sender, EventArgs e)
    {
        ScreenShake.Instance.ShakeScreen(5f);
    }

}
