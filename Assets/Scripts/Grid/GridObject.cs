using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{

    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private List<Unit> unitList;
    private IInteractable interactable;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        unitList = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public List<Unit> GetUnitList() => unitList;

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in unitList)
        {
            unitString += $"{unit} \n";
        }
        return unitString += $"- {gridPosition}";

    }

    public bool HasAnyUnit() => GetUnitList().Count > 0;

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return GetUnitList()[0];
        }
        return null;
    }
    public void SetInteractable(IInteractable interactable) => this.interactable = interactable;

    public IInteractable GetInteractable() => interactable;
}
