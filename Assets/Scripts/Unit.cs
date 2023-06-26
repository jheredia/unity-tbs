using UnityEngine;

public class Unit : MonoBehaviour
{
    const string IS_WALKING_PARAM = "IsWalking";
    [SerializeField] private Animator unitAnimator;
    [SerializeField] float StoppingDelta = .1f;
    private Vector3 targetPosition;
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] public float baseMovementSpeed = 4f;

    void Awake()
    {
        targetPosition = transform.position;
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

    private void CheckMovement()
    {
        // Calculate the distance between the taget position set in the Move function and the current position, 
        // if that distance is greater or equal than our stopping delta, continue moving, else stop the animation.
        if (Vector3.Distance(targetPosition, transform.position) >= StoppingDelta)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * movementSpeed * Time.deltaTime;

            // Smoothly rotate the model to the target position
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            unitAnimator.SetBool(IS_WALKING_PARAM, true);
        }
        else
        {
            unitAnimator.SetBool(IS_WALKING_PARAM, false);
        }
    }

    public void SetMovementSpeed(float newSpeed)
    {
        this.movementSpeed = newSpeed;
    }

    // Increase or decrease movement speed by a percentage.
    public void SetMovementBuff(float buffPercentage)
    {
        this.movementSpeed = this.movementSpeed * buffPercentage;
    }

}
