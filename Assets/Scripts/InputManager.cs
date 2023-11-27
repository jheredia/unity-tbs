using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple instances of {GetType().Name} present {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();
    }

    public Vector2 GetMouseScreenPosition()
    {

        return Mouse.current.position.ReadValue();
    }

    public bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }

    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    public Vector2 GetCameraMoveVector() => playerInputActions.Player.CameraMovement.ReadValue<Vector2>();


    public bool IsLeftMouseButtonDownThisFrame() => playerInputActions.Player.LeftClick.WasPressedThisFrame();

    public bool IsRightMouseButtonDownThisFrame() => playerInputActions.Player.RightClick.WasPressedThisFrame();

    public bool GetMouseButtonUp(int button)
    {
        return Input.GetMouseButtonUp(button);
    }

    public float GetCameraRotationAmount() => playerInputActions.Player.CameraRotation.ReadValue<float>();

    public float GetCameraZoomAmount() => playerInputActions.Player.CameraZoom.ReadValue<float>();

}
