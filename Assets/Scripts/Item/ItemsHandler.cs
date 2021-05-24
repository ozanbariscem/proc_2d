using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ItemsHandler : MonoBehaviour
{
    public static ItemsHandler Instance;

    private void Awake()
    {
        Instance = this;
        UserData.RegisterAssembly();

        ItemData.LoadItems();
    }

    public Item CreateItem(string tag, Cell onCell, ushort amount)
    {
        return ItemData.CreateFromUniqueTag(tag, onCell, amount);
    }
}
