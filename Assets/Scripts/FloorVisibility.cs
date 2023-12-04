using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] private bool dynamicFloorPosition;
    [SerializeField] private List<Renderer> ignoreRenderers;
    private Renderer[] rendererArray;
    private int floor;

    private void Awake()
    {
        rendererArray = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position);
        if (floor == 0 && dynamicFloorPosition) Destroy(this);
    }

    private void Update()
    {
        // Better to update only when an unit changes floor
        if (dynamicFloorPosition)
        {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }
        float cameraHeight = CameraController.Instance.GetCameraHeight();
        float floorHeightOffset = 2f;
        bool showObject = cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset;
        if (showObject || floor == 0) Show();
        else Hide();
    }

    public void Show(float alpha = 0)
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRenderers.Contains(renderer)) continue;
            renderer.enabled = true;
        }
    }

    public void Hide()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRenderers.Contains(renderer)) continue;
            renderer.enabled = false;
        }
    }
}
