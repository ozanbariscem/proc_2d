using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public abstract class ICharacter
{
    public Transform transform;
    public CharacterAI AI;
    public PathAStar path;
    public Cell onCell;
    public Cell targetCell;

    public List<Item> itemsInInventory;

    public string name = "<empty_character_name>";
    public float speed = 5;
    public byte health = 100;
    public bool playerControlled = false;

    public abstract void PickItem(Item item);
    public abstract void StoreItem(CellObject container, Item item);
    public abstract void NewPathTo(Cell targetCell);
    public abstract void MoveTo(Vector3 position);
    public abstract void MoveAlongPath();
}
