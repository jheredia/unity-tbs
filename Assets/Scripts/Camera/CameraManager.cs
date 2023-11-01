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
                Vector3 actionCameraViewpointLocalPosition = attackingUnit.GetActionCameraViewpoint().localPosition;
                actionCameraGameObject.transform.position = actionCameraViewpointLocalPosition;
                Vector3 cameraCharacterHeight = Vector3.up * actionCameraViewpointLocalPosition.y;
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
