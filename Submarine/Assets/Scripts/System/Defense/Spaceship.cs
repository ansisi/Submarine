using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FactionHandler))]
public class Spaceship : MonoBehaviour, IDamageable
{
    [Header("Spaceship Health Settings")]
    public float maxHealth = 100f;     // 우주선 최대 체력
    [SerializeField]
    private float currentHealth;       // 우주선 현재 체력

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
        {
            OnDeath();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void OnDeath()
    {
        GameManager.Instance.GameOver();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}