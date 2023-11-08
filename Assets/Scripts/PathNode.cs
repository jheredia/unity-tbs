using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;

    // Cost to move from the node currently being tested to this one
    private int gCost;

    // Heuristic cost 
    private int hCost;
    private int fCost;
    private PathNode previousNode;
    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }


    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;

    public void SetGCost(int gCost) => this.gCost = gCost;
    public void SetHCost(int hCost) => this.hCost = hCost;

    public void CalculateFCost() => fCost = hCost + gCost;

    public void SetPreviousNode(PathNode pathNode) => previousNode = pathNode;

    public PathNode GetPreviousNode() => previousNode;

    public void ResetPreviousNodeValue() => previousNode = null;

    public GridPosition GetGridPosition() => gridPosition;
}
