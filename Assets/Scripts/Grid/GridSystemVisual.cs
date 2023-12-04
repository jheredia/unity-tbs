using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        Blue,
        Green,
        LightBlue,
        Red,
        SoftRed,
        White,
        Yellow
    }

    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask floorLayerMask;

    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;
    private GridSystemVisualSingle lastSelectedGridSystemVisualSingle;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple instances of GridSystemVisual present {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelGrid levelGrid = LevelGrid.Instance;
        int levelGridWidth = levelGrid.GetWidth();
        int levelGridHeight = levelGrid.GetHeight();
        gridSystemVisualSingleArray = new GridSystemVisualSingle[levelGridWidth, levelGridHeight, levelGrid.GetFloorAmount()];
        for (int x = 0; x < levelGridWidth; x++)
        {
            for (int z = 0; z < levelGridHeight; z++)
            {
                for (int floor = 0; floor < levelGrid.GetFloorAmount(); floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, levelGrid.GetWorldPosition(gridPosition), Quaternion.identity);
                    gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }
        }
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        levelGrid.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyCrateDestroyed;
        Barrel.OnAnyDestroyed += Barrel_OnAnyDestroyed;
        UpdateGridVisual();
    }

    private void Update()
    {
        lastSelectedGridSystemVisualSingle?.ShowSelectedGridVisual(false);
        Vector3 mouseWorldPosition = MouseWorld.GetPositionOnlyHitVisible();
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPosition);
        if (LevelGrid.Instance.IsValidGridPosition(gridPosition))
            lastSelectedGridSystemVisualSingle = gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor];

        lastSelectedGridSystemVisualSingle?.ShowSelectedGridVisual();
    }

    private void HideAllGridPositions()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }

    private void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            GridSystemVisualSingle gridSystemVisualSingle = gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor];
            gridSystemVisualSingle.Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {


        List<GridPosition> gridPositionList = new List<GridPosition>();
        Vector3 originWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                for (int floor = -range; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = gridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Get the radius
                    if (testDistance > range) { continue; }// Outside of range
                    if (!HasFloorOnGridPosition(testGridPosition)) continue;
                    if (HasObstacleOnGridPosition(testGridPosition)) continue;
                    float unitShoulderHeight = 1.7f;
                    Vector3 testWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                    Vector3 actionDirection = (testWorldPosition - originWorldPosition).normalized;
                    if (Physics.Raycast(
                        originWorldPosition + Vector3.up * unitShoulderHeight,
                        actionDirection,
                        Vector3.Distance(originWorldPosition, testWorldPosition),
                        obstaclesLayerMask
                    )) continue;
                    if (Physics.Raycast(
                        originWorldPosition + Vector3.up * unitShoulderHeight,
                        actionDirection,
                        Vector3.Distance(originWorldPosition, testWorldPosition),
                        floorLayerMask
                    )) continue;
                    gridPositionList.Add(testGridPosition);
                }
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);

    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {


        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                if (!HasFloorOnGridPosition(testGridPosition)) continue;
                if (HasObstacleOnGridPosition(testGridPosition)) continue;
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);

    }

    private bool HasFloorOnGridPosition(GridPosition gridPosition)
    {
        float raycastOffsetDistance = 1f;
        Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        return Physics.Raycast(
                        worldPosition + Vector3.up * raycastOffsetDistance,
                        Vector3.down,
                        raycastOffsetDistance * 2,
                        floorLayerMask);
    }

    private bool HasObstacleOnGridPosition(GridPosition gridPosition)
    {
        float raycastOffsetDistance = 1f;
        Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        return Physics.Raycast(
                        worldPosition + Vector3.down * raycastOffsetDistance,
                        Vector3.up,
                        raycastOffsetDistance * 2,
                        obstaclesLayerMask);
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) return;
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        GridVisualType gridVisualType = GridVisualType.White;
        GridVisualType rangeGridVisualType = GridVisualType.White;
        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.LightBlue;
                break;
            case AttackAction attackAction:
                gridVisualType = GridVisualType.Red;
                rangeGridVisualType = GridVisualType.SoftRed;
                break;
            case MeleeAction meleeAction:
                gridVisualType = GridVisualType.Red;
                rangeGridVisualType = GridVisualType.SoftRed;
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Green;
                break;
            case CrouchAction crouchAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Green;
                break;
        }
        List<GridPosition> validGridPositionList = selectedAction.GetValidActionGridPositionList();
        if (selectedAction.GetShowRange())
        {
            ShowGridPositionRange(selectedUnit.GetGridPosition(), selectedAction.GetActionRange(), rangeGridVisualType);
        }
        ShowGridPositionList(
            validGridPositionList,
            gridVisualType);
    }


    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Unit_OnAnyUnitDied(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void DestructibleCrate_OnAnyCrateDestroyed(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Barrel_OnAnyDestroyed(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) return gridVisualTypeMaterial.material;
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
