using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    public override string GetActionName()
    {
        return "interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction(
            gridPosition,
            0
        );
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
                IInteractable interactable = levelGrid.GetInteractableAtGridPosition(testGridPosition);
                if (interactable == null) continue; // No interactable on grid position

                validGridPositionList.Add(testGridPosition); // Add the grid position of the enemy
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractionComplete);
        ActionStart(onActionComplete);
    }


    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
    }

    private void OnInteractionComplete()
    {
        ActionComplete();
    }
}
