using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class CharacterAI
{
    private enum State
    {
        Idle, Executing
    }

    public static readonly float AITaskCheckInterval = 1f;

    TaskSystem taskSystem;
    public TaskSystem.Task task;
    public Character character;
    State state;
    float lastCheckTime;

    public CharacterAI(Character character, TaskSystem taskSystem)
    {
        this.taskSystem = taskSystem;
        this.character = character;
        this.character.AI = this;
        this.state = State.Idle;
    }

    public void Update()
    {
        switch (state)
        {
            case State.Idle:
                if (Time.time - lastCheckTime >= AITaskCheckInterval)
                {
                    lastCheckTime = Time.time;
                    RequestNextTask(0);
                }
                break;
            case State.Executing:
                ExecuteTask();
                break;
        }
    }

    public void AddForceTask(TaskSystem.Task task)
    {
        this.character.path = null;
        this.task = task;
        this.state = State.Executing;
    }

    private void RequestNextTask(int depth)
    {
        if (depth >= taskSystem.taskQ.Count) return;
        task = taskSystem.RequestNextTask();

        if (task is TaskSystem.Task.Dig _digTask)
        {
            if (!_digTask.validateAction(_digTask.targetCell))
            {
                AbandonTask();
                RequestNextTask(depth + 1);
            }
        }
        if (task == null)
        {
            this.character.path = null;
            state = State.Idle;
        }
        else
        {
            this.character.path = null;
            state = State.Executing;
        }
    }

    private void ExecuteTask()
    {
        if (task == null) return;

        if (task is TaskSystem.Task.MoveToCell)
        {
            if (Execute_MoveToCellTask(task as TaskSystem.Task.MoveToCell))
            {
                TaskCompleted();
            }
        }
        else if (task is TaskSystem.Task.ChopTree _chopTask)
        {
            if (_chopTask.validateAction(character, _chopTask.targetCell))
            {
                if (Execute_ChopTreeTask(_chopTask))
                {
                    TaskCompleted();
                }
            }
            else
            {
                AbandonTask();
            }
        }
        else if(task is TaskSystem.Task.Dig _digTask)
        {
            //Debug.Log(task.state.ToString());
            if (_digTask.validateAction(_digTask.targetCell))
            {
                if (Execute_DigTask(_digTask))
                {
                    TaskCompleted();
                }
            }
            else
            {
                AbandonTask();
            }
        }
        else if(task is TaskSystem.Task.Haul _haulTask)
        {
            if (_haulTask.validateAction(_haulTask.item, _haulTask.container))
            {
                if (Execute_HaulTask(_haulTask))
                {
                    TaskCompleted();
                }
            }
            else
            {
                AbandonTask();
            }
        }
        else if(task is TaskSystem.Task.Build _buildTask)
        {
            if (_buildTask.validateAction())
            {
                if (Execute_BuildTask(_buildTask))
                {
                    TaskCompleted();
                }
            }
            else
            {
                AbandonTask();
            }
        }
    }

    public void TaskCompleted()
    {
        this.character.path = null;
        task = null;
        state = State.Idle;
    }

    public void AbandonTask()
    {
        TaskSystem.Instance.AddNewTask(task);
        TaskCompleted();
    }

    public void InvalidateTask()
    {
        TaskCompleted();
    }

    private bool Execute_MoveToCellTask(TaskSystem.Task.MoveToCell task)
    {
        if (task.state == TaskSystem.Task.TaskState.MoveToCell)
        {
            if (task.moveAction(this.character, task.targetCell, task.cellRange))
            {
                task.state = TaskSystem.Task.TaskState.Done;
            }
        }
        
        return task.state == TaskSystem.Task.TaskState.Done;
    }

    private bool Execute_ChopTreeTask(TaskSystem.Task.ChopTree task)
    {
        if (task.state == TaskSystem.Task.TaskState.MoveToCell)
        {
            if (task.moveAction(this.character, task.targetCell, task.cellRange))
            {
                task.state = TaskSystem.Task.TaskState.Chop;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.Chop)
        {
            if (task.chopAction(this.character, task.targetCell))
            {
                task.state = TaskSystem.Task.TaskState.Done;
            }
        }

        return task.state == TaskSystem.Task.TaskState.Done;
    }

    private bool Execute_DigTask(TaskSystem.Task.Dig task)
    {
        Debug.Log(task.state.ToString());
        if (task.state == TaskSystem.Task.TaskState.GetPath)
        {
            string tag = task.targetCell.objectOnCell.data.unique_tag;
            task.targetCell.RemoveObjectOnCell(true); // Remove the object so pathfinding cost becomes 1
            bool arrived = task.moveAction(this.character, task.targetCell, task.cellRange); // This one will create the path
            task.targetCell.AddObjectToCell(CellObjectData.CreateFromData(CellObjectData.objectDatas[tag]), true); // Add the object so pathfinding cost becomes 0

            task.state = arrived ? TaskSystem.Task.TaskState.Dig : TaskSystem.Task.TaskState.MoveToCell;
        }
        else if (task.state == TaskSystem.Task.TaskState.MoveToCell)
        {
            if (task.moveAction(this.character, task.targetCell, task.cellRange))
            {
                task.state = TaskSystem.Task.TaskState.Dig;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.Dig)
        {
            if (task.digAction(this.character, task.targetCell))
            {
                task.state = TaskSystem.Task.TaskState.Done;
            }
        }

        return task.state == TaskSystem.Task.TaskState.Done;
    }

    private bool Execute_HaulTask(TaskSystem.Task.Haul task)
    {
        if (task.state == TaskSystem.Task.TaskState.MoveToItem)
        {
            if (task.moveToItemAction(this.character, task.targetCell, task.cellRange))
            {
                task.state = TaskSystem.Task.TaskState.PickItem;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.PickItem)
        {
            if (task.pickAction(this.character, task.item))
            {
                task.state = TaskSystem.Task.TaskState.MoveToContainer;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.MoveToContainer)
        {
            if (task.moveToContainerAction(this.character, task.container.PlacedCell, 1))
            {
                task.state = TaskSystem.Task.TaskState.StoreItem;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.StoreItem)
        {
            if (task.storeAction(this.character, task.container, task.item))
            {
                task.state = TaskSystem.Task.TaskState.Done;
            }
        }

        return task.state == TaskSystem.Task.TaskState.Done;
    }

    private bool Execute_BuildTask(TaskSystem.Task.Build task)
    {
        if (task.state == TaskSystem.Task.TaskState.MoveToItem)
        {
            if (task.moveToItemAction(character, task.item.onCell, task.cellRange))
            {
                task.state = TaskSystem.Task.TaskState.PickItem;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.PickItem)
        {
            if (task.pickAction(character, task.item))
            {
                task.state = TaskSystem.Task.TaskState.MoveToCell;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.MoveToCell)
        {
            if (task.moveToCellAction(character, task.targetCell, task.cellRange))
            {
                task.state = TaskSystem.Task.TaskState.Build;
            }
        }
        else if (task.state == TaskSystem.Task.TaskState.Build)
        {
            if (task.buildAction(task, character, task.targetCell))
            {
                task.state = TaskSystem.Task.TaskState.Done;
            }
        }

        return task.state == TaskSystem.Task.TaskState.Done;
    }
}
