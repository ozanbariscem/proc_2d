using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Item : IItem
{
    public override void OnCreate()
    {
        ItemData.AddToCreatedItems(data.unique_tag, this);
    }

    public override void OnDrop()
    {
        transform.gameObject.SetActive(true);
        transform.position = onCell.TruePosition + new Vector3(0.5f, 0.5f, 0);

        TaskSystem.Instance.AddNewTask(new TaskSystem.Task.Haul()
        {
            targetCell = onCell,
            container = onCell.ClosestContainer(),
            item = this,
        }, 10);

        data.script.Call(data.on_drop_func, this);
    }

    public override void OnPick(Character character)
    {
        this.transform.gameObject.SetActive(false);
        onCharacter = character;

        data.script.Call(data.on_pick_func, this);
    }

    public override void OnStore(CellObject container)
    {
        UIInventory.Instance.NewEntry(data.unique_tag, amount, data.icon);

        data.script.Call(data.on_store_func, this);
    }

    public void CreateTransform()
    {
        ItemData data = ItemData.itemDatas[this.data.unique_tag];

        GameObject obj = new GameObject(data.name);
        obj.transform.position = onCell.TruePosition + new Vector3(0.5f, 0.5f, 0);
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = data.sprite;
        sr.sortingOrder = 11;

        transform = obj.transform;
    }
}
