using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;

    private float timer;

    [SerializeField] private float enemyTimer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn)) state = State.Busy; // If an enemy can take an action, change state
                    else TurnSystem.Instance.NextTurn(); // No enemy can take an action
                }
                break;
            case State.Busy:
                break;
        }

    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = enemyTimer;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) return true;
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        SpinAction spinAction = enemyUnit.GetSpinAction();
        GridPosition enemyGridPosition = enemyUnit.GetGridPosition();
        if (!spinAction.IsValidActionGridPosition(enemyGridPosition)) return false;
        if (!enemyUnit.TrySpendActionPointsOnAction(spinAction)) return false;
        spinAction.TakeAction(enemyGridPosition, onEnemyAIActionComplete);
        return true;
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
}
