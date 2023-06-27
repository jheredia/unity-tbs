using UnityEngine;

public class Unit : MonoBehaviour
{
    const string IS_WALKING_PARAM = "IsWalking";
    [SerializeField] private Animator unitAnimator;
    [SerializeField] float StoppingDelta = .1f;
    private Vector3 targetPosition;
    private GridPosition gridPosition;
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] public float baseMovementSpeed = 4f;

    void Awake()
    {
        targetPosition = transform.position;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }
    // Update is called once per frame
    void Update()
    {
        CheckMovement();
    }

    // Moves the Unit from its position to the target position represented by a three dimensions vector
    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
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
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }

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
        this.movementSpeed = this.movementSpeed * buffPercentage;
    }

}
