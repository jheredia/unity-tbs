using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    private static UnitActionSystem instance;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitsLayerMask;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (TryHandleUnitSelection()) return;
            selectedUnit.Move(MouseWorld.GetPosition());
        }
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

    private bool TryHandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.unitsLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
            {
                instance.selectedUnit = unit;
                return true;
            }
        }
        return false;
    }

}
