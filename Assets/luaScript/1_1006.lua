-- ====================================
-- Skill : 1
-- ID    : 1006
-- Auto Generated
-- ====================================

local skill = {}

skill.cooldown = 0

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
