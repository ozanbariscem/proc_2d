local action = {}

function action.Action(character, cell, cellRange)
    if character.path == nil then
        character.NewPathTo(cell)

        if character.path.Length() ~= 0 then
            character.targetCell = character.path.Dequeue()
            character.targetCell = character.path.Dequeue()
        else
            character.AI.AbandonTask()
            return false
        end
    end
    character.MoveAlongPath()
    if character.onCell == cell then
        character.path = nil
        return true
    else
        if cellRange == 0 then
            if character.path.Length() == cellRange - 1 then
                return true
            end
        end
    end
    return false
end

return action