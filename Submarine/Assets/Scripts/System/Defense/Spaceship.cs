using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(FactionHandler))]
public class Spaceship : MonoBehaviour, IDamageable
{
    [Header("Spaceship Health Settings")]
    public float maxHealth = 100f;     // 우주선 최대 체력
    public float baseMaxHealth = 100f; // 업그레이드 전 기본 maxHealth
    [SerializeField] private float currentHealth;       // 우주선 현재 체력

    private Coroutine autoRepairCoroutine;
    private float autoRepairAmount = 10f;
    private float autoRepairInterval = 0f;

    [SerializeField] private Material spaceshipMaterial;

    private void Awake()
    {
        baseMaxHealth = maxHealth; // 초기값을 Awake에서 기록
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += HandleWaveStarted;
            WaveManager.Instance.OnWaveEnded += HandleWaveEnded;
        }
    }

    private void Update()
    {
        if (spaceshipMaterial != null)
        {
            float hpRatio = currentHealth / maxHealth;
            spaceshipMaterial.SetFloat("_HPAmount", hpRatio);
        }
    }
    private void OnDestroy()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted -= HandleWaveStarted;
            WaveManager.Instance.OnWaveEnded -= HandleWaveEnded;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
        {
            StopAutoRepair(); // 죽을 때 회복 중단
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

    public void SetAutoRepairInterval(float interval)
    {
        autoRepairInterval = interval;
    }

    public void StartAutoRepair(float intervalSeconds)
    {
        autoRepairInterval = intervalSeconds;
        autoRepairAmount = 10f;

        if (autoRepairCoroutine != null)
            StopCoroutine(autoRepairCoroutine);

        autoRepairCoroutine = StartCoroutine(AutoRepairCoroutine());
    }

    public void StopAutoRepair()
    {
        if (autoRepairCoroutine != null)
        {
            StopCoroutine(autoRepairCoroutine);
            autoRepairCoroutine = null;
        }
    }

    private IEnumerator AutoRepairCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoRepairInterval);
            Heal(autoRepairAmount);
        }
    }

    private void HandleWaveStarted(int waveIndex)
    {
        StopAutoRepair();
    }

    private void HandleWaveEnded(int waveIndex)
    {
        // 자동회복 레벨을 PlayerData에서 직접 조회
        int arLevel = PlayerData.Instance.GetUpgradeLevel(UpgradeType.AutoRepair);

        // 2레벨 이상, 그리고 현재 체력이 최대의 50% 이하라면
        if (arLevel >= 2 && currentHealth <= maxHealth * 0.5f)
        {
            currentHealth = maxHealth * 0.7f; 
        }


        if (autoRepairInterval > 0f)
            StartAutoRepair(autoRepairInterval);
    }
}