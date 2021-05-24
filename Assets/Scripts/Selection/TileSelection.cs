using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelection : MonoBehaviour
{
    public static TileSelection Instance;

    public List<Cell> selectedCells;

    public Cell selectionStart;
    public Cell lastAdded;

    public QuickTasks quickTask;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        selectedCells = new List<Cell>();
        quickTask = QuickTasks.None;
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedCells.Clear();
                Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Cell targetCell = Map.map.GetCell((int)clickPos.x, (int)clickPos.y);
                selectionStart = targetCell;
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Cell targetCell = Map.map.GetCell((int)clickPos.x, (int)clickPos.y);

                if (targetCell != null)
                {
                    var bounds = GetMarqueeBounds(selectionStart.TruePosition, targetCell.TruePosition);
                    foreach (var pos in bounds.allPositionsWithin)
                    {
                        Cell cell = Map.map.GetCell(pos.x, pos.y);
                        selectedCells.Add(cell);
                    }
                }
                
                switch (quickTask)
                {
                    case QuickTasks.None:
                        break;
                    case QuickTasks.Chop:
                        QuickTaskManager.Instance.ChopTask(selectedCells);
                        break;
                    case QuickTasks.Dig:
                        QuickTaskManager.Instance.DigTask(selectedCells);
                        break;
                    case QuickTasks.Build:
                        QuickTaskManager.Instance.BuildTask(selectedCells);
                        break;
                }
            }
            else
            if (Input.GetMouseButton(0))
            { // Case: being held down
                
            }
        }
    }

    public static BoundsInt GetMarqueeBounds(Vector3Int p1, Vector3Int p2)
    {
        return new BoundsInt(
                Mathf.Min(p1.x, p2.x),
                Mathf.Min(p1.y, p2.y),
                Mathf.Min(p1.z, p2.z),
                Mathf.Abs(p2.x - p1.x) + 1,
                Mathf.Abs(p2.y - p1.y) + 1,
                Mathf.Abs(p2.z - p1.z) + 1
        );
    }
}
