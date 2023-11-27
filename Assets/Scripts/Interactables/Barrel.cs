using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Barrel : MonoBehaviour, IInteractable
{

    public static event EventHandler OnAnyDestroyed;

    private GridPosition gridPosition;

    private Action onInteractionComplete;
    private float timer;
    private bool isActive;
    private Animator animator;
    private const string SHOW_POPUP_PARAMETER = "ShowPopup";
    [SerializeField] private int keyLoot;
    [SerializeField] private int grenadeLoot;
    [SerializeField] private Transform barrelDestroyedPrefab;

    private bool isDestroyed = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = .5f;
        Unit interactingUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (keyLoot > 0)
        {
            int unitKeyCount = interactingUnit.GetKeys();
            interactingUnit.SetKeys(unitKeyCount + keyLoot);
        }
        if (grenadeLoot > 0)
        {
            // TODO: Refactor this in a proper inventory system
            int unitGrenadeCount = interactingUnit.GetAction<GrenadeAction>().GetAvailableCharges();
            interactingUnit.GetAction<GrenadeAction>().SetAvailableCharges(unitGrenadeCount + grenadeLoot);
        }
        Transform crateDestroyedTransform = Instantiate(barrelDestroyedPrefab, transform.position, transform.rotation);
        float explosionForce = 150f;
        float explosionRadius = 10f;
        ApplyExplosionToChildren(crateDestroyedTransform, explosionForce, transform.position, explosionRadius);

        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponent<Light>().enabled = false;
        isDestroyed = true;
        animator.SetBool(SHOW_POPUP_PARAMETER, isDestroyed);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        LevelGrid.Instance.RemoveInteractableAtGridPosition(gridPosition);
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
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
