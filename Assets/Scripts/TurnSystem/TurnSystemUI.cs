using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;

    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private TextMeshProUGUI enemyTurnVisual;


    // Start is called before the first frame update
    void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });
        UpdateTurnVisuals();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty)
    {
        UpdateTurnVisuals();
    }

    private void UpdateTurnNumberText()
    {
        turnNumberText.text = $"Turn: {TurnSystem.Instance.GetTurnNumber()}";
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisual.text = $"{(TurnSystem.Instance.IsPlayerTurn() ? "Player" : "Enemy")} Turn";
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateTurnVisuals()
    {
        UpdateTurnNumberText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }
}

