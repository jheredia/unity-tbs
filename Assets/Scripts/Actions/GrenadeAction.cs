using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    private int maxThrowDistance = 7;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private Transform grenadeProjectilePrefab;

    public override string GetActionName()
    {
        return "grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(gridPosition, 0);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new();
        LevelGrid levelGrid = LevelGrid.Instance;
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!levelGrid.IsValidGridPosition(testGridPosition)) { continue; }// Not a valid grid position
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Get the radius
                if (testDistance > maxThrowDistance) { continue; }// Outside of attack rangei


                validGridPositionList.Add(testGridPosition); // Add the grid position of the enemy
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        grenadeProjectileTransform.GetComponent<GrenadeProjectile>().Setup(gridPosition, OnGrenadeBehaviourComplete);
        ActionStart(onActionComplete);
    }

    private void Update()
    {
        if (!isActive) return;

    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }
}
