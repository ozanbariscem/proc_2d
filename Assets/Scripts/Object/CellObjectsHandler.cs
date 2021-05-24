using UnityEngine;
using MoonSharp.Interpreter;

public class CellObjectsHandler : MonoBehaviour
{
    private void Awake()
    {
        UserData.RegisterAssembly();

        CellObjectData.LoadObjects();
    }

    [ContextMenu("Create Default Cell Object Data JSON")]
    public void CreateDefaultCellObjectDataJSON()
    {
        CellObjectData.CreateDefaultJSON();
    }

    [ContextMenu("Load Default Cell Object Data JSON")]
    public void LoadDefaultCellObjectDataJSON()
    {
        CellObjectData.LoadDefaultJSON();
    }
}
