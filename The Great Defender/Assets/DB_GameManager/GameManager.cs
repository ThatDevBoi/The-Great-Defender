using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int score;

    public GameObject PC_Prefab;
    public GameObject Abducter_Prefab;


    public static GameManager s_GM;


    private void Awake()
    {
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
        Debug.Log(score);
    }



    void InitialiseGame()
    {
        CreatePlayer();
    }



    public static void CreatePlayer()
    {
        Instantiate(s_GM.PC_Prefab, Vector3.zero, Quaternion.identity);     // Spawns the player prefab
    }


// Add Points Reciver
void ScorePoints(int AddPoints)
    {
        score += AddPoints;
    }
}
