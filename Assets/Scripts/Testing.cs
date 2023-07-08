using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    [SerializeField] private Unit unit;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) GridSystemVisual.Instance.ShowGridPositionList(unit.GetMoveAction().GetValidActionGridPositionList());
        if (Input.GetKeyDown(KeyCode.H)) GridSystemVisual.Instance.HideAllGridPositions();
        //Debug.Log(gridSystem.GetGridPosition(MouseWorld.GetPosition()));
    }

}
