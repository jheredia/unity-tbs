using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    const string IS_WALKING_PARAM = "IsWalking";
    const string SHOOT_TRIGGER = "Shoot";
    const string MELEE_SWORD_TRIGGER = "Melee Sword";

    [SerializeField] private Animator unitAnimator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform laserProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;


    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent(out AttackAction attackAction))
        {
            attackAction.OnAttack += AttackAction_OnAttack;
        }
        if (TryGetComponent(out MeleeAction meleeAction))
        {
            meleeAction.OnSwordActionStarted += MeleeAction_OnSwordActionStarted;
            meleeAction.OnSwordActionCompleted += MeleeAction_OnSwordActionCompleted;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        unitAnimator.SetBool(IS_WALKING_PARAM, true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        unitAnimator.SetBool(IS_WALKING_PARAM, false);
    }

    private void AttackAction_OnAttack(object sender, AttackAction.OnAttackEventArgs e)
    {
        unitAnimator.SetTrigger(SHOOT_TRIGGER);
        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        if (bulletProjectileTransform.TryGetComponent(out BulletProjectile bulletProjectile))
        {
            Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

            targetUnitShootAtPosition.y = shootPointTransform.position.y;
            bulletProjectile.Setup(targetUnitShootAtPosition);
        }
    }

    private void MeleeAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        unitAnimator.SetTrigger(MELEE_SWORD_TRIGGER);
    }

    private void MeleeAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
    }
}
