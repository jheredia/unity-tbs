using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsCostText;

    private List<ActionButtonUI> actionButtonUIs;

    private void Awake()
    {
        actionButtonUIs = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;



        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPointsCost();

    }
    private void CreateUnitActionButtons()
    {
        ClearUnitActionButtons();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) return;
        BaseAction[] selectedUnitActions = selectedUnit.GetUnitActions();
        foreach (BaseAction action in selectedUnitActions)
        {
            Transform actionButton = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButton.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);
            actionButtonUIs.Add(actionButtonUI);
        }
    }

    private void ClearUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIs.Clear();
    }

    /// <summary>
    /// Listens to unit selection change on the class <c>UnitActionSystem</c>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="empty"></param>
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs empty)
    {
        UpdateSelectedVisual();
        UpdateActionPointsCost();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIs)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPointsCost()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        actionPointsCostText.text = (selectedBaseAction != null) ? $"Action Cost: {selectedBaseAction.GetActionPointsCost()}" : "";
    }

}
