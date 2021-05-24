-- Called when an object has been created, not necessarily built
-- param: object -> object created using this data
function OnCreate(object)
    return object.data.unique_tag
end

-- Called when an object has been built, meaning its placed on world space
-- param: object -> object created using this data
function OnBuild(object)
    return object.PlacedCell.TrueX
end

-- Called when an object has been destroyed, meaning its durability reached < 0
-- param: object -> object created using this data
function OnDestroy(object)
    local item = ItemsHandler.CreateItem('wood', object.PlacedCell, object.data.max_durability)
    return item
end