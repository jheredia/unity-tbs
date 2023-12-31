using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    const float explosionForce = 300f;
    const float explosionRange = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(Transform originalRootBone)
    {
        Setup(originalRootBone, transform.position);
    }

    public void Setup(Transform originalRootBone, Vector3 ragdollExplosionPosition)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ApplyExplosionToRagdoll(ragdollRootBone, explosionForce, ragdollExplosionPosition + randomDirection, explosionRange);
    }


    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        if (clone == null) return; // This is needed for childs that destroy themselves like trails
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
            }
            MatchAllChildTransforms(child, cloneChild);
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidBody))
            {
                childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
