using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Action onActionComplete;
    protected Unit unit;
    protected bool isActive;

    const int actionPointsCost = 1;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
    }

    protected virtual void ActionComplete()
    {
        isActive = false;
        onActionComplete();
    }
}
