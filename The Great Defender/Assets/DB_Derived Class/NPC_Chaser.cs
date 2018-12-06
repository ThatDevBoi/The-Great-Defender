using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NPC_Chaser : Base_Class
{
    [SerializeField]
    private int turboPoints = 2;      // Points for Turbo Charge
    [SerializeField]
    private int ScoreBoardPoints = 250;
    [SerializeField]
    private GameObject FlickingTextMesh;
    [SerializeField]
    private Transform PC;       // Reference to the player
    [SerializeField]
    private GameObject Critter_Prefab;
    [SerializeField]
    private Slider ChargeBar;

	// Use this for initialization
    protected override void Start ()
    {
        base.Start();
        IDE_PC_BC.isTrigger = true; // Makes Circle Collider Trigger true
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find Player
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
        if (PC == null && GameManager.s_GM.bl_Player_Dead == false)
            PC = GameObject.Find("PC(Clone)").GetComponent<Transform>();
        transform.position += mvelocity * Time.deltaTime;
        if(PC != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, PC.position, fl_movement_speed * Time.deltaTime);
        }
        
        if(PC != null)
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
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
            GameObject Crit = Instantiate(Critter_Prefab, transform.position, Quaternion.identity);
            ChargeBar.value += GameManager.s_GM.int_turbo_Shot_score_monitor;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", ScoreBoardPoints);

            GameObject TextMeshGO = Instantiate(FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
