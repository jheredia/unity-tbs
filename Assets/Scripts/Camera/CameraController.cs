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

    /// <summary>
    /// Consts related to camera rotation options
    /// </summary>
    [SerializeField] private float rotationSpeed = 100f;

    /// <summary>
    /// Consts related to zoom options
    /// </summary>
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFST = 12f;
    [SerializeField] private float zoomSpeed = 5f;
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
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector();
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
        transform.position += moveVector * movementSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Rotate along Y axis using Q and E keys
    /// </summary>
    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0)
        {
            y = InputManager.Instance.GetCameraRotationAmount()
        };
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;

    }

    /// <summary>
    /// Zoom in and out using mouse wheel scrolling
    /// </summary>
    private void HandleZoom()
    {
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFST);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime); ;
    }


    // Not useful now, maybe check later if it's something cool
    // public void SetPosition(Vector3 newPosition)
    // {
    //     this.transform.position = newPosition;//Vector3.Lerp(this.transform.position, newPosition, movementSpeed * Time.deltaTime);
    // }
}

