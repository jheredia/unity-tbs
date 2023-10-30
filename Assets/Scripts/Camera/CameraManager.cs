using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private GameObject overlayUICanvas;
    // Start is called before the first frame update
    void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        HideActionCamera();
    }


    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case AttackAction attackAction:
                Unit attackingUnit = attackAction.GetUnit();
                Unit targetUnit = attackAction.GetTargetUnit();
                // Transform actionCameraViewPointTransform = attackingUnit.GetActionCameraViewpoint();
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                Vector3 attackDirection = (targetUnit.GetWorldPosition() - attackingUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * attackDirection * shoulderOffsetAmount;

                Vector3 actionCameraPosition = attackingUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (attackDirection * -1);

                // actionCameraGameObject.transform.position = actionCameraViewPointTransform.right + actionCameraViewPointTransform.up + actionCameraViewPointTransform.forward;

                // Debug.Log(actionCameraViewPointTransform.right + actionCameraViewPointTransform.up + actionCameraViewPointTransform.forward);
                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                overlayUICanvas.SetActive(false);
                ShowActionCamera();
                break;

        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case AttackAction attackAction:
                overlayUICanvas.SetActive(true);
                HideActionCamera();
                break;

        }
    }
}
