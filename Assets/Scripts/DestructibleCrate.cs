using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{

    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;
    [SerializeField, Min(1)] private int hitPoints;


    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    // Damage without an unspecificed origin will assume it's the same position as the transform
    public void Damage(int damage)
    {
        Damage(damage, transform.position);
    }

    public void Damage(int damage, Vector3 damageOrigin, float explosionForce = 150f, float explosionRadius = 10f)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

            ApplyExplosionToChildren(crateDestroyedTransform, explosionForce, damageOrigin, explosionRadius);

            Destroy(gameObject);

            OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }

}

