using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeaponManager : MonoBehaviour
{
    [SerializeField] Transform rifleTransform;
    [SerializeField] Transform swordTransform;
    [SerializeField] Transform grenadeTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out MeleeAction meleeAction))
        {
            meleeAction.OnSwordActionStarted += MeleeAction_OnSwordActionStarted;
            meleeAction.OnSwordActionCompleted += MeleeAction_OnSwordActionCompleted;
        }
        if (TryGetComponent(out GrenadeAction grenadeAction))
        {
            grenadeAction.OnGrenadeLaunchStarted += GrenadeAction_OnGrenadeLaunchStarted;
            grenadeAction.OnGrenadeLaunchCompleted += GrenadeAction_OnGrenadeLaunchCompleted;
        }
        EquipRifle();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
        grenadeTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
        grenadeTransform.gameObject.SetActive(false);
    }

    private void EquipGrenade()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(false);
        grenadeTransform.gameObject.SetActive(true);
    }
    private void GrenadeAction_OnGrenadeLaunchCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void GrenadeAction_OnGrenadeLaunchStarted(object sender, EventArgs e)
    {
        EquipGrenade();
    }

    private void MeleeAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
    }

    private void MeleeAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

}
