using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class AttackInfo
{
    public enum AttackType
    {
        Melee,
        Range
    }

    public string attackId;
    public AttackType type;
    public float attackDuration;
    public float staminaCost;
    public float manaCost;
    public float healthCost;
    public float damage;
    public float maxChargeMultiplier;
}

public class PlayerStats : MonoBehaviour
{
    [Header("Max Cap")]
    public int maxAllowedHealth = 1800;
    public int maxAllowedMana = 1800;
    public int maxAllowedStamina = 1800;
    
    [Header("Current Max")]
    public int maxHealth = 100;
    public int maxMana = 100;
    public int maxStamina = 100;

    [Header("Current Stats")]
    [ReadOnly] public float currentHealth;
    [ReadOnly] public float currentMana;
    [ReadOnly] public float currentStamina;

    [Header("Multipliers")]
    public bool allowHealthRegeneration = true;
    public bool allowManaRegeneration = true;
    public bool allowStaminaRegeneration = true;
    public float healthRegenRate = 0.5f;
    public float healthRegenDelay = 1.0f;
    public float staminaRegenRate = 5f;   // Stamina regen per second
    public float staminaRegenDelay = 1.0f; // Delay before stamina starts regenerating after depletion
    public float manaRegenRate = 3f;
    public float manaRegenDelay = 1.0f;

    
    [Header("Cost")]
    public float dodgeCost = 50f;
    public float dashCost = 20f;
    public float blockCost = 10f;
    public float attackCost = 30f;
    public float subsequentAttackCostMultiplier = 1f;
    public float sprintCostPerSecond = 5f;
    public float sprintCostMultiplier = 0.4f;

    
    [Header("Attacks")]
    public List<AttackInfo> airAttacks;
    public List<AttackInfo> lightMeleeAttacks;
    public List<AttackInfo> heavyMeleeAttacks;
    public List<AttackInfo> rangeAttacks;
    
    [Space(20)]
    [ReadOnly] public float healthRegenTimer = 0f;
    [ReadOnly] public float staminaRegenTimer = 0f;
    [ReadOnly] public float manaRegenTimer = 0f;
    public bool IsDead { get; private set; } = false;

    private void Start()
    {
        // Initialize stats
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMana = maxMana;
        PlayerUI.Instance.Init();
    }
    
    private void Update()
    {
        if(IsDead) return;
        RegenerateStats();
    }

    #region Upgrade Max Health
    public void UpgradeMaxHealth(int amount)
    {
        maxHealth += amount;
        maxHealth = Mathf.Clamp(maxHealth, 0, maxAllowedHealth);
        currentHealth = maxHealth;  // Optionally, refill health after upgrade
        PlayerUI.Instance.UpdateTotalUI();
    }
    public void UpgradeMaxStamina(int amount)
    {
        maxStamina += amount;
        maxStamina = Mathf.Clamp(maxStamina, 0, maxAllowedStamina);
        currentStamina = maxStamina;  // Optionally, refill health after upgrade
        PlayerUI.Instance.UpdateTotalUI();

    }
    public void UpgradeMaxMana(int amount)
    {
        maxMana += amount;
        maxMana = Mathf.Clamp(maxMana, 0, maxAllowedMana);
        currentMana = maxMana;  // Optionally, refill health after upgrade
        PlayerUI.Instance.UpdateTotalUI();
    }
    #endregion
    
    private void Die()
    {
        IsDead = true;
        Debug.Log("Player is dead!");
    }
    
    #region Updaters
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(); // Handle player death
            return;
        }
        
        if(allowHealthRegeneration)
            healthRegenTimer = healthRegenDelay;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Cap health to max
        }
    }

   

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        
        if(allowStaminaRegeneration)
            staminaRegenTimer = staminaRegenDelay;
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0)
        {
            currentMana = 0;
        }
        if(allowManaRegeneration)
           manaRegenTimer = manaRegenDelay;
    }
    #endregion

    #region Regeneration Logic
    private void RegenerateStats()
    {
        if(allowHealthRegeneration)
            RegenerateHealth();
        if(allowStaminaRegeneration)
            RegenerateStamina();
        if(allowManaRegeneration)
            RegenerateMana();
    }

    
    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina && staminaRegenTimer <= 0)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            // Debug.Log($"Stamina Regeneration: {currentStamina}");
        }

        if (staminaRegenTimer > 0)
        {
            staminaRegenTimer -= Time.deltaTime;
        }
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana && manaRegenTimer <= 0)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            // Debug.Log($"Mana Regeneration: {currentMana}");
        }

        if (manaRegenTimer > 0)
        {
            manaRegenTimer -= Time.deltaTime;
        }
    }

    private void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            // Debug.Log($"Health Regeneration: {currentHealth}");
        }
        
        if (healthRegenTimer > 0)
        {
            healthRegenTimer -= Time.deltaTime;
        }
    }
    #endregion


    #region Usages

    public bool IsAttackPossible(AttackInfo attackInfo)
    {
        return attackInfo.healthCost <= currentHealth && attackInfo.staminaCost <= currentStamina && attackInfo.manaCost <= currentMana;
    }
    
    public void AttackModifiers(AttackInfo attackInfo)
    {
        UseStamina(attackInfo.staminaCost);
        UseMana(attackInfo.manaCost);
        TakeDamage(attackInfo.healthCost);
    }
    
    public bool IsDashPossible()
    {
        return dashCost <= currentStamina;
    }
    public bool IsDodgePossible()
    {
        return dodgeCost <= currentStamina;
    }
    public void DashModifiers()
    {
        UseStamina(dashCost);
    }
    
    public void DodgeModifiers()
    {
        UseStamina(dodgeCost);
    }

    #endregion
    
    #region Testing
    [Button]
    public void TestUpdateUI()
    {
        PlayerUI.Instance.UpdateTotalUI();
    }
    [Button]
    public void TestMana50()
    {
        UseMana(50);
    }
    [Button]
    public void TestStamina50()
    {
        UseStamina(50);
    }
    [Button]
    public void TestHealth50()
    {
        TakeDamage(50);
    }
    #endregion
}