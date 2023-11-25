using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sector : MonoBehaviour
{
    [SerializeField] private List<Unit> enemyList;
    [SerializeField] private List<GameObject> hiderList;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material transparentMaterial;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Unit enemy in enemyList)
        {
            enemy.SetIsAwake(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Unit> GetEnemyList() => enemyList;

    public List<GameObject> GetHiders() => hiderList;

    public void AwakeEnemies()
    {
        foreach (Unit enemy in enemyList)
        {
            enemy.SetIsAwake(true);
        }
    }

    public void SetHiderVisibility(float alpha)
    {
        foreach (MeshRenderer hider in GetComponentsInChildren<MeshRenderer>())
        {
            hider.material.color = new Color(0, 0, 0, alpha);
        }
    }
}
