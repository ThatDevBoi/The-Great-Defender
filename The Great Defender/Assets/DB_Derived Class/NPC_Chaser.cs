using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This Derived Class allows the NPC to chase the player around endlessly. Also spawning a Critter NPC prefab when it dies
/// This Dervied Class needs to be applied to a GameObject with the tag NPC_Chaser no layer is required
/// </summary>
public class NPC_Chaser : Base_Class
{
    [SerializeField]
    private int int_turboPoints = 2;      // Points for Turbo Charge
    [SerializeField]
    private int int_ScoreBoardPoints = 250;
    [SerializeField]
    private GameObject GO_FlickingTextMesh;
    [SerializeField]
    private Transform IDE_trans_PC;       // Reference to the player
    [SerializeField]
    private GameObject GO_Critter_Prefab;
    [SerializeField]
    private Slider ChargeBar;

	// Use this for initialization
    protected override void Start ()
    {
        base.Start();
        IDE_PC_BC.isTrigger = true; // Makes Circle Collider Trigger true
        IDE_trans_PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find Player
        ChargeBar = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();  // Find Slider Component
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // Functions
        DoMove();
        Movement_Restriction();

    }

    protected override void DoMove()
    {
        // when there is no PC Transform Reference and the player is not dead we find the Player in the scene and get its Transform Componenet
        if (IDE_trans_PC == null && GameManager.s_GM.bl_Player_Dead == false)
            IDE_trans_PC = GameObject.Find("PC(Clone)").GetComponent<Transform>();

        transform.position += mvelocity * Time.deltaTime;   // move object with base class vector and time
        // when the PC transform reference has been found
        if(IDE_trans_PC != null)
        {
            // move towards the players position
            transform.position = Vector3.MoveTowards(transform.position, IDE_trans_PC.position, fl_movement_speed * Time.deltaTime);
        }
        // if Player Transform is found show in the console
        if(IDE_trans_PC != null)
        {
            Debug.Log("Chaser Has PC Transform");
            return;
        }
    }
    // Base class Movement Restrictions
    // Shows how far the gameObject can go on thr X and Y before being stopped.
    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if this gameObject is hit with a tag called bullet
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);    // recyle gameObject
            GameObject Crit = Instantiate(GO_Critter_Prefab, transform.position, Quaternion.identity);  // spawn new NPC Critter
            ChargeBar.value += GameManager.s_GM.int_turbo_Shot_score_monitor;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", int_ScoreBoardPoints);   // send message to GameManager void to score points

            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
