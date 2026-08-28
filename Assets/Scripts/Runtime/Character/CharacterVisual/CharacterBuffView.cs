using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class CharacterBuffView : MonoBehaviour
{
    [Header("目标角色")]
    [SerializeField] private Character target;

    [Header("UI")]
    [SerializeField] private TMP_Text buffText;

    [Header("刷新设置")]
    [SerializeField] private float refreshInterval = 0.1f;

    private readonly StringBuilder stringBuilder = new StringBuilder();

    private CharacterRuntime runtime;
    private Coroutine bindCoroutine;
    private float refreshTimer;

    private void OnEnable()
    {
        bindCoroutine = StartCoroutine(BindWhenReady());
    }

    private void Update()
    {
        if (runtime == null)
        {
            return;
        }

        refreshTimer -= Time.deltaTime;

        if (refreshTimer > 0f)
        {
            return;
        }

        refreshTimer = Mathf.Max(0.02f, refreshInterval);
        RefreshBuffText();
    }

    private void OnDisable()
    {
        if (bindCoroutine != null)
        {
            StopCoroutine(bindCoroutine);
            bindCoroutine = null;
        }

        runtime = null;
        ClearText();
    }

    private IEnumerator BindWhenReady()
    {
        while (isActiveAndEnabled)
        {
            if (target == null)
            {
                Debug.Log("[CharacterBuffView] 未设置目标角色");
                bindCoroutine = null;
                yield break;
            }

            CharacterRuntime targetRuntime = target.GetRuntime();

            if (targetRuntime != null)
            {
                runtime = targetRuntime;
                refreshTimer = 0f;
                RefreshBuffText();

                Debug.Log($"[CharacterBuffView] 已绑定角色 {runtime.CharacterId} 的 Buff 状态");

                bindCoroutine = null;
                yield break;
            }

            yield return null;
        }

        bindCoroutine = null;
    }

    private void RefreshBuffText()
    {
        if (buffText == null)
        {
            return;
        }

        stringBuilder.Clear();

        for (int i = 0; i < runtime.Buffs.Count; i++)
        {
            Buff buff = runtime.Buffs[i];

            if (buff == null || buff.Config == null)
            {
                continue;
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.AppendLine();
            }

            stringBuilder.Append(buff.DisplayName);
            stringBuilder.Append(" ×");
            stringBuilder.Append(buff.CurrentStack);
            stringBuilder.Append("  ");
            stringBuilder.Append(Mathf.Max(0f, buff.RemainingTime).ToString("F1"));
            stringBuilder.Append("s");
        }

        if (stringBuilder.Length == 0)
        {
            buffText.text = "当前无 Buff";
            return;
        }

        buffText.text = stringBuilder.ToString();
    }

    private void ClearText()
    {
        if (buffText != null)
        {
            buffText.text = string.Empty;
        }
    }
}