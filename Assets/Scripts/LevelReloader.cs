using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelReloader : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Button endGameButton;
    [SerializeField] private Button restartGameButton;
    [SerializeField] private TextMeshProUGUI endGameMessage;


    // Start is called before the first frame update
    void Start()
    {
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
        endGameButton.onClick.AddListener(EndGame);
        restartGameButton.onClick.AddListener(RestartGame);
    }

    private void Unit_OnAnyUnitDied(object sender, EventArgs e)
    {
        if (UnitManager.Instance.GetFriendlyUnitList().Count == 0)
        {
            ShowEndScreen(true);
            endGameMessage.text = "You lose";
        }
        if (UnitManager.Instance.GetEnemyUnitList().Count == 0)
        {
            ShowEndScreen(true);
            endGameMessage.text = "You win!";
        }
    }

    private void EndGame()
    {
        ShowEndScreen(false);
    }
    private void RestartGame()
    {
        ShowEndScreen(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ShowEndScreen(bool show)
    {
        gameOverScreen.SetActive(show);
    }


}
