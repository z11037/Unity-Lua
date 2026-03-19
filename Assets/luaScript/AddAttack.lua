local buff = {}

function buff.Apply(character, level)
    print("Buff: Increase attack")
    character:AddAttack(10 * level)
end

function buff.Remove(character, level)
    print("Buff: Removed")
    character:AddAttack(-10 * level)
end

return buff