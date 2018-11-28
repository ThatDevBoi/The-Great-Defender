﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NPC_Critter : Base_Class
{
    [SerializeField]
    private int ScoreBoardPoints = 50;
    [SerializeField]
    private GameObject FlickingTextMesh;
    public float deductSpeed = 2;
    public Transform Player;
    private PC_Space_Ship_Controller PC_script;
    [SerializeField]
    private Slider ChargeBar;

    // Use this for initialization
    protected virtual void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        PC_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PC_Space_Ship_Controller>();        // Finding the Player Derived Script
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Functions
        DoMove();
        Movement_Restriction();
    }

    protected override void DoMove()
    {
        mvelocity = transform.position = Vector3.MoveTowards(transform.position, Player.position, speed * Time.deltaTime);
    }

    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            gameObject.transform.parent = Player.transform;
            PC_script.GetComponent<PC_Space_Ship_Controller>().speed -= deductSpeed;        // decreases players orgianl when it is a child
        }

        if(other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);

            GameObject TextMeshGO = Instantiate(FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PC_script.GetComponent<PC_Space_Ship_Controller>().speed += deductSpeed;        // adds players orgianl speed back when the NPC is no longer a child
        }
    }
}