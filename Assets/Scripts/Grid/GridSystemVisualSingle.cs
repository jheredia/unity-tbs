using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject selectedGameObject;

    public void Show(Material material)
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
            meshRenderer.material = material;
        }
    }

    public void Hide()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    public void ShowSelected(bool show = true)
    {
        selectedGameObject.SetActive(show);
    }
}
