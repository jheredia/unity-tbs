using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

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
        gridSystemVisualSingleArray = new GridSystemVisualSingle[levelGridWidth, levelGridHeight];
        for (int x = 0; x < levelGridWidth; x++)
        {
            for (int z = 0; z < levelGridHeight; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, levelGrid.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    private void Update()
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            GridSystemVisualSingle gridSystemVisualSingle = gridSystemVisualSingleArray[gridPosition.x, gridPosition.z];
            gridSystemVisualSingle.Show();
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList());
    }
}
