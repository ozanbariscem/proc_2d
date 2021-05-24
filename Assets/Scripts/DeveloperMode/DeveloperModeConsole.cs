using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeveloperModeConsole : MonoBehaviour
{
    public static DeveloperModeConsole Instance;

    public GameObject parent;
    public Transform content;
    public TMP_InputField inputField;
    public TextMeshProUGUI logText;
    public Scrollbar scrollbar;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        logText.autoSizeTextContainer = true;

        NewCommand("onehit true");
        NewCommand("skipwalk true");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Hide();
        }
        if (parent.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                NewCommand(inputField.text);
                inputField.text = "";
            }
        }
    }

    void Hide()
    {
        CameraController.Instance.AcceptInput(parent.activeInHierarchy);
        parent.SetActive(!parent.activeInHierarchy);
    }

    public void NewCommand(string input)
    {
        DeveloperMode.NewCommand(input);
    }

    public void PushLog(string log, bool is_end_of_line = true)
    {
        logText.text += log + (is_end_of_line ? "\n" : "");

        Vector2 textSize = logText.GetPreferredValues(logText.text);
        logText.rectTransform.sizeDelta = textSize + Vector2.one;

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        scrollbar.value = 0;
    }

    public void Clear()
    {
        logText.text = "";
    }
}
