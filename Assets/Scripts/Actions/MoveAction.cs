using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    public event EventHandler<OnChangeFloorsStartedEventArgs> OnChangeFloorsStarted;

    public class OnChangeFloorsStartedEventArgs : EventArgs
    {
        public GridPosition unitGridPosition;
        public GridPosition targetGridPosition;
    }
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float baseMovementSpeed = 4f;
    [SerializeField] readonly float stoppingDelta = .1f;
    [SerializeField] readonly float rotateSpeed = 10f;
    const int actionPointsCost = 1;

    private List<Vector3> positionList;
    int currentPositionIndex;
    public float GetBaseMovementSpeed() => baseMovementSpeed;

    public override string GetActionName()
    {
        return "Move";
    }

    private bool isChangingFloors;
    private float differentFloorsTeleportTimer;
    private float differentFloorsTeleportTimerMax = .5f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        if (isChangingFloors)
        {
            Vector3 targetPosition = positionList[currentPositionIndex];
            Vector3 lookDirection = (targetPosition - transform.position).normalized;
            lookDirection.Scale(new Vector3(1, 0, 1));
            transform.forward = Vector3.Slerp(transform.forward, lookDirection, Time.deltaTime * rotateSpeed);
            differentFloorsTeleportTimer -= Time.deltaTime;
            if (differentFloorsTeleportTimer < 0f)
            {
                isChangingFloors = false;
                transform.position = targetPosition;
            }
        }
        else
        {
            // Regular movement
            CheckMovement();
        }
    }

    /// <summary>
    /// Calculate the distance between the taget position set in the Move function and the current position, 
    /// if that distance is greater or equal than our stopping delta, continue moving, else stop the animation.
    /// Rotate the character to look for the target position.
    /// </summary>
    private void CheckMovement()
    {
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
        if (Vector3.Distance(targetPosition, transform.position) <= stoppingDelta)
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            else
            {
                targetPosition = positionList[currentPositionIndex];
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
                if (unitGridPosition.floor != targetGridPosition.floor)
                {
                    isChangingFloors = true;
                    differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;
                    OnChangeFloorsStarted.Invoke(this, new OnChangeFloorsStartedEventArgs
                    {
                        unitGridPosition = unitGridPosition,
                        targetGridPosition = targetGridPosition
                    });
                }
            }
        }
    }

    // Moves the Unit from its position to the target position represented by a three dimensions vector
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    /// <summary>
    /// Change the current movement speed of the Unit
    /// </summary>
    /// <param name="newSpeed">New movement speed for this Unit</param>
    public void SetMovementSpeed(float newSpeed)
    {
        this.movementSpeed = newSpeed;
    }

    /// <summary>
    /// Increase or decrease movement speed of the unit by <c>buffPercentage</c> percent.
    /// </summary>
    /// <param name="buffPercentage">Positive or negative to be applied on the movement speed of the unit</param>
    public void SetMovementBuff(float buffPercentage)
    {
        this.movementSpeed = movementSpeed * buffPercentage;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        LevelGrid levelGrid = LevelGrid.Instance;
        for (int x = -actionRange; x <= actionRange; x++)
        {
            for (int z = -actionRange; z <= actionRange; z++)
            {
                for (int floor = -actionRange; floor <= actionRange; floor++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!levelGrid.IsValidGridPosition(testGridPosition)) continue;
                    if (testGridPosition == unitGridPosition) continue;
                    if (levelGrid.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;
                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) continue;
                    int pathfindingDistanceMultiplier = 10;
                    if (Pathfinding.Instance.GetPathLength(
                        unitGridPosition, testGridPosition) > actionRange * pathfindingDistanceMultiplier
                    ) continue;
                    validGridPositionList.Add(testGridPosition);
                }
            }
        }
        return validGridPositionList;
    }

    public override int GetActionPointsCost()
    {
        return actionPointsCost;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<AttackAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction(gridPosition, targetCountAtGridPosition * 10);
    }
}
