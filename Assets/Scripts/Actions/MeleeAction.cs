using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    public class OnMeleeEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit attackingUnit;
        public int damageDealt;
        public bool triggerActionCamera;
    }
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private float rotateSpeed = 10f;
    public static event EventHandler<OnMeleeEventArgs> OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
                break;
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
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = .5f;
                stateTimer = afterHitStateTime;
                int damageDealt = UnityEngine.Random.Range(80, 100);
                float critChance = 5f;
                bool triggerActionCamera = false;
                if (UnityEngine.Random.Range(0, 101) <= critChance)
                {
                    triggerActionCamera = true;
                    damageDealt = Mathf.RoundToInt(damageDealt * 1.20f);
                }
                if (damageDealt >= 90)
                {
                    triggerActionCamera = true;
                }
                targetUnit.Damage(damageDealt);
                OnMeleeEventArgs eventArgs = new OnMeleeEventArgs
                {
                    targetUnit = targetUnit,
                    attackingUnit = unit,
                    damageDealt = damageDealt,
                    triggerActionCamera = triggerActionCamera
                };
                OnAnySwordHit?.Invoke(this, eventArgs);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;

        }
    }

    public override string GetActionName()
    {
        return "melee";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(gridPosition, 200);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new();
        LevelGrid levelGrid = LevelGrid.Instance;
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -actionRange; x <= actionRange; x++)
        {
            for (int z = -actionRange; z <= actionRange; z++)
            {
                GridPosition offsetGridPosition = new(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!levelGrid.IsValidGridPosition(testGridPosition)) { continue; }// Not a valid grid position
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

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit() => targetUnit;
}
