using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    public static event EventHandler<OnDoorInteractedEventArgs> OnAnyDoorOpened;
    public event EventHandler OnDoorOpened;

    public event EventHandler OnDoorClosed;
    public static event EventHandler<OnDoorInteractedEventArgs> OnAnyDoorClosed;

    private enum State
    {
        Open,
        Opening,
        Closed,
        Closing,
        Destroyed,
        Jammed
    }

    public class OnDoorInteractedEventArgs : EventArgs
    {
        public List<Sector> connectedSectors;
    }

    [SerializeField] bool isOpen;
    [SerializeField] bool isLocked;

    [SerializeField] Light innerLight;
    [SerializeField] Light outerLight;

    [Header("Connecting sectors")]
    [SerializeField] Sector sector1;
    [SerializeField] Sector sector2;

    private GridPosition gridPosition;
    private Animator animator;
    private const string IS_OPEN_PARAMETER = "IsOpen";
    private Action onInteractionComplete;
    private float timer;
    private bool isActive;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        if (isOpen)
            Open();
        else
            Close();
        if (isLocked)
            ChangeLights(Color.red);
        else
            ChangeLights(Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = .5f;
        if (isLocked)
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            selectedUnit.SetKeys(selectedUnit.GetKeys() - 1);
            Unlock();
            ChangeLights(Color.green);
        }
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {

        isOpen = true;
        animator.SetBool(IS_OPEN_PARAMETER, isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        OnDoorInteractedEventArgs eventArgs = new OnDoorInteractedEventArgs
        {
            connectedSectors = new List<Sector> { sector1, sector2 }
        };
        OnAnyDoorOpened?.Invoke(this, eventArgs);
        OnDoorOpened?.Invoke(this, eventArgs);
    }

    public void Close()
    {
        isOpen = false;
        animator.SetBool(IS_OPEN_PARAMETER, isOpen);
        // Update the pathfinding script to indicate that this position can't be used
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
        OnDoorInteractedEventArgs eventArgs = new OnDoorInteractedEventArgs
        {
            connectedSectors = new List<Sector> { sector1, sector2 }
        };
        OnAnyDoorClosed?.Invoke(this, eventArgs);
        OnDoorClosed?.Invoke(this, eventArgs);
    }

    private void ChangeLights(Color color)
    {
        innerLight.color = color;
        outerLight.color = color;
    }

    private void Lock()
    {
        isLocked = true;
    }

    private void Unlock()
    {
        isLocked = false;
    }
}
