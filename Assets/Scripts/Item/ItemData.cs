using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ItemData
{
    public static Dictionary<string, ItemData> itemDatas;
    public static Dictionary<string, List<Item>> createdItems;

    public string unique_tag;
    public string name;
    public string description;

    public ushort weight;

    [JsonIgnore] public string path;
    [JsonIgnore] public Sprite icon;
    [JsonIgnore] public Sprite sprite;

    [JsonIgnore] public Script script;
    [JsonIgnore] public DynValue on_drop_func;
    [JsonIgnore] public DynValue on_pick_func;
    [JsonIgnore] public DynValue on_store_func;

    [MoonSharpHidden]
    public static void LoadItems()
    {
        Debug.LogWarning("Doesnt read mods right now.");
        string[] objectPaths = Directory.GetDirectories(Application.streamingAssetsPath + "/Mods/Vanilla/Items");

        for (int i = 0; i < objectPaths.Length; i++)
        {
            LoadItem(objectPaths[i]);
        }
    }

    [MoonSharpHidden]
    public static void LoadItem(string filename)
    {
        ItemData data = GetDataFromJSON(filename);

        if (data == null) return; // Case: couldnt find file
        if (!data.TryGetTextureSet()) return;
        if (!data.TryGetScriptSet()) return;

        AddToObjectDatas(data);
    }

    private static ItemData GetDataFromJSON(string filename)
    {
        if (!File.Exists(filename + "/item_data.txt")) return null;

        string json;
        using (StreamReader sr = new StreamReader(filename + "/item_data.txt"))
        {
            json = sr.ReadToEnd();
        }
        ItemData data = JsonConvert.DeserializeObject<ItemData>(json);
        data.path = filename;
        return data;
    }

    private bool TryGetTextureSet()
    {
        if (!Directory.Exists(path + "/Textures")) return false;

        string[] texturePaths = Directory.GetFiles(path + "/Textures", "*.*").Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();

        for (int i = 0; i < texturePaths.Length; i++)
        {
            if (texturePaths[i].Contains("texture"))
            {
                byte[] byteArray = File.ReadAllBytes(texturePaths[i]);
                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, byteArray);
                sprite = Sprite.Create(
                                texture,
                                new Rect(0.0f, 0.0f, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                texture.width * 2);

            }
            else if (texturePaths[i].Contains("icon"))
            {
                byte[] byteArray = File.ReadAllBytes(texturePaths[i]);
                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, byteArray);
                icon = Sprite.Create(
                                texture,
                                new Rect(0.0f, 0.0f, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                texture.width);

            }
        }
        return true;
    }

    private bool TryGetScriptSet()
    {
        if (!File.Exists(path + "/Scripts/scripts.lua")) return false;
        string lua_source;
        using (StreamReader sr = new StreamReader(path + "/Scripts/scripts.lua"))
        {
            lua_source = sr.ReadToEnd();
        }

        script = new Script();
        script.DoString(lua_source);

        on_drop_func = script.Globals.Get("OnDrop");
        on_pick_func = script.Globals.Get("OnPick");
        on_store_func = script.Globals.Get("OnStore");

        return true;
    }

    private static void AddToObjectDatas(ItemData data)
    {
        if (itemDatas == null)
            itemDatas = new Dictionary<string, ItemData>();
        if (!itemDatas.ContainsKey(data.unique_tag))
            itemDatas.Add(data.unique_tag, data);
        else if (!itemDatas.ContainsKey(data.path.Replace(Application.streamingAssetsPath, "") + "/" + data.unique_tag))
            itemDatas.Add(data.path.Replace(Application.streamingAssetsPath, "") + "/" + data.unique_tag, data);
        else
            itemDatas.Add(data.path.Replace(Application.streamingAssetsPath, "") + "/" + data.unique_tag + "_overwritten_by_same_mod", data);
    }

    public static void AddToCreatedItems(string key, Item item)
    {
        if (createdItems == null)
            createdItems = new Dictionary<string, List<Item>>();
        if (!createdItems.ContainsKey(key))
        {
            createdItems.Add(key, new List<Item>());
        }
        createdItems[key].Add(item);
    }

    public static Item CreateFromUniqueTag(string tag, Cell onCell, ushort amount)
    {
        if (itemDatas.ContainsKey(tag))
        {
            ItemData data = itemDatas[tag];
            Item item = new Item() { data = data, onCell = onCell, amount = amount };
            onCell.AddItemToCell(item);
            item.CreateTransform();
            item.OnCreate();
            item.OnDrop();
            return item;
        }
        return null;
    }
}
