using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 150;

    public event EventHandler OnDeath;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health < 0) health = 0;
        if (health == 0) Die();
        Debug.Log(health);
    }

    public void Heal(int healingAmount)
    {
        health += healingAmount;
        if (health > maxHealth) health = maxHealth;
    }


    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }
}
