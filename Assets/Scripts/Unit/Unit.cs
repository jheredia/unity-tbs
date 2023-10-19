using System;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private GridPosition gridPosition;
    private MoveAction moveAction;

    private SpinAction spinAction;

    private CrouchAction crouchAction;
    private BaseAction[] baseActionArray;
    private const int STARTING_ACTION_POINTS = 4;
    private int actionPoints = STARTING_ACTION_POINTS;

    public static event EventHandler OnAnyActionPointsChanged;
    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        crouchAction = GetComponent<CrouchAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }


    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }


    public MoveAction GetMoveAction() => moveAction;

    public SpinAction GetSpinAction() => spinAction;

    public CrouchAction GetCrouchAction() => crouchAction;

    public GridPosition GetGridPosition() => gridPosition;

    public BaseAction[] GetUnitActions() => baseActionArray;

    public int GetActionPoints() => actionPoints;

    public bool TrySpendActionPointsOnAction(BaseAction action)
    {
        if (CanSpendActionPoints(action))
        {
            SpendActionPoints(action.GetActionPointsCost());
            return true;
        }
        return false;
    }

    public bool CanSpendActionPoints(BaseAction action) => this.GetActionPoints() >= action.GetActionPointsCost();

    private void SpendActionPoints(int amount)
    {
        if (GetActionPoints() - amount >= 0)
        {
            actionPoints -= amount;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty)
    {
        ResetActionPoints();
    }

    private void ResetActionPoints()
    {
        actionPoints = STARTING_ACTION_POINTS;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
}
