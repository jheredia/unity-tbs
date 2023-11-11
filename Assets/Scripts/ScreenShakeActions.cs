using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{


    private void Start()
    {
        AttackAction.OnAnyAttack += AttackAction_OnAnyAttack;
    }

    private void AttackAction_OnAnyAttack(object sender, AttackAction.OnAttackEventArgs e)
    {
        ScreenShake.Instance.ShakeScreen();
    }
}
