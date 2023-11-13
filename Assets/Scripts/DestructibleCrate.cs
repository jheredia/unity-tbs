using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField, Min(1)] private int hitPoints;

    public static event EventHandler OnAnyCrateDestroyed;

    [SerializeField] private Transform destroyedCratePrefab;

    private GridPosition gridPosition;

    public void Damage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Transform destroyedCrateTransform = Instantiate(destroyedCratePrefab, transform.position, transform.rotation);
            ApplyExplosionToChildren(destroyedCratePrefab, 1500f, transform.position, 100f);
            Destroy(gameObject);
            OnAnyCrateDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition() => gridPosition;

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidBody))
            {
                childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
