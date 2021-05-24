Tag = 'moveto'
Name = 'Move To'
Description = 'Moves the selected character to target cell.'

TargetCell = nil
CellRange = 0
MoveAction = require('MoveToCellAction')

function Execute(character)
    return MoveAction.Action(character, TargetCell, CellRange)
end

function Validate()
    return true
end