// Help used for this code
// Links https://answers.unity.com/questions/1420281/wait-for-seconds-instantiate-inside-for-loop.html <--Helped with Spawning Enemies with delay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int score;    // Monitors score for turbo charge

    public GameObject PC_Prefab;    // Player Character Prefab Reference 
    public GameObject human_Prefab; // Humanoid Character Prefab Reference 
    public GameObject abducter_Prefab;  // Abducter Character Prefab Reference 
    public GameObject ufo_Prefab;   // Flying sorser Character Prefab Reference 
    public static GameManager s_GM;

    
    public float spawnDelay = 2;    // How long it takes to spawn Abducters
    public float UFO_spawn_Timer = 40;  // Timer that when = 0 destroys the current Active UFO


    private void Awake()
    {
        // Singlton
        if(s_GM == null)
        {
            s_GM = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(s_GM != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitialiseGame();
    }

    private void Update()
    {
        Debug.Log(score);       // Figure out the score of turbo shot
        UFO_spawn_Timer -= Time.deltaTime;  // Decrease Float Value
        if(UFO_spawn_Timer <= 0)
        {
            UFO_spawn_Timer = 40;
            Instantiate(ufo_Prefab, RandomScreenPosition, Quaternion.identity);
        }
    }



    void InitialiseGame()
    {
        // Create Player Function
        CreatePlayer();
        StartCoroutine(SpawnWave());

        // Spawn UI or turn it on

        // Spawn Set Amount of humans near the ground layer

    }

    public static void CreateNPCAbductor()
    {
        Instantiate(s_GM.abducter_Prefab, RandomScreenPosition, Quaternion.identity);     // Spawns the player prefab
    }

    public static void CreatePlayer()
    {
        Instantiate(s_GM.PC_Prefab, Vector3.zero, Quaternion.identity);     // Spawns the player prefab
    }

   public static Vector3  RandomScreenPosition
    {
        get
            {
            float yscreenpos_Fixed = 7.2f;     // y restriction value
            float xscreenpos_Fixed = 70f;      // x screen wrap values
            return new Vector3(Random.Range(-xscreenpos_Fixed, xscreenpos_Fixed), Random.Range(-yscreenpos_Fixed, yscreenpos_Fixed), 0.0f);     // Retrun the values as a new Vector3 within a random position on the negetive and positive float positions
        }
    }

    // Add Points Reciver
    void ScorePoints(int AddPoints)
    {
        score += AddPoints;
    }


    IEnumerator SpawnWave()
    {
        for(int i =0; i < 20; i++)      // Let Computer Calculate how many Enemies it will spawn
        {
            yield return new WaitForSeconds(1 / spawnDelay);    // Wait For Seconds and Divid 1 by spawnDelay
            CreateNPCAbductor();        // After Seconds have been over spawn NPC
        }
        yield break;
    }
}
