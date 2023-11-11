using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private Action onGrenadeBehaviourComplete;
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float moveSpeed = 15f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float deltaDistance = .2f;
        float damageDropdown = 5; // Based on the distance from the target position, do less damage if it's further away
        if (Vector3.Distance(transform.position, targetPosition) < deltaDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit unit))
                {
                    int damageDealt = UnityEngine.Random.Range(20, 40);
                    float distanceFromTarget = Vector3.Distance(collider.transform.position, targetPosition);
                    int finalDamage = Mathf.RoundToInt(damageDealt - (distanceFromTarget * damageDropdown));
                    unit.Damage(Mathf.Abs(finalDamage));
                }
            }
            onGrenadeBehaviourComplete();
            Destroy(gameObject);
        }
    }
}
