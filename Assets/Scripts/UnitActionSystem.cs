using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitsLayerMask;

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
    /// <summary>
    /// On click, handle unit selection or movement
    /// On key up, check for movement speed modifications
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (TryHandleUnitSelection()) return;
            selectedUnit.Move(MouseWorld.GetPosition());
        }
        HandleMovementSpeedInteractions();
    }

    /// <summary>
    /// Attempt to select an unit in the units layer mask of the scene. Cast a ray from the mouse position to the game world and in
    /// case of a hit on a component retrieve that Unit component and set it as a the selected unit for this instance.
    /// </summary>
    /// <returns>True if an Unit component was hit; False otherwise</returns>
    private bool TryHandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitsLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
            {
                SetSelectedUnit(unit);
                return true;
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
            selectedUnit.SetMovementBuff(.9f);
        }

        // Speed up player (10%)
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            selectedUnit.SetMovementBuff(1.1f);
        }

        // Reset the speed of the player to base speed
        if (Input.GetKeyUp(KeyCode.R))
        {
            selectedUnit.SetMovementSpeed(selectedUnit.baseMovementSpeed);
        }
    }
    /// <summary>
    /// Change this Instance <c>Unit</c> and emit an event notifying of the change
    /// </summary>
    /// <param name="unit"></param>
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
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
}
