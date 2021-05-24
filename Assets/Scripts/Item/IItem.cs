using UnityEngine;

public abstract class IItem
{
    public ItemData data;
    public Cell onCell;
    public Character onCharacter;
    public ushort amount;
    public Transform transform;

    public abstract void OnCreate();
    public abstract void OnDrop();
    public abstract void OnPick(Character character);
    public abstract void OnStore(CellObject container);
}
