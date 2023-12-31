using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;

    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitsLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple instances of {GetType().Name} present {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
        SetSelectedUnit(selectedUnit);
    }

    private void Unit_OnAnyUnitDied(object sender, EventArgs e)
    {
        // The selected unit died
        if (selectedUnit == sender as Unit)
        {
            SetSelectedUnit(null);
            SetSelectedAction(null);
        }
    }

    /// <summary>
    /// On click, handle unit selection or movement
    /// On key up, check for movement speed modifications
    /// </summary>
    private void Update()
    {
        if (isBusy) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (TryHandleUnitSelection()) return;
        if (!TurnSystem.Instance.IsPlayerTurn()) return;
        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsLeftMouseButtonDownThisFrame())
        {
            Vector3 mousePosition = MouseWorld.GetPositionOnlyHitVisible();
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);
            // If it's not a valid grid position
            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
            // If this action uses charges and doesn't have any available
            if (!selectedAction.HasChargesAvailable()) return;
            // If selected unit doesn't have enough action points
            if (!selectedUnit.TrySpendActionPointsOnAction(selectedAction)) return;
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    /// <summary>
    /// Attempt to select an unit in the units layer mask of the scene. Cast a ray from the mouse position to the game world and in
    /// case of a hit on a component retrieve that Unit component and set it as a the selected unit for this instance.
    /// </summary>
    /// <returns>True if an Unit component was hit and it's not the same one; False otherwise</returns>
    private bool TryHandleUnitSelection()
    {
        InputManager inputManagerInstance = InputManager.Instance;
        if (inputManagerInstance.IsLeftMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(inputManagerInstance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitsLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out Unit unit) && unit != selectedUnit)
                {
                    if (unit == selectedUnit || unit.IsEnemy()) return false;
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Change this Instance <c>Unit</c> and emit an event notifying of the change
    /// </summary>
    /// <param name="unit"></param>
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        if (unit != null)
            SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Return the selected unit for this instance
    /// </summary>
    /// <returns>The selected unit on this instance</returns>
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    public BaseAction GetSelectedAction() => this.selectedAction;

    public bool GetIsBusy() => this.isBusy;
}
