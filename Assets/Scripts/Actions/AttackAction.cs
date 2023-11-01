using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
        Reloading
    }

    private State state;
    private float stateTimer;

    private int maxAttackRange = 7;
    private Unit targetUnit;
    private bool canShootBullet;
    private float rotateSpeed = 10f;

    public event EventHandler<OnAttackEventArgs> OnAttack;

    public class OnAttackEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit attackingUnit;
        public int damageDealt;
    }

    public override string GetActionName()
    {
        return "attack";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new();
        LevelGrid levelGrid = LevelGrid.Instance;
        for (int x = -maxAttackRange; x <= maxAttackRange; x++)
        {
            for (int z = -maxAttackRange; z <= maxAttackRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!levelGrid.IsValidGridPosition(testGridPosition)) { continue; }// Not a valid grid position
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Get the radius
                if (testDistance > maxAttackRange) { continue; }// Outside of attack range
                // validGridPositionList.Add(testGridPosition);
                if (!levelGrid.HasAnyUnitOnGridPosition(testGridPosition)) { continue; }// No units in that position
                Unit targetUnit = levelGrid.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy()) continue;
                validGridPositionList.Add(testGridPosition); // Add the grid position of the enemy
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        //Debug.Log(targetUnit);
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;
        canShootBullet = true;
        ActionStart(onActionComplete);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;


        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff: break;
            case State.Reloading: break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = .1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = .5f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                state = State.Reloading;
                float reloadingStateTime = .3f;
                stateTimer = reloadingStateTime;
                break;
            case State.Reloading:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        int damageDealt = UnityEngine.Random.Range(30, 51);
        OnAttack?.Invoke(this, new OnAttackEventArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit,
            damageDealt = damageDealt,
        });
        targetUnit.Damage(damageDealt);
    }

    public Unit GetTargetUnit() => targetUnit;

    public int GetMaxAttackRange() => maxAttackRange;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        Debug.Log($"Attack value {100 + Mathf.RoundToInt(1 - targetUnit.GetHealthNormalized() * 100f)}");
        return new EnemyAIAction(gridPosition, 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f));
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
