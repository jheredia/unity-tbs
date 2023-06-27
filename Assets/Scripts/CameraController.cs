using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Consts related to camera movement options
    /// </summary>
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float movementUnits = 1f;

    /// <summary>
    /// Consts related to camera rotation options
    /// </summary>
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float rotationUnits = 1f;

    /// <summary>
    /// Consts related to zoom options
    /// </summary>
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFST = 12f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    /// <summary>
    /// Move camera around game space using W, A, S, D.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 inputMoveDirection = new Vector3(0, 0, 0);

        // Move on Z axis
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.z += movementUnits;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.z -= movementUnits;
        }

        // Move on X axis
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x -= movementUnits;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x += movementUnits;
        }
        Vector3 moveVector = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
        transform.position += moveVector * movementSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Rotate along Y axis using Q and E keys
    /// </summary>
    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y += rotationUnits;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y -= rotationUnits;
        }
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;

    }

    /// <summary>
    /// Zoom in and out using mouse wheel scrolling
    /// </summary>
    private void HandleZoom()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            targetFollowOffset.y -= zoomAmount;
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            targetFollowOffset.y += zoomAmount;
        }

        if (Input.mouseScrollDelta.y > 0 && targetFollowOffset.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFST);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime); ;
    }


    // Not useful now, maybe check later if it's something cool
    // public void SetPosition(Vector3 newPosition)
    // {
    //     this.transform.position = newPosition;//Vector3.Lerp(this.transform.position, newPosition, movementSpeed * Time.deltaTime);
    // }
}

