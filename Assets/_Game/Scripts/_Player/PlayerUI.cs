using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private static PlayerUI _instance;
    public static PlayerUI Instance => _instance;
    
    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private CanvasGroup hudGroup;
    [SerializeField] private CanvasGroup deathScreenGroup;
    [SerializeField] private RectTransform deathText;
    
    [Header("UI Elements")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;

    [Header("UI RectTransforms")]
    [SerializeField] private RectTransform healthBarRect;
    [SerializeField] private RectTransform staminaBarRect;
    [SerializeField] private RectTransform manaBarRect;

    [Header("Max Allowed Stats")]
    [SerializeField] private float maxAllowedHealthWidth = 1800f;
    [SerializeField] private float maxAllowedStaminaWidth = 1800f;
    [SerializeField] private float maxAllowedManaWidth = 1800f;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("More than one PlayerUI found");
            return;
        }
        _instance = this;
    }

    public void Init()
    {
        hudGroup.gameObject.SetActive(true);
        deathScreenGroup.gameObject.SetActive(false);
        UpdateTotalUI();
        UpdateUI();
    }

    [Button]
    public void UpdateTotalUI()
    {
        AdjustHealthBarWidth();
        AdjustStaminaBarWidth();
        AdjustManaBarWidth();
    }

    private void Update()
    {
        if (playerStats.IsDead)
        {
            if(!deathScreenGroup.gameObject.activeSelf)
                ShowDeadScreen();
            return;
        }
        UpdateUI();
    }

    private void ShowDeadScreen()
    {
        hudGroup.gameObject.SetActive(false);
        deathScreenGroup.gameObject.SetActive(true);
        deathScreenGroup.alpha = 0;
        Sequence sequence = DOTween.Sequence()
            .AppendInterval(1)
            .Append(deathScreenGroup.DOFade(1, 1f))
            .Join(deathText.DOScale(1.5f, 2f).SetEase(Ease.Linear)).Play();
        ;
    }

    private void UpdateUI()
    {
        healthBar.fillAmount = playerStats.currentHealth / playerStats.maxHealth;
        staminaBar.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
        manaBar.fillAmount = playerStats.currentMana / playerStats.maxMana;
    }

    public void AdjustHealthBarWidth()
    {
        float widthFactor = playerStats.maxHealth / (float)playerStats.maxAllowedHealth;
        healthBarRect.sizeDelta = new Vector2(maxAllowedHealthWidth * widthFactor, healthBarRect.sizeDelta.y);
        healthBar.fillAmount = playerStats.currentHealth / playerStats.maxHealth;
    }

    public void AdjustStaminaBarWidth()
    {
        float widthFactor = playerStats.maxStamina / (float)playerStats.maxAllowedStamina;
        staminaBarRect.sizeDelta = new Vector2(maxAllowedStaminaWidth * widthFactor, staminaBarRect.sizeDelta.y);
        staminaBar.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
    }

    public void AdjustManaBarWidth()
    {
        float widthFactor = playerStats.maxMana / (float)playerStats.maxAllowedMana;
        manaBarRect.sizeDelta = new Vector2(maxAllowedManaWidth * widthFactor, manaBarRect.sizeDelta.y);
        manaBar.fillAmount = playerStats.currentMana / playerStats.maxMana;
    }
}