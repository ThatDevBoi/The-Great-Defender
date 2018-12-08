using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This Dervied Class when a Chaser Dies allows to gameObject to follow the player and when within range it can child itself onto the player and slow it down by a declared value
/// It Doesnt Screenwrap only when childed onto the player
/// This Dervied class needs to be put on a GameObject with the Tag Critter
/// </summary>
public class NPC_Critter : Base_Class
{
    [SerializeField]
    private int int_ScoreBoardPoints = 50;          // Int that will be shown on the flickering text. 
    [SerializeField]
    private GameObject GO_FlickingTextMesh;        // GameObject of animated TextMesh
    public float fl_deductSpeed = 0.5f;        // Speed that the Critter Prefab will take away from the player when its been childed to it
    private Transform IDE_trans_Player;        // Reference to the players Transform
    private PC_Space_Ship_Controller PC_script;     // Player Character Script Rference

    // Use this for initialization
    void Start ()
    {
        base.Start();   // Bass class start function
        IDE_PC_BC.isTrigger = true;     // BoxCollider will be a trigger
        IDE_trans_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find the Players Transform
        PC_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PC_Space_Ship_Controller>();        // Find the players script
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Functions
        DoMove();
        Movement_Restriction();

        if (IDE_trans_Player == null && GameManager.s_GM.bl_Player_Dead == false)     // if there is no Player Transform component and the player isnt dead
        {
            IDE_trans_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // find the transform componenet
        }
        else if (IDE_trans_Player != null && GameManager.s_GM.bl_Player_Dead == true)     // however if we have the transform componenet and the player is dead
            return;     // return information back to if statement

        if (PC_script == null && GameManager.s_GM.bl_Player_Dead == false)      // if there is no PC Script reference and the player isnt dead
            PC_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PC_Space_Ship_Controller>();        // Finding the Player Derived Script
        else if (PC_script != null && GameManager.s_GM.bl_Player_Dead == true)  // However if we have the script reference and the player is dead
            return;         // return back to if 




    }

    protected override void DoMove()
    {
        if (IDE_trans_Player == null && GameManager.s_GM.bl_Player_Dead == false) // if the player reference is not found and the player is alive
            IDE_trans_Player = GameObject.Find("PC(Clone)").GetComponent<Transform>();    // Find the player in the scene
        transform.position += mvelocity * Time.deltaTime;   // the transform of the object moves with Vector3 from base class with time
        if (IDE_trans_Player != null)     // if the Player Transform is found
        {
            // gameObject transform moves towards the players position with speed and time
            transform.position = Vector3.MoveTowards(transform.position, IDE_trans_Player.position, fl_movement_speed * Time.deltaTime);
        }
        // Allow console to show we have a transform
        if (IDE_trans_Player != null) 
        {
            Debug.Log("Chaser Has PC Transform");
            return;
        }
    }

    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")        // if we collid with the player with a trigger collider
        {
            gameObject.transform.parent = IDE_trans_Player.transform;     // gameObjects childs onto the players moves with the player gaining its transform
            PC_script.GetComponent<PC_Space_Ship_Controller>().fl_movement_speed -= fl_deductSpeed;        // decreases players orgianl when it is a child
        }
        else if(other.gameObject.tag == "Player" == null)   // However if the gameObject collids with a tag that isnt player
        {
            gameObject.transform.parent = null;         // gameObject has no parent
            PC_script = null;       // which also means the PC Script is nothing
        }

        if(other.gameObject.tag == "Bullet")        // if the gameObject is hit by a GameObject called Bullet
        {
            Destroy(gameObject);                    // Recyle the gameObject
            GameManager.s_GM.SendMessage("Leader_Board_Score", int_ScoreBoardPoints);       // send message to GameManager Function to add points
            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")       // if the gameObject exits a trigger from where it stops touching the player
        {
            PC_script.GetComponent<PC_Space_Ship_Controller>().fl_movement_speed += fl_deductSpeed;        // adds players orgianl speed back when the NPC is no longer a child
        }

        if (other.gameObject.tag == "Player" == null)   // if their is no more player
            PC_script = null;       // PC Script is nothing
    }
}
