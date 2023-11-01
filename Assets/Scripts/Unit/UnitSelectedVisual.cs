using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    // Texture of the selector
    private MeshRenderer meshRenderer;

    // Get the mesh renderer and assign it
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UpdateVisual();
    }

    /// <summary>
    /// Listens to unit selection change on the class <c>UnitActionSystem</c>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="empty"></param>
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    /// <summary>
    /// UpdateVisual enables or disables the mesh renderer of the unit's selected visual object
    /// </summary>
    private void UpdateVisual()
    {
        meshRenderer.enabled = UnitActionSystem.Instance.GetSelectedUnit() == unit;
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }
}
