using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{

    const string IS_WALKING_PARAM = "IsWalking";

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float baseMovementSpeed = 4f;
    [SerializeField] readonly float StoppingDelta = .1f;
    [SerializeField] private Animator unitAnimator;
    [SerializeField] readonly float rotateSpeed = 10f;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;
    private Unit unit;

    public float GetBaseMovementSpeed() => baseMovementSpeed;

    void Awake()
    {
        unit = GetComponent<Unit>();
        targetPosition = transform.position;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
    }

    /// <summary>
    /// Calculate the distance between the taget position set in the Move function and the current position, 
    /// if that distance is greater or equal than our stopping delta, continue moving, else stop the animation.
    /// Rotate the character to look for the target position.
    /// </summary>
    private void CheckMovement()
    {
        if (Vector3.Distance(targetPosition, transform.position) >= StoppingDelta)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * movementSpeed * Time.deltaTime;

            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            unitAnimator.SetBool(IS_WALKING_PARAM, true);
        }
        else
        {
            unitAnimator.SetBool(IS_WALKING_PARAM, false);
        }
    }

    // Moves the Unit from its position to the target position represented by a three dimensions vector
    public void Move(GridPosition targetPosition)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(targetPosition);
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

    public bool IsValidActionGridPosition(GridPosition gridPosition) => GetValidActionGridPositionList().Contains(gridPosition);

    public List<GridPosition> GetValidActionGridPositionList()
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
