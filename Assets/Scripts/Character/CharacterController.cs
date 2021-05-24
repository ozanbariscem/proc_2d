using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MoonSharp.Interpreter;

public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance;

    public GameObject humanPrefab;
    public List<CharacterAI> AIs;
    CharacterAI selectedAI;
    Map map;

    public void Awake()
    {
        Instance = this;
    }

    public void Setup(TaskSystem taskSystem)
    {
        AIs = new List<CharacterAI>();
        map = GameObject.Find("Map").GetComponent<MapGenerator>().map;

        // "Create the actual character and AI"
        Test(taskSystem);
    }

    void Test(TaskSystem taskSystem)
    {
        Debug.Log("Testing code here.");

        for (int i = 0; i < 50; i++)
        {
            Character player = new Character();
            player.onCell = map.GetCell(0, 5);
            player.transform = GameObject.Instantiate(humanPrefab).transform;
            player.transform.position = player.onCell.TruePosition;
            CharacterAI ai = new CharacterAI(player, taskSystem);
            AIs.Add(ai);

        }
        selectedAI = AIs[0];
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Testing code here.");
                Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Cell targetCell = map.GetCell((int)clickPos.x, (int)clickPos.y);

                if (targetCell != null)
                {
                    if (targetCell.PathfindingCost == 0)
                    {
                        targetCell = null;
                    }
                    else
                    {
                        TaskSystem.Task.MoveToCell task = new TaskSystem.Task.MoveToCell { targetCell = targetCell };
                        selectedAI.AddForceTask(task);
                    }
                }
                
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < AIs.Count; i++)
        {
            AIs[i].Update();
        }
    }
}
