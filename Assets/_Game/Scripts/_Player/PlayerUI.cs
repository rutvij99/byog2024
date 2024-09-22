using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private static PlayerUI _instance;
    public static PlayerUI Instance => _instance;


    [SerializeField] private GameObject trackerObj;
    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private CanvasGroup hudGroup;
    [SerializeField] private CanvasGroup deathScreenGroup;
    [SerializeField] private RectTransform deathText;
    
    [Header("UI Elements")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image healthDiffBar;
    [SerializeField] private Image staminaDiffBar;
    [SerializeField] private Image manaDiffBar;

    [Header("UI RectTransforms")]
    [SerializeField] private RectTransform healthBarRect;
    [SerializeField] private RectTransform staminaBarRect;
    [SerializeField] private RectTransform manaBarRect;

    [Header("Max Allowed Stats")]
    [SerializeField] private float maxAllowedHealthWidth = 1800f;
    [SerializeField] private float maxAllowedStaminaWidth = 1800f;
    [SerializeField] private float maxAllowedManaWidth = 1800f;

    [Header("Weapon UI")]
    [SerializeField] private Image weaponImage; 

    // Add sprites for each weapon type.
    [SerializeField] private Sprite swordShieldSprite;
    [SerializeField] private Sprite greatSwordSprite;
    [SerializeField] private Sprite wandSprite;
    [SerializeField] private Sprite noWeaponSprite; // When no weapon is equipped
    
    private void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("More than one PlayerUI found");
            return;
        }
        _instance = this;
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    public void Init()
    {
        hudGroup.gameObject.SetActive(true);
        deathScreenGroup.gameObject.SetActive(false);
        UpdateTotalUI();
        UpdateUI();
        UpdateWeaponUI();
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
            .Join(deathText.DOScale(1.5f, 2f).SetEase(Ease.Linear))
            .AppendCallback(() =>
            {
                SceneManager.LoadScene(this.gameObject.scene.name);
            }).Play();
        ;
    }
    
    public void UpdateWeaponUI()
    {
        // Get the current weapon type from the PlayerStats
        switch (playerStats.currentWeaponType)
        {
            case WeaponType.SwordShield:
                weaponImage.sprite = swordShieldSprite;
                break;
            case WeaponType.GreatSword:
                weaponImage.sprite = greatSwordSprite;
                break;
            case WeaponType.Wand:
                weaponImage.sprite = wandSprite;
                break;
            default:
                weaponImage.sprite = noWeaponSprite;
                break;
        }
    }

    private void UpdateUI()
    {
        healthBar.fillAmount = playerStats.currentHealth / playerStats.maxHealth;
        StartCoroutine(DiffCoroutine(healthDiffBar, healthBar));
        staminaBar.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
        StartCoroutine(DiffCoroutine(staminaDiffBar, staminaBar));
        manaBar.fillAmount = playerStats.currentMana / playerStats.maxMana;
        StartCoroutine(DiffCoroutine(manaDiffBar, manaBar));

    }
    
    public void ShowTracker(bool show)
    {
        trackerObj.SetActive(show);
    }

    public void UpdateLockPoisiton(Vector3 pos)
    {
        var screenPos = Camera.main.WorldToScreenPoint(pos);
        trackerObj.transform.position = screenPos;
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
    
    private IEnumerator DiffCoroutine(Image Diff, Image Fill)
    {
        yield return new WaitForSeconds(2f);
        float timeStep = 0;
        float start = Diff.fillAmount;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 0.25f;
            Diff.fillAmount = Mathf.Lerp(start, Fill.fillAmount, timeStep);
            yield return new WaitForEndOfFrame();
        }
    }
}