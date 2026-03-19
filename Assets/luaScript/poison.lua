local buff = {}
local damage=5

function buff.Apply(character, level)
    print("Poison Apply")
end

function buff.Tick(character, level)
    character:TakeDamage(damage * level)
end

function buff.Remove(character, level)
    print("Poison End")
end

return buff