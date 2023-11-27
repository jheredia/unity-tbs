using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitName;

    private Unit unit;
    // Start is called before the first frame update
    void Start()
    {
        unit = UnitActionSystem.Instance.GetSelectedUnit();
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UpdateUIElements();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateUIElements();
    }

    private void UpdateUIElements()
    {
        if (unit != null) unitName.text = unit.name;
    }
}
