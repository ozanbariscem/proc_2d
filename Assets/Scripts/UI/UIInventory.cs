using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventory : MonoBehaviour
{
    public class Entry
    {
        public GameObject obj;
        public Image icon;
        public TextMeshProUGUI text;
        public int amount;
    }

    public static UIInventory Instance;
    private Dictionary<string, Entry> entries;

    public GameObject entryPrefab;
    public Transform contentTransform;

    public void Awake()
    {
        Instance = this;
    }

    public void NewEntry(string tag, int amount, Sprite icon)
    {
        if (entries == null)
            entries = new Dictionary<string, Entry>();
        if (!entries.ContainsKey(tag))
        {
            GameObject obj = Instantiate(entryPrefab);
            obj.transform.SetParent(contentTransform);
            obj.transform.localScale = Vector3.one;

            Entry entry = new Entry()
            {
                obj = obj,
                text = obj.transform.Find("text").GetComponent<TextMeshProUGUI>(),
                icon = obj.transform.Find("icon").GetComponent<Image>(),
                amount = 0,
            };
            entry.icon.sprite = icon;

            entries.Add(tag, entry);
        }
        entries[tag].amount += amount;
        entries[tag].text.text = entries[tag].amount.ToString();
    }
}
