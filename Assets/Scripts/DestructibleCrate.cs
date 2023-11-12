using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField, Min(1)] private int hitPoints;

    public static event EventHandler OnAnyCrateDestroyed;

    private GridPosition gridPosition;

    public void Damage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            OnAnyCrateDestroyed?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GridPosition GetGridPosition() => gridPosition;
}
