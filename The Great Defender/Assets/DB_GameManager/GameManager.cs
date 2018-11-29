// Help used for this code
// Links https://answers.unity.com/questions/1420281/wait-for-seconds-instantiate-inside-for-loop.html <--Helped with Spawning Enemies with delay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int score = 1;    // Monitors score for turbo charge
    private int nextWave = 1;   // Holds value of the next wave to be spawned
    public int enemy_Count = 0;    // Used for how many NPCs will be spawned in a for loop         // MAKE THIS DECREASE WHEN AN NPC_ABDUCTER DIES

    // NPC Prefabs
    public GameObject PC_Prefab;    // Player Character Prefab Reference 
    public GameObject human_Prefab; // Humanoid Character Prefab Reference 
    public GameObject abducter_Prefab;  // Abducter Character Prefab Reference 
    public GameObject ufo_Prefab;   // Flying sorser Character Prefab Reference 

    public GameObject spawning_Effect_Prefab;

    // Singleton GameManager
    public static GameManager s_GM;

    public static int NPC_Human_Count;

    
    public float spawnDelay = 2;    // How long it takes to spawn Abducters
    public float Abducter_Monitor_Timer = 1;    // Used to monitor how many NPC abdcuters are left
    public float UFO_spawn_Timer = 40;  // Timer that when = 0 destroys the current Active UFO

    public Text WaveText;   // Text UI reference Componenet


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
        WaveText = GameObject.FindGameObjectWithTag("Wave_Text").GetComponent<Text>();     // Find the Text GameObject
    }

    private void Update()
    {
        Debug.Log(enemy_Count);
        Debug.Log(nextWave);
        Debug.Log(EnemyisAlive());
        EnemyisAlive();
        if(Abducter_Monitor_Timer<= 0)
        {
            if (enemy_Count <= 0)
            {
                //nextWave++;
            }
        }
        WaveText.text = "Wave:" + nextWave.ToString();  // Display int on Text Componenet
        UFO_spawn_Timer -= Time.deltaTime;  // Decrease Float Value
        if(UFO_spawn_Timer <= 0)
        {
            UFO_spawn_Timer = 40;
            Instantiate(ufo_Prefab, RandomScreenPosition, Quaternion.identity);
        }

        if(NPC_Human_Count <= 0)
        {
            // Activate a UI Restart or Quit Button
        }
    }
    


    bool EnemyisAlive()
    {
        s_GM.Abducter_Monitor_Timer -= Time.deltaTime;  // Decreases float value
        // If the Timer is more than or equal to 0 start to search for the GameObject
        if (Abducter_Monitor_Timer <= 0)
            Abducter_Monitor_Timer = 1f;
        if(GameObject.FindGameObjectWithTag("NPC_Abducter") == null)
        {
            return false;   // Return bool false if no enemies are found
        }
        return true;        // Enemies are found and are still active in the scene
    }

    void InitialiseGame()
    {
        // Create Player Function
        CreatePlayer();
        EnemyisAlive();
        StartCoroutine(SpawnWave());
        // Calculate how many Human Prefabs get spawned
        for(int i =0; i < 10; i++)
        {
            GameObject human = Instantiate(s_GM.human_Prefab, HumanRandomScreenPosition, Quaternion.identity); // Spawns 10 Humans within the HumanRandomScreenPosition
        }

        // Spawn UI or turn it on
        // Spawn Set Amount of humans near the ground layer

    }

    public static void CreateNPCAbductor()
    {
        Instantiate(s_GM.abducter_Prefab, RandomScreenPosition, Quaternion.identity); s_GM.enemy_Count++;    // Spawns the player prefab and adds a static int to monitor count of enemy
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
            float xscreenpos_Fixed = 50f;      // x screen wrap values
            return new Vector3(Random.Range(-xscreenpos_Fixed, xscreenpos_Fixed), Random.Range(-yscreenpos_Fixed, yscreenpos_Fixed), 0.0f);     // Retrun the values as a new Vector3 within a random position on the negetive and positive float positions
        }
   }

    public static Vector3 HumanRandomScreenPosition
    {
        get
        {
            float yscreenpos = -7.4f;  // y spawn value positions
            float xscreenpos = 50f;         // x spawn values positions
            return new Vector3(Random.Range(-xscreenpos, xscreenpos), Random.Range(yscreenpos, yscreenpos), 0.0f);
        }
    }

    // Add Points Reciver
    void ScorePoints(int AddPoints)
    {
        score += AddPoints;
    }

    IEnumerator SpawnWave()
    {
        for(int i =0; i < 10; i++)      // Let Computer Calculate how many Enemies it will spawn
        {
            yield return new WaitForSeconds(1 / spawnDelay);    // Wait For Seconds and Divid 1 by spawnDelay
            CreateNPCAbductor();        // After Seconds have been over spawn NPC
            NPC_Human_Count++;
            if (!EnemyisAlive())
            {
                nextWave++;
            }
        }
        yield break;
    }
}
