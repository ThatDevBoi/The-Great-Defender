// Help used for this code
// Links https://answers.unity.com/questions/1420281/wait-for-seconds-instantiate-inside-for-loop.html <--Helped with Spawning Enemies with delay
// https://www.youtube.com/watch?v=r8N6J79W0go <-- Helped with wave system
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// This Class is used to manage references in the game and certain states the game can be in. 
/// Its also in control of a WaveSpawner so it determins how many NPC abdcuters get spawned each wave
/// Without this class the game wont function or work it holds Flags that ive used in all dervied classes to let them know when something has been changed or missing
/// This Class can be emplied to an Empty GameObject and just left in the scene
/// </summary>
public class GameManager : MonoBehaviour
{
    // Ints
    public int int_Enemies = 5;        // How many Enemies will spawn depends on this value (Enemy Wave Logic)
    private int int_increase_Enemies = 5;   // Value to use so after a wave this int increases the current Enemies int (Enemy Wave Logic)
    public int int_turbo_Shot_score_monitor = 1;    // Monitors score for turbo charge use to reference in other classes and add when enemy dies
    private int int_nextWave = 1;   // Holds value of the next wave to be spawned Displayed to UI (Enemy Wave Logic)
    public int int_Score_Board = 0;     // int displayed to UI Text to display the score recived from killing NPCs
    public int int_NPC_Human_Count;     // Monitors how many Human NPCs are in the scene    Keep public its being decreased in human NPC class when the NPC dies
    public static int int_Player_Lives = 3;     // Used to monitor how many times to player has died. Also decreasing it will make Life Images SetActive false
    public static int int_enemy_Count;    // Used for how many NPCs will be spawned in a for loop  (Enemy Wave Logic)        // MAKE THIS DECREASE WHEN AN NPC_ABDUCTER DIES

    // Floats
    public float fl_spawnDelay;    // How long it takes to spawn Abducters (Enemy Wave Logic)
    public float fl_waveWait;  // (Enemy Wave Logic)
    public float fl_time_between_waves = 5f;       // (Enemy Wave Logic)
    public float fl_waveCountdown;     // (Enemy Wave Logic)

    public float fl_PC_RespawnTimer = 3f;  // When the player dies this value decreases so PC doesnt spawn back instanlty
    public float fl_Monitor_NPC_abductor_Alive = 1;    // Used to monitor how many NPC abdcuters are left
    public float fl_UFO_spawn_Timer = 40;  // Timer that when = 0 destroys the current Active UFO
    public float fl_Respawn_Human_Timer = 400f;        // Every 400 seconds a new human spawns so the game can continue

    // Bools
    public bool bl_Player_Dead = false;        // Bool to define is the player alive or dead

    // GameObjects
    public GameObject GO_PC_Prefab;    // Player Character Prefab Reference 
    public GameObject GO_human_Prefab; // Humanoid Character Prefab Reference 
    public GameObject GO_abducter_Prefab;  // Abducter Character Prefab Reference 
    public GameObject GO_ufo_Prefab;   // Flying sorser Character Prefab Reference 
    public GameObject GO_spawning_effect;  // References to particele effect used to spawn enemy
    // 3 GameObjects list of lifes on the UI Canvas
    public GameObject GO_Life_01;  // (used on UI)
    public GameObject GO_Life_02;  // (used on UI)
    public GameObject GO_Life_03;  // (used on UI)
    public GameObject GO_StartMenu;    // Reference to the StartMenu GameObject on UI Canvast (used on UI)
    public GameObject GO_GameOver; // Reference to the GameOver GameObject on UI Canvas (used on UI)
    public GameObject GO_inGame_UI;    //Reference to the inGame GameObject on UI Canvas (used on UI)
    public GameObject GO_Charge_Shot_Slider;   // Reference to the Charge Shot GameObject on UI Canvas (used on UI)
    public GameObject GO_PC_health_HUD;    // Reference to the Health HUD GameObject on UI Canvas (used on UI)

    // other
    private enum SpawnState { SPAWNING, WAITING, COUNTING };     // different states for the wave spawner to be in so there are delays and flags (Enemy Wave Logic)
    private SpawnState state = SpawnState.COUNTING;      // Spawnstate always starts countind so enemies dont spawn straight away (Enemy Wave Logic)
    public static GameManager s_GM;     // Singleton GameManager

    // UI
    public Text UI_WaveText;   // Text UI reference Componenet which will display the current wave
    public Text UI_score_Text;  // Text Ui reference component which will display the players score 

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
        // Call Functions
        InitialiseGame();

        GO_inGame_UI.SetActive(false);  // On first run of script Turn off ingame UI GameObject
        GO_GameOver.SetActive(false);   // Turn off GameOver GameObject in UI Canvas on Start

        int_Player_Lives = 3;           // Start with 3 lives each time
        if (int_Player_Lives <= 3)      // if the Players lifes are greater or equal to 3
        {
            // Turn on Lifes
            GO_Life_01.SetActive(true);
            GO_Life_02.SetActive(true);
            GO_Life_03.SetActive(true);
        }
        Time.timeScale = 0.0f;      // TimeScale is 0 so everything is paused
        fl_waveCountdown = fl_time_between_waves;   // The waveCountdown value will always equal time Between Waves value
    }

    void Update()
    {
        //Player Lifes
        if(bl_Player_Dead)      // if the player is dead
        {
            if (int_Player_Lives == 2)  // if player lives equals 2
            {
                GO_Life_03.SetActive(false);    // turn off first life
                fl_PC_RespawnTimer -= Time.deltaTime;   // cue the players respawn timer to decrease
                if(fl_PC_RespawnTimer <= 0)     // when the players respawn timer value = 0 or greater
                {
                    fl_PC_RespawnTimer = 3;     // reset the respawn value
                    CreatePlayer();             // Call create Player Function
                    bl_Player_Dead = false;     // boolean flag of PC Dead is no longer true
                }
            } 
        }

        if(bl_Player_Dead)
        {
            if (int_Player_Lives == 1)
            {
                GO_Life_02.SetActive(false);
                fl_PC_RespawnTimer -= Time.deltaTime;
                if (fl_PC_RespawnTimer <= 0)
                {
                    fl_PC_RespawnTimer = 3;
                    CreatePlayer();
                    bl_Player_Dead = false;
                }
            }
        }

        if (bl_Player_Dead)
        {
            if (int_Player_Lives == 0)
            {
                GO_Life_01.SetActive(false);
                fl_PC_RespawnTimer -= Time.deltaTime;
                if (fl_PC_RespawnTimer <= 0)
                {
                    fl_PC_RespawnTimer = 3;
                    CreatePlayer();
                    bl_Player_Dead = false;
                }
            }
        }

        if (int_Player_Lives == -1 || int_NPC_Human_Count <= 0)     // When the Players lives = -1 or Humans count int = 0
        {
            // Turn off irrelevant GameObjects and que end screen
            GO_GameOver.SetActive(true);
            GO_inGame_UI.SetActive(false);
            GO_Charge_Shot_Slider.SetActive(true);
            GO_PC_health_HUD.SetActive(false);
            bl_Player_Dead = true;
        }


        fl_Respawn_Human_Timer -= Time.deltaTime;   // Decrease this value overtime
        if(fl_Respawn_Human_Timer <= 0)             // when the Humans respawn timer = 0 or greater
        {
            fl_Respawn_Human_Timer = 400f;          // reset the timer
            CreateNPCHuman();                       // Make Another Human NPC
        }

        if(state == SpawnState.WAITING)     // if the spawnstate is waiting 
        {
            if (!EnemyisAlive())            // if the enemyAlive Boolean is false 
            {
                WaveCompleted();            // Player Has Completed A Wave
            }
            else   // however if not true
                return;         // return until completed
        }
        fl_waveCountdown -= Time.deltaTime;     // Start to decline the waveCountdown for Spawning a wave of enemies
        if(fl_waveCountdown <= 0)               // When the waveCountdown is more than or equal to 0
        {
            if(state != SpawnState.SPAWNING)        // and if the Spawnstate is not in the state of Spawning enemies
            {
                StartCoroutine(SpawnWave());        // Then start Coroutine to spawn the enemies
            }
        }

        UI_WaveText.text = "Wave:" + int_nextWave.ToString();       // Making sure UI Text for WaveText prints wave and shows the int of wave value
        UI_score_Text.text = "Score:"  + int_Score_Board.ToString();    // Making sure UI Text for Score prints score and shows int value

        fl_UFO_spawn_Timer -= Time.deltaTime;       // Decline UFO Spawn Timer 

        if(fl_UFO_spawn_Timer <= 0)     // if the value of UFO Spawn Timer is more than or equal to 0
        {
            StartCoroutine(SpawnUFO());     // Start Couroutine to spawn another UFO
        }

    }
    // Void Restarts the scene
    public void RestartScene(string nameofscene)
    {
        SceneManager.LoadSceneAsync(nameofscene);
    }
    // void that Lets players quit the game when pressing a button
    public void QuitGame()
    {
        Application.Quit();
    }

    public void WaveCompleted()
    {
        Debug.Log("Wave Completed");    // Show wave is completed in console
        state = SpawnState.COUNTING;    // Spawnstate is njow Counting again for next wave to begin
        fl_waveCountdown = fl_time_between_waves;   // make sure the waveCountdown is Time Between Wave float
        int_nextWave++;     // increase the wave int so wave 1 goes onto wave 2 ect
    }
    // Void that turns on GameObjects needed in game and turns paused time fior start Function On
    public void StartGame()
    {
        GO_inGame_UI.SetActive(true);
        GO_Charge_Shot_Slider.SetActive(true);
        GO_PC_health_HUD.SetActive(true);
        Time.timeScale = 1.0f;
    }
    // Boolean used to check if all enemies are dead
    bool EnemyisAlive()
    {
        s_GM.fl_Monitor_NPC_abductor_Alive -= Time.deltaTime;  // Decreases float value
        // If the Timer is more than or equal to 0 start to search for the GameObject
        if (fl_Monitor_NPC_abductor_Alive <= 0)
            fl_Monitor_NPC_abductor_Alive = 1f; // reset the value
        if(GameObject.FindGameObjectWithTag("NPC_Abducter") == null)    // if no GameObjects are found with the NPC Abductor Tag
        {
            int_Enemies += int_increase_Enemies;    // increase how many Enemies can be spawned and goes up per wave [Wave: 01 = 10 Enemies, Wave: 02 = 15 Enemies] Makes for endless waves
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
        GameObject human = Instantiate(s_GM.GO_human_Prefab, HumanRandomScreenPosition, Quaternion.identity); int_NPC_Human_Count++; // Spawns 10 Humans within the HumanRandomScreenPosition
    }

    public static void CreatePlayer()
    {
       GameObject pc =  Instantiate(s_GM.GO_PC_Prefab, Vector3.zero, Quaternion.identity);     // Spawns the player prefab
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

    // Add Points Reciver for turbo shot used by the player
    void ScorePoints(int AddPoints)
    {
        int_turbo_Shot_score_monitor += AddPoints;
    }
    // Reciver to add points to the ui Text of how much the player has scored by killing and saving NPCs
    void Leader_Board_Score(int AddPoints)
    {
        int_Score_Board += AddPoints;
    }
    // Used to spawn the Abductors
    IEnumerator SpawnWave()
    {  
        state = SpawnState.SPAWNING;        // Spawnstate is Spawning A Enemy Wave Will Spawn
        for (int i = 0; i < int_Enemies; i++)      // Let Computer Calculate how many Enemies it will spawn
        {
            GameObject GO_Spawn = Instantiate(s_GM.GO_spawning_effect, RandomScreenPosition, Quaternion.identity);  // Start by Spawning a Spawn effect so player knows where to NPC is going to spawn
            Destroy(GO_Spawn, 3);       // Destroy this effect after 3 seconds
            yield return new WaitForSeconds(1 / fl_spawnDelay);    // Wait For Seconds and Divid 1 by spawnDelay
            Instantiate(s_GM.GO_abducter_Prefab, GO_Spawn.transform.position, Quaternion.identity); int_enemy_Count++;  // Allow NPC Abductor to spawn on the spawn Effect postion before its recyled
            yield return new WaitForSeconds(fl_waveWait);
        }
            state = SpawnState.WAITING;     // Spawnstate is now Waiting for countdown or Spawning to start
            yield break;
    }

    IEnumerator SpawnUFO()
    {
        fl_UFO_spawn_Timer = 40;        // Make sure the UFO SpawnTimer starts at 40 each time
        GameObject UFO_Spawn_pos = Instantiate(s_GM.GO_spawning_effect, RandomScreenPosition, Quaternion.identity); // Spawn Spawing effect so player knows where the NPC will be (So its not unfair for the player to randomyl die from a randomly spawned enemy)
        Destroy(UFO_Spawn_pos, 2);      // recyle spawn effect in 2 Seconds
        yield return new WaitForSeconds(1);     // Wait 1 second
        Instantiate(s_GM.GO_ufo_Prefab, UFO_Spawn_pos.transform.position, Quaternion.identity);     // Spawn the NPC UFO in the spawn effects position before its recyled
        
        yield break;
    }
}
