// Help used for this code
// Links https://answers.unity.com/questions/1420281/wait-for-seconds-instantiate-inside-for-loop.html <--Helped with Spawning Enemies with delay
// https://www.youtube.com/watch?v=r8N6J79W0go <-- Helped with wave system
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Wave Spawner Logic 
    public static int enemy_Count;    // Used for how many NPCs will be spawned in a for loop         // MAKE THIS DECREASE WHEN AN NPC_ABDUCTER DIES
    public int Enemies = 15;
    public float spawnDelay;    // How long it takes to spawn Abducters
    //public float startWait;
    public float waveWait;
    public float time_between_waves = 5f;
    public float waveCountdown;
    public enum SpawnState { SPAWNING, WAITING, COUNTING };
    public SpawnState state = SpawnState.COUNTING;


    public static int Player_Lives = 3;

    public bool Player_Dead = false;
    public bool Startgame;
    public float RespawnTimer = 3f;

    public float spawn_Effect_timer = 2f;


    public int score = 1;    // Monitors score for turbo charge
    private int nextWave = 1;   // Holds value of the next wave to be spawned
    public int Score_Board = 0;
    private int MonitorScore = 0;

    // NPC Prefabs
    public GameObject PC_Prefab;    // Player Character Prefab Reference 
    public GameObject human_Prefab; // Humanoid Character Prefab Reference 
    public GameObject abducter_Prefab;  // Abducter Character Prefab Reference 
    public GameObject ufo_Prefab;   // Flying sorser Character Prefab Reference 
    public GameObject spawning_effect;  // References to particele effect used to spawn enemy

    // Singleton GameManager
    public static GameManager s_GM;

    public int NPC_Human_Count;

    
    public float Abducter_Monitor_Timer = 1;    // Used to monitor how many NPC abdcuters are left
    public float UFO_spawn_Timer = 40;  // Timer that when = 0 destroys the current Active UFO
    // UI
    public Text WaveText;   // Text UI reference Componenet
    public Text score_Text;
    public GameObject Life_01;
    public GameObject Life_02;
    public GameObject Life_03;
    public GameObject StartMenu;
    public GameObject GameOver;
    public GameObject inGame_UI;

    public float Respawn_Human_Timer = 30f;

    private void Awake()
    {
        // Singlton
        if(s_GM == null)
        {
            s_GM = this;
        }
        else if(s_GM != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inGame_UI.SetActive(false);
        GameOver.SetActive(false);

        Player_Lives = 3;

        if (Player_Lives <= 3)
        {
            // Turn on Lifes
            Life_01.SetActive(true);
            Life_02.SetActive(true);
            Life_03.SetActive(true);
        }

        Time.timeScale = 0.0f;
       

        InitialiseGame();

        waveCountdown = time_between_waves;
    }

    void Update()
    {
        //Player Lifes
        if(Player_Dead)
        {
            if (Player_Lives == 2)
            {
                Life_03.SetActive(false);
                RespawnTimer -= Time.deltaTime;
                if(RespawnTimer <= 0)
                {
                    RespawnTimer = 3;
                    CreatePlayer();
                    Player_Dead = false;
                }
            } 
        }

        if(Player_Dead)
        {
            if (Player_Lives == 1)
            {
                Life_02.SetActive(false);
                RespawnTimer -= Time.deltaTime;
                if (RespawnTimer <= 0)
                {
                    RespawnTimer = 3;
                    CreatePlayer();
                    Player_Dead = false;
                }
            }
        }

        if (Player_Dead)
        {
            if (Player_Lives == 0)
            {
                Life_01.SetActive(false);
                RespawnTimer -= Time.deltaTime;
                if (RespawnTimer <= 0)
                {
                    RespawnTimer = 3;
                    CreatePlayer();
                    Player_Dead = false;
                }
            }
        }

        if (Player_Lives == -1 || NPC_Human_Count <= 0)
        {
            GameOver.SetActive(true);
            Player_Dead = true;
        }


        Respawn_Human_Timer -= Time.deltaTime;
        if(Respawn_Human_Timer <= 0)
        {
            Respawn_Human_Timer = 30f;
            CreateNPCHuman();
        }

        if(state == SpawnState.WAITING)
        {
            if (!EnemyisAlive())
            {
                WaveCompleted();
            }
            else
                return;
        }
        waveCountdown -= Time.deltaTime;
        if(waveCountdown <= 0)
        {
            if(state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave());
            }
        }

        WaveText.text = "Wave:" + nextWave.ToString();  // Display int on Text Componenet
        score_Text.text = "Score:"  + Score_Board.ToString();

        UFO_spawn_Timer -= Time.deltaTime;  // Decrease Float Value
        if(UFO_spawn_Timer <= 0)
        {
            UFO_spawn_Timer = 40;
            Instantiate(ufo_Prefab, RandomScreenPosition, Quaternion.identity);
        }
    }

    public void RestartScene(string nameofscene)
    {
        SceneManager.LoadSceneAsync(nameofscene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void WaveCompleted()
    {
        Debug.Log("Wave Completed");
        state = SpawnState.COUNTING;
        waveCountdown = time_between_waves;
        nextWave++;
    }

    public void StartGame()
    {
        inGame_UI.SetActive(true);
        Time.timeScale = 1.0f;
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

        // Calculate how many Human Prefabs get spawned
        for(int i =0; i < 10; i++)
        {
            CreateNPCHuman();
        }
    }

    public void CreateNPCHuman()
    {
        GameObject human = Instantiate(s_GM.human_Prefab, HumanRandomScreenPosition, Quaternion.identity); NPC_Human_Count++; // Spawns 10 Humans within the HumanRandomScreenPosition
    }

    public static void CreatePlayer()
    {
        Instantiate(s_GM.PC_Prefab, Vector3.zero, Quaternion.identity);     // Spawns the player prefab
    }

   public static Vector3  RandomScreenPosition
    {
        get
            {
            float yscreenpos_Fixed = 5f;     // y restriction value
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

    void Leader_Board_Score(int AddPoints)
    {
        Score_Board += AddPoints;
        MonitorScore += AddPoints;
    }

    IEnumerator SpawnWave()
    {  
        state = SpawnState.SPAWNING;
        for (int i = 0; i < Enemies; i++)      // Let Computer Calculate how many Enemies it will spawn
          {
            GameObject GO_Spawn = Instantiate(s_GM.spawning_effect, RandomScreenPosition, Quaternion.identity);
            Destroy(GO_Spawn, 3);
            yield return new WaitForSeconds(1 / spawnDelay);    // Wait For Seconds and Divid 1 by spawnDelay
            Instantiate(s_GM.abducter_Prefab, GO_Spawn.transform.position, Quaternion.identity); enemy_Count++;
           
            if (!EnemyisAlive())
            {
                enemy_Count += enemy_Count + 5;
                nextWave++;
            }
                yield return new WaitForSeconds(waveWait);
        }
            state = SpawnState.WAITING;
            yield break;
        
        
    }
}
