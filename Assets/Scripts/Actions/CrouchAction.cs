using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchAction : BaseAction
{
    private float totalCrouchAmount;
    [SerializeField] private Animator unitAnimator;
    private const float crouchSpeed = 10f;
    private const float crouchDelta = 10f;
    const string IS_WALKING_PARAM = "IsWalking";
    const string IS_CROUCHING_PARAM = "IsCrouching";
    const string IS_STANDING_PARAM = "IsStanding";
    const string IS_CROUCHED_PARAM = "IsCrouched";

    const int actionPointsCost = 2;

    void Update()
    {
        if (!isActive) return;
        bool IsCrouched = unitAnimator.GetBool(IS_CROUCHED_PARAM);
        if (IsCrouched)
        {
            CrouchToStand();
        }
        else
        {
            StandToCrouch();
        }

    }

    private void CrouchToStand()
    {
        unitAnimator.SetBool(IS_STANDING_PARAM, true);

        float crouchAmount = crouchSpeed * Time.deltaTime;
        totalCrouchAmount -= crouchAmount;
        if (totalCrouchAmount <= 0)
        {
            unitAnimator.SetBool(IS_CROUCHED_PARAM, false);
            unitAnimator.SetBool(IS_STANDING_PARAM, false);
            ActionComplete();
        }
    }

    private void StandToCrouch()
    {
        unitAnimator.SetBool(IS_CROUCHING_PARAM, true);

        float crouchAddAmount = crouchSpeed * Time.deltaTime;
        totalCrouchAmount += crouchAddAmount;
        if (totalCrouchAmount >= crouchDelta)
        {
            unitAnimator.SetBool(IS_CROUCHED_PARAM, true);
            unitAnimator.SetBool(IS_CROUCHING_PARAM, false);
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "crouch";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override int GetActionPointsCost()
    {
        return actionPointsCost;
    }
}
