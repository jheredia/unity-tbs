using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyCrateDestroyed;
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
    }

    private void DestructibleCrate_OnAnyCrateDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GetGridPosition(), true);
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        // Unit unit = sender as Unit;
        // Pathfinding.Instance.SetIsWalkableGridPosition(unit.GetGridPosition(), false);
    }

    private void Unit_OnAnyUnitDied(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        Pathfinding.Instance.SetIsWalkableGridPosition(unit.GetGridPosition(), true);
    }
}
