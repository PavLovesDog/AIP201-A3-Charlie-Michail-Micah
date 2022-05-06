using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Gameplay Loop")]
    public bool isGameRunning = false;

    [Header("Race Variables")]
    public int lapsToFinish;

    [Header("UI")]
    public GameObject startButton;

    [Header("CARS")]
    public List<NPC_AI> NPCScripts = new List<NPC_AI>();
    public vehicleController player;

    void Start()
    {
        isGameRunning = false;

        // detect NPCS
        GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");

        // add those items to list
        foreach (GameObject npc in NPCs)
        {
            NPCScripts.Add(npc.GetComponent<NPC_AI>());
        }
    }

    void Update()
    {
        // Track who won!
        //Player check
        if (player.lap == lapsToFinish)
        {
            isGameRunning = false;

            print(player.ToString() + " is the winner!!");

        }

        // NPC check
        for (int i = 0; i < NPCScripts.Count; ++i)
        {
            if (NPCScripts[i].lap == lapsToFinish)
            {
                isGameRunning = false;

                print(NPCScripts[i].ToString() + " is the winner!!");
            }
        }

        //Track who is in the lead?

    }

    public void OnStartButton()
    {
        isGameRunning = true;

        startButton.SetActive(false);
    }
}
