using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private GameObject overlayUICanvas;
    // Start is called before the first frame update
    void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        ShowActionCamera(false);
    }


    private void ShowActionCamera(bool show = true)
    {
        if (actionCameraGameObject == null) return;
        actionCameraGameObject.SetActive(show);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case AttackAction attackAction:
                if (UnityEngine.Random.Range(1, 101) <= 40)
                {
                    Unit attackingUnit = attackAction.GetUnit();
                    Unit targetUnit = attackAction.GetTargetUnit();
                    PositionActionCamera(attackingUnit, targetUnit);
                    ShowActionCamera();
                }
                break;
            case MeleeAction meleeAction:
                if (UnityEngine.Random.Range(1, 101) <= 60)
                {
                    Unit attackingUnit = meleeAction.GetUnit();
                    Unit targetUnit = meleeAction.GetTargetUnit();
                    PositionActionCamera(attackingUnit, targetUnit);
                    ShowActionCamera();
                }
                break;
            case GrenadeAction grenadeAction:
                if (UnityEngine.Random.Range(1, 101) <= 30)
                {
                    Unit attackingUnit = grenadeAction.GetUnit();
                    GridPosition targetGridPosition = grenadeAction.GetTargetGridPosition();
                    PositionActionCamera(attackingUnit, targetGridPosition);
                    ShowActionCamera();
                }
                break;
        }
    }

    private void PositionActionCamera(Unit attackingUnit, Unit targetUnit)
    {
        PositionActionCamera(attackingUnit, targetUnit.GetWorldPosition());
    }


    private void PositionActionCamera(Unit attackingUnit, GridPosition targetGridPosition)
    {
        PositionActionCamera(attackingUnit, LevelGrid.Instance.GetWorldPosition(targetGridPosition));
    }

    private void PositionActionCamera(Unit attackingUnit, Vector3 targetWorldPosition)
    {
        if (actionCameraGameObject == null) return;
        Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
        Vector3 attackDirection = (targetWorldPosition - attackingUnit.GetWorldPosition()).normalized;
        float shoulderOffsetAmount = 0.5f;
        Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * attackDirection * shoulderOffsetAmount;
        Vector3 actionCameraPosition =
            attackingUnit.GetWorldPosition() +
            cameraCharacterHeight +
            shoulderOffset +
            (attackDirection * -1);
        actionCameraGameObject.transform.position = actionCameraPosition;
        actionCameraGameObject.transform.LookAt(targetWorldPosition + cameraCharacterHeight);
        ShowGameUI(false);
    }

    private void ShowGameUI(bool show = true)
    {
        if (overlayUICanvas == null) return;
        overlayUICanvas.SetActive(show);
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case AttackAction:
                ShowGameUI();
                ShowActionCamera(false);
                break;
            case MeleeAction:
                ShowGameUI();
                ShowActionCamera(false);
                break;
            case GrenadeAction:
                ShowGameUI();
                ShowActionCamera(false);
                break;
            default:
                ShowGameUI();
                ShowActionCamera(false);
                break;
        }
    }
}
