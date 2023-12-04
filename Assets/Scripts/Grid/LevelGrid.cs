using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{

    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 3f;
    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs
    {
        public float x;
        public float z;
    }

    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] float cellSize = 2f;
    [SerializeField] int floorAmount;

    private List<GridSystem<GridObject>> gridSystemList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple instances of {GetType().Name} present {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridSystemList = new List<GridSystem<GridObject>>();
        for (int floor = 0; floor < floorAmount; floor++)
        {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(
                width,
                height,
                cellSize,
                floor,
                FLOOR_HEIGHT,
                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition)
            );
            gridSystemList.Add(gridSystem);
        }

        // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
    // Start is called before the first frame update
    void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridSystem<GridObject> gridSystem = gridSystemList[gridPosition.floor];
        if (gridSystem != null)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }
        else
        {
            Debug.LogError("Attempting to add an unit to a non existing floor");
        }
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromPosition, GridPosition targetPosition)
    {
        RemoveUnitAtGridPosition(fromPosition, unit);
        AddUnitAtGridPosition(targetPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
        {
            x = targetPosition.x,
            z = targetPosition.z
        });
    }

    public int GetFloor(Vector3 worldPosition)
    {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        int floor = GetFloor(worldPosition);
        return GetGridSystem(floor).GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount)
        {
            return false;
        }
        return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).HasAnyUnit();

    public int GetHeight(int floor = 0) => GetGridSystem(floor).GetHeight();

    public int GetWidth(int floor = 0) => GetGridSystem(floor).GetWidth();

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetInteractable(interactable);
    }

    public void RemoveInteractableAtGridPosition(GridPosition gridPosition)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).RemoveInteractable();
    }

    private GridSystem<GridObject> GetGridSystem(int floor) => gridSystemList[floor];

    public int GetFloorAmount() => floorAmount;
}