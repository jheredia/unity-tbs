using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private Action onGrenadeBehaviourComplete;
    public static event EventHandler OnAnyGrenadeExplosion;
    [SerializeField] private Transform grenadeExplosionVFXPrefab;
    // [SerializeField] private Transform grenadeSmokeVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private float totalDistance;
    private Vector3 positionXZ;

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - (distance / totalDistance);

        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized);

        float maxHeight = totalDistance / 4f;
        transform.position = new Vector3(positionXZ.x, positionY * maxHeight, positionXZ.z);
        float deltaDistance = .2f;
        float damageDropdown = 5; // Based on the distance from the target position, do less damage if it's further away
        if (Vector3.Distance(positionXZ, targetPosition) < deltaDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                int damageDealt = UnityEngine.Random.Range(20, 40);
                float distanceFromTarget = Vector3.Distance(collider.transform.position, targetPosition);
                int finalDamage = Mathf.Abs(Mathf.RoundToInt(damageDealt - (distanceFromTarget * damageDropdown)));
                if (collider.TryGetComponent<Unit>(out Unit unit))
                {
                    unit.Damage(finalDamage);
                }
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage(finalDamage, targetPosition);
                }

            }


            OnAnyGrenadeExplosion?.Invoke(this, EventArgs.Empty);
            if (trailRenderer != null) trailRenderer.transform.parent = null;
            Instantiate(grenadeExplosionVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            // Instantiate(grenadeSmokeVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);
            onGrenadeBehaviourComplete();
        }
    }
}
