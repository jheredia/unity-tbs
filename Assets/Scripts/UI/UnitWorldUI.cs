using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UnitWorldUI : MonoBehaviour
{
    // Action points UI elements
    [SerializeField] private TextMeshProUGUI actionPointsText;

    [SerializeField] private Image actionPointsBarImage;

    // Health UI elements
    [SerializeField] private TextMeshProUGUI healthPointsText;
    [SerializeField] private Image healthBarImage;

    // Systems 
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Unit unit;


    // Start is called before the first frame update
    void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamage += HealthSystem_OnDamage;
        healthSystem.OnHeal += HealthSystem_OnHeal;
        UpdateActionPointsText();
        UpdateActionPointsBar();
        UpdateHealthBar();
        UpdateHealthText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Action points UI updates
    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
        UpdateActionPointsBar();
    }

    private void UpdateActionPointsBar()
    {
        actionPointsBarImage.fillAmount = unit.GetActionPointsNormalized();
    }

    // Health UI updates
    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDamage(object sender, EventArgs e)
    {
        UpdateHealthBar();
        UpdateHealthText();
    }

    private void HealthSystem_OnHeal(object sender, EventArgs e)
    {
        UpdateHealthBar();
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        healthPointsText.text = $"{healthSystem.GetHealth()}/{healthSystem.GetMaxHealth()}";
    }
}
