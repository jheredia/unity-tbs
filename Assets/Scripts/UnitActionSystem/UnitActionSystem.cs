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
            Debug.LogError($"Multiple instances of UnitActionSystem present {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
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

        HandleSelectedAction();
        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        if (Input.GetKeyUp(KeyCode.C))
        {
            CrouchAction unitCrouchAction = selectedUnit.GetCrouchAction();
            SetBusy();
            unitCrouchAction.TakeAction(mouseGridPosition, ClearBusy);
        }
        HandleMovementSpeedInteractions();
    }

    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = MouseWorld.GetPosition();
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);
            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitsLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit) && unit != selectedUnit)
                {
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Check whether the arrow up, arrow down or r key were pressed and change movement speed values accordingly
    /// </summary>
    private void HandleMovementSpeedInteractions()
    {
        //Slow player (10%)
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            selectedUnit.GetMoveAction().SetMovementBuff(.9f);
        }

        // Speed up player (10%)
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            selectedUnit.GetMoveAction().SetMovementBuff(1.1f);
        }

        // Reset the speed of the player to base speed
        if (Input.GetKeyUp(KeyCode.R))
        {
            selectedUnit.GetMoveAction().SetMovementSpeed(selectedUnit.GetMoveAction().GetBaseMovementSpeed());
        }
    }
    /// <summary>
    /// Change this Instance <c>Unit</c> and emit an event notifying of the change
    /// </summary>
    /// <param name="unit"></param>
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
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
        this.selectedAction = action;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    public BaseAction GetSelectedAction() => this.selectedAction;

    public bool GetIsBusy() => this.isBusy;
}
