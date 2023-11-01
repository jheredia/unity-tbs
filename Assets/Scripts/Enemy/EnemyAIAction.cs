using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAction
{
    private GridPosition gridPosition;
    private int actionValue;

    public EnemyAIAction(GridPosition gridPosition, int actionValue)
    {
        this.gridPosition = gridPosition;
        this.actionValue = actionValue;
    }
    public GridPosition GetGridPosition() => gridPosition;

    public int GetActionValue() => actionValue;
}
