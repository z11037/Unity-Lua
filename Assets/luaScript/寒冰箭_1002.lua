-- ====================================
-- Skill : 寒冰箭
-- ID    : 1002
-- Auto Generated
-- ====================================

local skill = {}

skill.cooldown = 3

------------------------------------------------
-- 技能执行入口
------------------------------------------------
function skill.Execute(attacker, target)

    -- TODO 播放动画

    -- TODO 播放特效

    -- TODO 造成伤害
    target:TakeDamage(attacker:GetAttackValue())

end

return skill
