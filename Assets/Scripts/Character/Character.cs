using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Character : ICharacter
{
    public override void PickItem(Item item)
    {
        if (itemsInInventory == null)
            itemsInInventory = new List<Item>();

        itemsInInventory.Add(item);
        item.OnPick(this);
    }

    public override void StoreItem(CellObject container, Item item)
    {
        if (itemsInInventory == null)
            return;
        
        if (itemsInInventory.Remove(item))
        {
            item.OnStore(container);
        }
    }

    public override void NewPathTo(Cell targetCell)
    {
        path = new PathAStar(onCell, targetCell);
    }

    public override void MoveTo(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, 
            DeveloperMode.skipwalk ? 
            Vector3.Distance(transform.position, position) : (speed * Time.deltaTime));
    }

    public override void MoveAlongPath()
    {
        if (targetCell != null)
        {
            Vector2 targetPosition = new Vector2(targetCell.TrueX + 0.5f, targetCell.TrueY + 0.5f);
            MoveTo(targetPosition);

            if (Vector2.Distance(transform.position, targetPosition) < 0.02f)
            {
                transform.position = targetPosition;
                onCell = targetCell;
                targetCell = path.Dequeue();
            }
        } 
    }
}
