using UnityEngine;
using UnityEngine.TextCore.Text;

public class SkillSystemTester : MonoBehaviour
{
    [SerializeField] private Character caster;
    [SerializeField] private Character target;
    [SerializeField] private int testSkillId = 1007;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TestCast();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PrintCooldown();
        }
    }

    private void TestCast()
    {
        if (caster == null)
        {
            Debug.LogError("测试失败：Caster为空");
            return;
        }
        int characterId = caster.gameObject.GetInstanceID();
        bool success = SkillManager.Instance.RequestCast(characterId, testSkillId, caster, target);
        Debug.Log($"技能测试结果：{success}");
    }

    private void PrintCooldown()
    {
        if (caster == null)
        {
            Debug.LogError("测试失败：Caster为空");
            return;
        }
        int characterId = caster.gameObject.GetInstanceID();
        if (!SkillManager.Instance.TryGetRuntime(characterId, out CharacterRuntime runtime))
        {
            Debug.LogError($"找不到角色运行时：{characterId}");
            return;
        }

        float cooldown = runtime.GetRemainingCooldown(testSkillId);
        Debug.Log($"技能 {testSkillId} 剩余冷却：{cooldown:F2} 秒");
    }
}