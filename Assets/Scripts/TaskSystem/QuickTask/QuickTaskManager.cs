using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTaskManager : MonoBehaviour
{
    public static QuickTaskManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetQuickCommand(int index)
    {
        if (index < 0 || index >= Enum.GetValues(typeof(QuickTasks)).Length)
        {
            Debug.LogError($"Couldn't find quick command with index {index}.");
            return;
        }
        else
        {
            TileSelection.Instance.quickTask = (QuickTasks)index;
        }
    }

    public void ChopTask(List<Cell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.PathfindingCost != 0 && cell.objectOnCell != null && cell.objectOnCell.data.is_choppable)
            {
                TaskSystem.Task.ChopTree task = new TaskSystem.Task.ChopTree { targetCell = cell };
                TaskSystem.Instance.AddNewTask(task);
            }
        }
    }

    public void DigTask(List<Cell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.objectOnCell != null && cell.objectOnCell.data.is_diggable)
            {
                TaskSystem.Task.Dig task = new TaskSystem.Task.Dig { targetCell = cell };
                TaskSystem.Instance.AddNewTask(task);
            }
        }
    }

    public void BuildTask(List<Cell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.objectOnCell == null)
            {
                TaskSystem.Task.Build task = new TaskSystem.Task.Build { 
                    targetCell = cell,
                    item = cell.ClosestItemOfTypeWithAmount("rock", 100),
                };
                TaskSystem.Instance.AddNewTask(task);
            }
        }
    }
}
