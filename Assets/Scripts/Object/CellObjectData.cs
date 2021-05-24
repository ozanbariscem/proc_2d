using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class CellObjectData
{
    public static Dictionary<string, CellObjectData> objectDatas;
    public static Dictionary<string, List<CellObject>> createdObjects;

    public string unique_tag;
    public string name;
    public string description;

    public byte width;
    public byte height;

    public ushort max_durability;
    public uint container_capacity;

    public byte pathfinding_cost;

    [JsonIgnore]
    public byte PathfindingCost
    {
        get
        {
            if (is_walkable) return pathfinding_cost;
            else return 0;
        }
    }

    public bool is_walkable;
    public bool is_carriable;
    public bool is_craftable;
    public bool is_container;
    public bool is_choppable;
    public bool is_diggable;


    [JsonIgnore] public string path;
    [JsonIgnore] public Sprite sprite_base;
    [JsonIgnore] public Sprite sprite_2sided;
    [JsonIgnore] public Sprite sprite_3sided;
    [JsonIgnore] public Sprite sprite_4sided;

    [JsonIgnore] public Script script;
    [JsonIgnore] public DynValue on_create_func;
    [JsonIgnore] public DynValue on_build_func;
    [JsonIgnore] public DynValue on_destroy_func;

    [MoonSharpHidden]
    public static void LoadObjects()
    {
        Debug.LogWarning("Doesnt read mods right now.");
        string[] objectPaths = Directory.GetDirectories(Application.streamingAssetsPath + "/Mods/Vanilla/Objects");

        for (int i = 0; i < objectPaths.Length; i++)
        {
            LoadObject(objectPaths[i]);
        }
    }

    [MoonSharpHidden]
    public static void LoadObject(string filename)
    {
        CellObjectData data = GetDataFromJSON(filename);

        if (data == null) return; // Case: couldnt find file
        if (!data.TryGetTextureSet()) return;
        if (!data.TryGetScriptSet()) return;

        AddToObjectDatas(data);
    }

    private static CellObjectData GetDataFromJSON(string filename)
    {
        if (!File.Exists(filename + "/object_data.txt")) return null;

        string json;
        using (StreamReader sr = new StreamReader(filename + "/object_data.txt"))
        {
            json = sr.ReadToEnd();
        }
        CellObjectData data = JsonConvert.DeserializeObject<CellObjectData>(json);
        data.path = filename;
        return data;
    }

    private bool TryGetTextureSet()
    {
        if (!Directory.Exists(path + "/Textures")) return false;

        string[] texturePaths = Directory.GetFiles(path + "/Textures", "*.*").Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();

        for (int i = 0; i < texturePaths.Length; i++)
        {
            if (texturePaths[i].Contains("base_texture"))
            {
                byte[] texture_byte_array = File.ReadAllBytes(texturePaths[i]);
                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, texture_byte_array);
                sprite_base = Sprite.Create(
                                texture,
                                new Rect(0.0f, 0.0f, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                texture.width / width);

            }
            else if (texturePaths[i].Contains("texture_2sided"))
            {
                byte[] texture_2sided_byte_array = File.ReadAllBytes(texturePaths[i]);
                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, texture_2sided_byte_array);
                sprite_2sided = Sprite.Create(
                                texture,
                                new Rect(0.0f, 0.0f, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                texture.width / width);

            }
            else if (texturePaths[i].Contains("texture_3sided"))
            {
                byte[] texture_3sided_byte_array = File.ReadAllBytes(texturePaths[i]);
                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, texture_3sided_byte_array);
                sprite_3sided = Sprite.Create(
                                texture,
                                new Rect(0.0f, 0.0f, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                texture.width / width);
            }
            else if (texturePaths[i].Contains("texture_4sided"))
            {
                byte[] texture_4sided_byte_array = File.ReadAllBytes(texturePaths[i]);
                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, texture_4sided_byte_array);
                sprite_4sided = Sprite.Create(
                                texture,
                                new Rect(0.0f, 0.0f, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f),
                                texture.width / width);
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

        script.Globals.Set("ItemsHandler", UserData.Create(ItemsHandler.Instance));

        on_create_func = script.Globals.Get("OnCreate");
        on_build_func = script.Globals.Get("OnBuild");
        on_destroy_func = script.Globals.Get("OnDestroy");

        return true;
    }

    [MoonSharpHidden]
    public static void CreateDefaultJSON()
    {
        CellObjectData data = new CellObjectData();
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        using (StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/Mods/Vanilla/Objects/default_object_data.txt"))
        {
            sw.Write(json);
        }
    }

    [MoonSharpHidden]
    public static void LoadDefaultJSON()
    {
        CellObjectData data;
        string json;
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/Mods/Vanilla/Objects/default_object_data.txt"))
        {
            json = sr.ReadToEnd();
        }
        data = JsonConvert.DeserializeObject<CellObjectData>(json);
    }

    private static void AddToObjectDatas(CellObjectData data)
    {
        if (objectDatas == null)
            objectDatas = new Dictionary<string, CellObjectData>();
        if (!objectDatas.ContainsKey(data.unique_tag))
            objectDatas.Add(data.unique_tag, data);
        else if (!objectDatas.ContainsKey(data.path.Replace(Application.streamingAssetsPath, "") + "/" + data.unique_tag))
            objectDatas.Add(data.path.Replace(Application.streamingAssetsPath, "") + "/" + data.unique_tag, data);
        else
            objectDatas.Add(data.path.Replace(Application.streamingAssetsPath, "") + "/" + data.unique_tag + "_overwritten_by_same_mod", data);
    }
   
    public static void AddToCreatedObjects(CellObject obj)
    {
        if (createdObjects == null)
        {
            createdObjects = new Dictionary<string, List<CellObject>>();
        }
        if (obj.data.is_craftable)
            AddValueToCreatedObjectDict("craftable", obj);
        if (obj.data.is_container)
            AddValueToCreatedObjectDict("container", obj);
    }

    private static void AddValueToCreatedObjectDict(string key, CellObject value)
    {
        if (!createdObjects.ContainsKey(key))
        {
            createdObjects.Add(key, new List<CellObject>());
        }
        createdObjects[key].Add(value);
    }

    [MoonSharpHidden]
    public static CellObject CreateFromData(CellObjectData data)
    {
        CellObject obj = new CellObject()
        {
            data = data,
            durability = data.max_durability,
            onCells = new Cell[data.width, data.height]
        };

        obj.OnCreate();
        return obj;
    }
}
