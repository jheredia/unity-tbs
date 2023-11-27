using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour
{
    [SerializeField] Sector bigRoomHider;
    [SerializeField] Sector startingSector;
    // Start is called before the first frame update
    void Start()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Door.OnAnyDoorOpened += Door_OnAnyDoorOpened;
        startingSector.AwakeEnemies();
        startingSector.SetHiderVisibility(0);
    }

    private void Door_OnAnyDoorOpened(object sender, Door.OnDoorInteractedEventArgs e)
    {
        SetActiveSectorList(e.connectedSectors, 0);
        foreach (Sector sector in e.connectedSectors)
        {
            sector.AwakeEnemies();
        }
    }


    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        if (e.x == 12 && e.z == 8)
        {
            SetActiveSectorList(new List<Sector> { bigRoomHider }, 0);
        }
    }

    private void SetActiveSectorList(List<Sector> sectors, float alpha = 255f)
    {
        foreach (Sector sector in sectors)
        {
            sector.SetHiderVisibility(alpha);
        }
    }
}
