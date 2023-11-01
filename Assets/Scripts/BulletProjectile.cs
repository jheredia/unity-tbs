using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletParticleVFXPrefab;
    private Vector3 targetPosition;
    [SerializeField] private float projectileSpeed = 200f;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        transform.position += moveDirection * projectileSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate(bulletParticleVFXPrefab, targetPosition, Quaternion.identity);
        }
    }
}
