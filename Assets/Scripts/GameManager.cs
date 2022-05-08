using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Gameplay Loop")]
    public bool isGameRunning = false;

    [Header("Race Variables")]
    public int lapsToFinish;
        
    [Header("UI")]
    public GameObject startButton;
    public TMP_Text lapCount;
    public TMP_Text Winner;
    public GameObject WinnerPanel;
    public GameObject Drift;

    [Header("CARS")]
    public List<NPC_AI> NPCScripts = new List<NPC_AI>();
    public vehicleController player;

    [Header("AUDIO CONTROLLER")]
    public audioManager audioManager;

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

        // not have music start immediately
        audioManager.StopRaceMusic();

        WinnerPanel.SetActive(false);

        Drift.SetActive(false);
    }

    void Update()
    {
        //debug help
        if(Input.GetKeyDown(KeyCode.M))
        {
            audioManager.StopRaceMusic();
        }

        // Track who won!
        //Player check
        if (player.lap == lapsToFinish)
        {
            isGameRunning = false;

            print(player.ToString() + " is the winner!!");

            // truncate name and display winner
            string message;
            message = player.ToString();
            message = message.Replace("(vehicleController)", "");

            Winner.text = message + "is the winner!!";
            WinnerPanel.SetActive(true);
        }

        // NPC check
        for (int i = 0; i < NPCScripts.Count; ++i)
        {
            if (NPCScripts[i].lap == lapsToFinish)
            {
                isGameRunning = false;
                
                print(NPCScripts[i].ToString() + " is the winner!!");

                //Show winner on Panel
                string message;
                message = NPCScripts[i].ToString();
                message = message.Replace("(NPC_AI)", "");

                Winner.text = message + "is the winner!!";
                WinnerPanel.SetActive(true);
            }
        }

        // update lap count text!
        lapCount.text = "Lap: " + player.lap.ToString();
    }

    public void OnStartButton()
    {
        isGameRunning = true;

        startButton.SetActive(false);

        // start music at 1/2 volume
        audioManager.PlayRaceMusic(0.1f);
    }

    public void OnPauseButton()
    {
        isGameRunning = false;

        startButton.SetActive(true);
    }

    //public void DriftButton()
    //{
    //
    //}
}
