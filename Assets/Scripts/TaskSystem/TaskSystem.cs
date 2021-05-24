using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class TaskSystem
{
    public abstract class Task
    {
        public enum TaskState
        {
            MoveToCell,
            Chop,
            GetPath, Dig,
            MoveToItem, PickItem, MoveToContainer, StoreItem,
            Build,
            Done
        }

        public Cell targetCell;
        public TaskState state;

        public class MoveToCell : Task
        {
            public int cellRange = 0;
            public new TaskState state = TaskState.MoveToCell;

            public Func<Character, Cell, int, bool> moveAction = MoveToCellAction;
        }

        public class ChopTree : Task
        {
            public int cellRange = 1;
            public new TaskState state = TaskState.MoveToCell;

            public Func<Character, Cell, bool> validateAction = ValidateChopAction;
            public Func<Character, Cell, int, bool> moveAction = MoveToCellAction;
            public Func<Character, Cell, bool> chopAction = ChopTreeAction;
        }

        public class Dig : Task
        {
            public int cellRange = 1;
            public new TaskState state = TaskState.GetPath;

            public Func<Cell, bool> validateAction = ValidateDigAction;
            public Func<Character, Cell, int, bool> moveAction = MoveToCellAction;
            public Func<Character, Cell, bool> digAction = DigAction;
        }

        public class Haul : Task
        {
            public int cellRange = 0;
            public new TaskState state = TaskState.MoveToItem;

            public Item item;
            public CellObject container;

            public Func<Item, CellObject, bool> validateAction = ValidateHaulAction;
            public Func<Character, Cell, int, bool> moveToItemAction = MoveToCellAction;
            public Func<Character, Item, bool> pickAction = PickAction;
            public Func<Character, Cell, int, bool> moveToContainerAction = MoveToCellAction;
            public Func<Character, CellObject, Item, bool> storeAction = StoreAction;
        }

        public class Build : Task
        {
            public int cellRange = 1;
            public byte buildProgress = 0;
            public new TaskState state = TaskState.MoveToItem;

            public Item item;

            public Func<bool> validateAction = ValidateBuildAction;
            public Func<Character, Cell, int, bool> moveToItemAction = MoveToCellAction;
            public Func<Character, Item, bool> pickAction = PickAction;
            public Func<Character, Cell, int, bool> moveToCellAction = MoveToCellAction;
            public Func<Build, Character, Cell, bool> buildAction = BuildAction;
        }

        public static bool MoveToCellAction(Character character, Cell cell, int cellRange)
        {
            if (character.path == null)
            {
                character.NewPathTo(cell);
                if (character.path.Length() != 0)
                {
                    character.targetCell = character.path.Dequeue();
                    character.targetCell = character.path.Dequeue();
                }
                else
                {
                    character.AI.InvalidateTask();
                    return false;
                }
            }
            character.MoveAlongPath();
            if (character.onCell == cell)
            {
                character.path = null;
                return true;
            }
            else
            {
                if (cellRange != 0)
                {
                    if (character.path.Length() == cellRange - 1)
                    {
                        // Arrived in range
                        character.path = null;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ChopTreeAction(Character character, Cell cell)
        {
            if (cell.objectOnCell == null)
            {
                character.AI.InvalidateTask();
                return false;
            }

            if (cell.objectOnCell.durability > 0)
            {
                float damage = DeveloperMode.onehit ? (cell.objectOnCell.durability) : (10 * Time.deltaTime);

                if ((float)cell.objectOnCell.durability - damage < 0)
                {
                    cell.objectOnCell.durability = 0;
                }
                else
                {
                    cell.objectOnCell.durability = (byte)((float)cell.objectOnCell.durability - damage);
                }
            }

            if (cell.objectOnCell.durability <= 0)
            {
                cell.RemoveObjectOnCell();
                return true;
            }

            return false;
        }

        public static bool DigAction(Character character, Cell cell)
        {
            if (cell.objectOnCell == null)
            {
                Debug.Log("hello");
                character.AI.InvalidateTask();
                return false;
            }
            Debug.Log("hello1");
            if (cell.objectOnCell.durability > 0)
            {
                float damage = DeveloperMode.onehit ? (cell.objectOnCell.durability) : (10 * Time.deltaTime);

                if ((float)cell.objectOnCell.durability - damage < 0)
                {
                    cell.objectOnCell.durability = 0;
                }
                else
                {
                    cell.objectOnCell.durability = (byte)((float)cell.objectOnCell.durability - damage);
                }
            }
            Debug.Log("hello2");
            if (cell.objectOnCell.durability <= 0)
            {
                cell.RemoveObjectOnCell();
                return true;
            }
            Debug.Log("hello3");
            return false;
        }

        public static bool PickAction(Character character, Item item)
        {
            if (item.onCharacter != null && item.onCharacter == character) return true;

            if (item.onCell == null) // Item was moved
            {
                character.AI.InvalidateTask();
                return false;
            }
            character.PickItem(item);
            return true;
        }

        public static bool StoreAction(Character character, CellObject container, Item item)
        {
            if (container.PlacedCell == null || !container.data.is_container) // object on target cell was changed
            {
                character.AI.InvalidateTask();
                return false;
            }
            character.StoreItem(container, item);
            return true;
        }

        public static bool BuildAction(Build task, Character character, Cell cell)
        {
            if (cell.objectOnCell != null)
            {
                character.AI.InvalidateTask();
                return false;
            }

            if (task.buildProgress < 100)
            {
                float damage = DeveloperMode.onehit ? (byte.MaxValue) : (10 * Time.deltaTime);

                task.buildProgress += (byte)Mathf.Max(damage, 255);
            }

            if (task.buildProgress >= 100)
            {
                cell.AddObjectToCell(CellObjectData.CreateFromData(CellObjectData.objectDatas["stone"]));
                return true;
            }

            return false;
        }

        public static bool ValidateDigAction(Cell cell)
        {
            Debug.Log("asked to validate");
            if (cell.objectOnCell == null || !cell.objectOnCell.data.is_diggable)
                return false;
            
            // < 4, Protects for diagonal digging, And even without it, pathfinding cant find a path to cell,
            // resulting in hiccups
            for (int i = 0; i < 4; i++)
            {
                if (cell.SetNeighbours()[i].PathfindingCost != 0) return true;
            }

            return false;
        }

        public static bool ValidateChopAction(Character character, Cell cell)
        {
            if (cell.objectOnCell == null || !cell.objectOnCell.data.is_choppable)
            {
                return false;
            }
            int i;
            for (i = 0; i < 4; i++)
            {
                if (cell.SetNeighbours()[i].PathfindingCost != 0) { i = -1; break; };
            }
            if (i != -1) return false;

            return true;
        }

        public static bool ValidateHaulAction(Item item, CellObject container)
        {
            if (container == null)
            {
                container = item.onCell.ClosestContainer();
                if (container == null) return false;
            }
            int i;
            for (i = 0; i < 4; i++)
            {
                if (container.PlacedCell.SetNeighbours()[i].PathfindingCost != 0) { i = -1; break; };
            }
            if (i != -1) return false;

            return true;
        }

        public static bool ValidateBuildAction()
        {
            return true;
        }
    }

    public static TaskSystem Instance;

    public SimplePriorityQueue<Task> taskQ;

    public TaskSystem()
    {
        Instance = this;
        taskQ = new SimplePriorityQueue<Task>();
    }

    public Task RequestNextTask()
    {
        if (taskQ.Count > 0)
        {
            return taskQ.Dequeue();
        }
        return null;
    }

    public void AddNewTask(Task task, int priority = -1)
    {
        taskQ.Enqueue(task, priority < 0 ? 5 : priority);
    }
}
