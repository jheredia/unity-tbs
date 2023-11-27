using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    [SerializeField] private int health = 100;
    private int maxHealth;

    public event EventHandler OnDeath;
    public event EventHandler OnDamage;
    public event EventHandler OnHeal;

    private void Awake()
    {
        maxHealth = health;
    }

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
        OnDamage?.Invoke(this, EventArgs.Empty);
        if (health == 0) Die();
    }

    public void Heal(int healingAmount)
    {
        health += healingAmount;
        if (health > maxHealth) health = maxHealth;
        OnHeal?.Invoke(this, EventArgs.Empty);
    }


    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health / maxHealth;
    }

    public int GetHealth() => health;

    public int GetMaxHealth() => maxHealth;
}
