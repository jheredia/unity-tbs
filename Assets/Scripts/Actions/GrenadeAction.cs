using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    private enum State
    {
        Aiming,
        Throwing,
        Cooloff,
    }
    private State state;
    private float stateTimer;

    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private Transform grenadeSpawnPoint;
    public static event EventHandler OnAnyGrenadeLaunched;
    public event EventHandler OnGrenadeLaunchStarted;
    public event EventHandler OnGrenadeLaunchCompleted;
    private float rotateSpeed = 10f;
    private GridPosition targetPosition;


    public override string GetActionName()
    {
        return "grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {

        int totalActionWeight = 0;
        if (GetAvailableCharges() != 0)
        {
            Vector3 targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit.IsEnemy())
                    {
                        totalActionWeight -= 25;
                    }
                    if (!unit.IsEnemy())
                    {
                        totalActionWeight += 25;
                    }
                }
            }
        }
        return new EnemyAIAction(gridPosition, totalActionWeight);
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
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!levelGrid.IsValidGridPosition(testGridPosition)) { continue; }// Not a valid grid position
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Get the radius
                if (testDistance > actionRange) { continue; }// Outside of attack rangei
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDirection = (LevelGrid.Instance.GetWorldPosition(testGridPosition) - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDirection,
                    Vector3.Distance(unitWorldPosition, LevelGrid.Instance.GetWorldPosition(testGridPosition)),
                    obstaclesLayerMask
                )) continue; // Blocked by an obstacle

                validGridPositionList.Add(testGridPosition); // Add the grid position of the enemy
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        state = State.Aiming;
        float aimingStateTime = 1.75f;
        stateTimer = aimingStateTime;
        targetPosition = gridPosition;

        OnGrenadeLaunchStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    private void Update()
    {
        if (!isActive) return;


        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (LevelGrid.Instance.GetWorldPosition(targetPosition) - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
        }
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void OnGrenadeBehaviourComplete()
    {
        SetAvailableCharges(GetAvailableCharges() - 1);
        ActionComplete();
    }
    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Throwing;
                float throwingStateTime = .1f;
                stateTimer = throwingStateTime;
                break;
            case State.Throwing:
                Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, grenadeSpawnPoint.position, Quaternion.identity);
                OnGrenadeLaunchCompleted?.Invoke(this, EventArgs.Empty);
                grenadeProjectileTransform.GetComponent<GrenadeProjectile>().Setup(targetPosition, OnGrenadeBehaviourComplete);
                OnAnyGrenadeLaunched?.Invoke(this, EventArgs.Empty);
                state = State.Cooloff;
                float cooloffStateTime = .1f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                break;
        }
    }

    public GridPosition GetTargetGridPosition() => targetPosition;
}
