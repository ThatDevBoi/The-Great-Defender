using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int score;


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

    private void Update()
    {
        Debug.Log(score);
    }


    void ScorePoints(int AddPoints)
    {
        score += AddPoints;
    }
}
