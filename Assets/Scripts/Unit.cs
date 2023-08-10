using UnityEngine;

public class Unit : MonoBehaviour
{

    private GridPosition gridPosition;
    private MoveAction moveAction;

    private SpinAction spinAction;

    private CrouchAction crouchAction;
    private BaseAction[] baseActionArray;
    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        crouchAction = GetComponent<CrouchAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }


    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }


    public MoveAction GetMoveAction() => moveAction;

    public SpinAction GetSpinAction() => spinAction;

    public CrouchAction GetCrouchAction() => crouchAction;

    public GridPosition GetGridPosition() => gridPosition;

    public BaseAction[] GetUnitActions() => baseActionArray;
}
