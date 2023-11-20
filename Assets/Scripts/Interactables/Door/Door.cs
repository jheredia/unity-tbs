using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private enum State
    {
        Open,
        Opening,
        Closed,
        Closing,
        Destroyed,
        Jammed
    }

    [SerializeField] bool isOpen;
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
        if (isOpen) Open();
        else Close();
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
        if (isOpen) Close();
        else Open();
        onInteractionComplete();
    }

    public void Open()
    {
        isOpen = true;
        animator.SetBool(IS_OPEN_PARAMETER, isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }

    public void Close()
    {
        isOpen = false;
        animator.SetBool(IS_OPEN_PARAMETER, isOpen);
        // Update the pathfinding script to indicate that this position can't be used
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }
}
