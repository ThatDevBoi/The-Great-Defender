using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int score;

    public GameObject PC_Prefab;
    public GameObject Abducter_Prefab;

    public GameObject spawn_point;


    public static GameManager s_GM;


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
        Debug.Log(score);       // Figure out the score
    }



    void InitialiseGame()
    {
        int i = 0;
        CreatePlayer();

        for (i = 0; i < 20; i++)        // Calculates how many NPC abducter Prefabs are going to be spawned
            CreateNPCAbductor();        // Call Function to create NPC abducter 

    }



    public static void CreatePlayer()
    {
        Instantiate(s_GM.PC_Prefab, Vector3.zero, Quaternion.identity);     // Spawns the player prefab
    }

   public static Vector3  RandomScreenPosition
    {
        //Instantiate(s_GM.Abducter_Prefab, new Vector3())
        get
            {
            float yscreenpos_Fixed = 7.2f;     // y restriction value
            float xscreenpos_Fixed = 70f;      // x screen wrap values
            return new Vector3(Random.Range(-xscreenpos_Fixed, xscreenpos_Fixed), Random.Range(-yscreenpos_Fixed, yscreenpos_Fixed), 0.0f);     // Retrun the values as a new Vector3 within a random position on the negetive and positive float positions
        }
    }
    public static void CreateNPCAbductor()
    {
        Instantiate(s_GM.Abducter_Prefab, RandomScreenPosition, Quaternion.identity);     // Spawns the player prefab
    }
    // Add Points Reciver
    void ScorePoints(int AddPoints)
    {
        score += AddPoints;
    }
}
