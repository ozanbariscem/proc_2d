using System;
using System.Collections.Generic;

public static class DeveloperMode
{
    public enum OverlayRenderMode
    {
        none, is_walkable, room
    }

    public static Dictionary<string, OverlayRenderMode> overlayRenderModes;

    public static bool onehit = false;
    public static bool skipwalk = false;
    public static OverlayRenderMode overlayRenderMode;

    public static DeveloperModeConsole Console { 
        get { return DeveloperModeConsole.Instance; } 
    }

    public static void NewCommand(string input)
    {
        string[] command = input.Split(' ');

        switch (command[0])
        {
            case "clear":
                Console.Clear();
                break;
            case "onehit":
                OnehitMode(command[1]);
                break;
            case "skipwalk":
                SkipWalkMode(command[1]);
                break;
            case "give_item":
                GiveItem(command[1], command[2]);
                break;
            case "developer_overlay_render":
                SetOverlayRenderMode(command[1]);
                break;
            default:
                Console.PushLog($"Can't understand command: {input}");
                break;
        }
    }

    public static void OnehitMode(string input)
    {
        onehit = input == "true";
        Console.PushLog($"Onehit mode set to: {onehit}");
    }

    public static void SkipWalkMode(string input)
    {
        skipwalk = input == "true";
        Console.PushLog($"Skip walk mode set to: {skipwalk}");
    }

    public static void SetOverlayRenderMode(string input)
    {
        if (overlayRenderModes == null) SetOverlayRenderModes();
        if (overlayRenderModes.ContainsKey(input))
        {
            overlayRenderMode = overlayRenderModes[input];
            Map.map.DeveloperOverlayRender(overlayRenderMode);
            Console.PushLog($"Overlay render mode set to: {overlayRenderMode}");
        }
        else
        {
            Console.PushLog($"Couldn't find render mode: {input}");
            Console.PushLog($"Possible options are: ", false);
            foreach (var key in overlayRenderModes.Keys)
            {
                Console.PushLog($"{key} ", false);
            }
            Console.PushLog($"");
        }
    }

    public static void GiveItem(string input, string amount)
    {
        if (ushort.TryParse(amount, out ushort _amount))
        {
            if (_amount != 0)
            {
                Item item = ItemsHandler.Instance.CreateItem(input, CharacterController.Instance.AIs[0].character.onCell, _amount);
                if (item == null)
                    Console.PushLog($"Given item tag: ({input}), doesn't exist.");
                else
                    Console.PushLog($"Created item ({input}x{amount}) at {CharacterController.Instance.AIs[0].character.onCell.TruePosition}.");
            }
            else
                Console.PushLog($"amount should be a value between 1-{ushort.MaxValue}, but was given: {amount}");
        }
        else
        {
            Console.PushLog($"amount should be a value between 1-{ushort.MaxValue}, but was given: {amount}");
        }
    }

    private static void SetOverlayRenderModes()
    {
        overlayRenderModes = new Dictionary<string, OverlayRenderMode>();

        string[] names = Enum.GetNames(typeof(OverlayRenderMode));

        for (int i = 0; i < names.Length; i++)
        {
            overlayRenderModes.Add(names[i], (OverlayRenderMode)i);
        }
    }
}
