using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        CreateUnitActionButtons();
    }
    private void CreateUnitActionButtons()
    {
        ClearUnitActionButtons();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction[] selectedUnitActions = selectedUnit.GetUnitActions();
        foreach (BaseAction action in selectedUnitActions)
        {
            Transform actionButton = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButton.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);
        }
    }

    private void ClearUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
    }

    /// <summary>
    /// Listens to unit selection change on the class <c>UnitActionSystem</c>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="empty"></param>
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        CreateUnitActionButtons();
    }
}
