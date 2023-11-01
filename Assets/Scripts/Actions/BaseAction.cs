using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
Should be refactored so that each action has its own range, color and bool to check if it should be shown
*/
public abstract class BaseAction : MonoBehaviour
{
    protected Action onActionComplete;
    protected Unit unit;
    protected bool isActive;

    // [SerializeField, Min(0)] private int actionRange = 4;
    // [SerializeField] private Color color = Color.white;
    // [SerializeField] private bool showRange;

    [SerializeField] GridSystemVisual.GridVisualType gridVisualType;
    [SerializeField] GridSystemVisual.GridVisualType rangeGridVisualType;
    [SerializeField] private int actionPointsCost = 1;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;


    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) => GetValidActionGridPositionList().Contains(gridPosition);

    public abstract List<GridPosition> GetValidActionGridPositionList();

    /// <summary>
    ///     Returns the action points cost for a base action, should be overriden by each specific action
    ///     indicating their corresponding cost
    /// </summary>
    /// <returns>The action points cost for this action</returns>
    public virtual int GetActionPointsCost() => actionPointsCost;

    protected virtual void ActionStart(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void ActionComplete()
    {
        isActive = false;
        onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit() => unit;

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }
        if (enemyAIActionList.Count == 0) return null;
        enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.GetActionValue() - a.GetActionValue());
        return enemyAIActionList[0];
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
