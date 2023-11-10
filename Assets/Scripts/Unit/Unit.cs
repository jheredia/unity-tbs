using System;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = 6;
    private int maxActionPoints;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDied;

    [SerializeField] private bool isEnemy;
    [SerializeField] private Transform actionCameraViewpoint;
    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        maxActionPoints = actionPoints;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDeath += HealthSystem_OnDeath;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }


    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public GridPosition GetGridPosition() => gridPosition;

    public Vector3 GetWorldPosition() => transform.position;

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
        if (CanResetActionPoints())
            actionPoints = maxActionPoints;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool CanResetActionPoints()
    {
        bool isPlayerTurn = TurnSystem.Instance.IsPlayerTurn();
        if ((IsEnemy() && !isPlayerTurn) || (!IsEnemy() && isPlayerTurn)) return true;
        return false;
    }

    public bool IsEnemy() => isEnemy;

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public void Heal(int healingAmount)
    {
        healthSystem.Heal(healingAmount);
    }

    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        Destroy(gameObject);
        LevelGrid.Instance.RemoveUnitAtGridPosition(GetGridPosition(), this);
        OnAnyUnitDied?.Invoke(this, EventArgs.Empty);
    }

    public float GetActionPointsNormalized()
    {
        return (float)actionPoints / maxActionPoints;
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

    public Transform GetActionCameraViewpoint() => actionCameraViewpoint;

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T t) return t;
        }
        return null;
    }
}
