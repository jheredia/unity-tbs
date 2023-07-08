using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{

    private GridPosition gridPosition;
    private GridSystem gridSystem;
    private List<Unit> unitList;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        unitList = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public List<Unit> GetUnitList()
    {
        return this.unitList;
    }

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
        return unitString += $"- {gridPosition.ToString()}";

    }

    public bool HasAnyUnit() => this.GetUnitList().Count > 0;
}
