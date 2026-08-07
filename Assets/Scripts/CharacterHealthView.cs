using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthView : MonoBehaviour
{
    [Header("目标角色")]
    [SerializeField] private Character target;

    [Header("UI")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthSlider;

    private CharacterRuntime runtime;
    private Coroutine bindCoroutine;

    private void OnEnable()
    {
        bindCoroutine = StartCoroutine(BindWhenReady());
    }

    private void OnDisable()
    {
        if (bindCoroutine != null)
        {
            StopCoroutine(bindCoroutine);
            bindCoroutine = null;
        }

        UnbindRuntime();
    }

    private IEnumerator BindWhenReady()
    {
        while (isActiveAndEnabled)
        {
            if (target == null)
            {
                Debug.Log("[CharacterHealthView] 未设置目标角色");
                bindCoroutine = null;
                yield break;
            }

            CharacterRuntime targetRuntime = target.GetRuntime();

            if (targetRuntime != null)
            {
                BindRuntime(targetRuntime);
                bindCoroutine = null;
                yield break;
            }

            yield return null;
        }

        bindCoroutine = null;
    }

    private void BindRuntime(CharacterRuntime targetRuntime)
    {
        if (targetRuntime == null)
        {
            return;
        }

        if (runtime == targetRuntime)
        {
            RefreshHealth(runtime.CurrentHealth, runtime.MaxHealth.Value);
            return;
        }

        UnbindRuntime();

        runtime = targetRuntime;
        runtime.OnHealthChanged += HandleHealthChanged;

        RefreshHealth(runtime.CurrentHealth, runtime.MaxHealth.Value);

        Debug.Log($"[CharacterHealthView] 已绑定角色 {runtime.CharacterId} 的生命值");
    }

    private void UnbindRuntime()
    {
        if (runtime == null)
        {
            return;
        }

        runtime.OnHealthChanged -= HandleHealthChanged;
        runtime = null;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        RefreshHealth(currentHealth, maxHealth);
    }

    private void RefreshHealth(float currentHealth, float maxHealth)
    {
        float validMaxHealth = Mathf.Max(1f, maxHealth);
        float validCurrentHealth = Mathf.Clamp(currentHealth, 0f, validMaxHealth);

        if (healthText != null)
        {
            healthText.text = $"{Mathf.FloorToInt(validCurrentHealth)} / {Mathf.FloorToInt(validMaxHealth)}";
        }

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = validMaxHealth;
            healthSlider.value = validCurrentHealth;
        }
    }
}