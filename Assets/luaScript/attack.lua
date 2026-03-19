local skill = {}

function skill.Execute(attacker,character)
    character:TakeDamage(attacker.attack.Value)
    print(attacker.name.."攻击了"..character.name)
    print(character.name.."剩余"..character.health.Value)
end

return skill