using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    const string IS_WALKING_PARAM = "IsWalking";

    [SerializeField] private Animator unitAnimator;

    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent(out AttackAction attackAction))
        {
            attackAction.OnShoot += AttackAction_OnShoot;
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

    private void AttackAction_OnShoot(object sender, EventArgs e)
    {
        unitAnimator.SetTrigger("Shoot");
    }
}