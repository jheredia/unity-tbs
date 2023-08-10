using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    const string IS_WALKING_PARAM = "IsWalking";

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float baseMovementSpeed = 4f;
    [SerializeField] readonly float StoppingDelta = .1f;
    [SerializeField] private Animator unitAnimator;
    [SerializeField] readonly float rotateSpeed = 10f;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;
    public float GetBaseMovementSpeed() => baseMovementSpeed;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    public override string GetActionName()
    {
        return "Move";
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isActive) CheckMovement();
    }

    /// <summary>
    /// Calculate the distance between the taget position set in the Move function and the current position, 
    /// if that distance is greater or equal than our stopping delta, continue moving, else stop the animation.
    /// Rotate the character to look for the target position.
    /// </summary>
    private void CheckMovement()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        if (Vector3.Distance(targetPosition, transform.position) >= StoppingDelta)
        {
            transform.position += moveDirection * movementSpeed * Time.deltaTime;
            unitAnimator.SetBool(IS_WALKING_PARAM, true);
        }
        else
        {
            unitAnimator.SetBool(IS_WALKING_PARAM, false);
            isActive = false;
            onActionComplete();
        }
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    // Moves the Unit from its position to the target position represented by a three dimensions vector
    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(targetPosition);
        isActive = true;
        this.onActionComplete = onActionComplete;
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
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!levelGrid.IsValidGridPosition(testGridPosition)) continue;
                if (testGridPosition == unitGridPosition) continue;
                if (levelGrid.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

}
