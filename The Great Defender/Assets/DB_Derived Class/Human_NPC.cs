using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_NPC : Base_Class
{
    public int ScoreBoardPoints = 600;
    public float HumanTimer = 5f;
    public GameObject NPC_Chaser;
    public GameObject FlickingTextMesh;
    public bool Grounded = false, Ready_For_Drop = false;
    public Transform PC;
    public Transform HumanHolder;

    // Falling Logic
    private float falling_Start_height;
    public float max_Safe_Height = 5;
    public float fatal_Fall_Height = 10;
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true;
        PC_RB.isKinematic = true;
        mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));  // On start we move randomly

        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        HumanHolder = GameObject.FindGameObjectWithTag("Human_Hold_Place").GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        // Functions
        DoMove();
        Movement_Restriction();
        HumanTimer -= Time.deltaTime;       // Decline timer 1 second Per frame

        // At this point on the Y axis the Human turns into a mutant NPC
        if(transform.position.y >= 9f)
        {
            Destroy(gameObject); GameManager.s_GM.NPC_Human_Count--; // Destroy but decline static int

            GameObject Chaser_NPC = Instantiate(NPC_Chaser, transform.position, Quaternion.identity);
        }

        if (!Grounded)   // in the air
        {
            // Maximum height the human npc reaches in the air
            if (transform.position.y > falling_Start_height) falling_Start_height = transform.position.y;
        }
        else   // on the ground
        {
            // if the height from fallig is fatal
            if (falling_Start_height - transform.position.y > fatal_Fall_Height)
                Destroy(gameObject);    // kill this object
            // reset the start height
            falling_Start_height = transform.position.y;
        }

        if (!Grounded)
        {
            Grounded = false;
            Ready_For_Drop = true;
        }
        else if (Grounded)
        {
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;  // Turn rigidbody back to kinematic so gravity doesnt prevent NPC Abducter childing this
            PC_RB.velocity = Vector3.zero;      // Xero out velocity of Rigidbody when it changes from dynamic to kinamatic. If not it'll fall through solid ground
            Grounded = true;                    // Human is back on the ground 

            Ready_For_Drop = false;
        }
            

        if (!Ready_For_Drop)
        {
            gameObject.transform.parent = null;
        }


        if (Ready_For_Drop && gameObject.transform.parent == PC.transform)  // When the Human NPC is ready to be dropped back to the ground and the Human is a child to the PC
        {
            falling_Start_height = transform.position.y;    // Whenever the NPC human became a child it resets the height of falling so NPC doesnt die when dropped off
        }

    }

    protected override void DoMove()
    {
           transform.position += mvelocity * Time.deltaTime;         
    }

    protected override void Movement_Restriction()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);    GameManager.s_GM.NPC_Human_Count--;
        }

        if(other.gameObject.tag == "NPC_Abducter")       // NPC Needs to turn Grounded off if not the Human wont be able to be picked up
        {
            Grounded = false;
        }

        if(other.gameObject.tag == "Ground")
        {
            Grounded = true;
            DoMove();
        }
        if(other.gameObject.tag == "Player" && Ready_For_Drop)
        {
            transform.position = HumanHolder.transform.position;
            gameObject.transform.parent = PC.transform;
            Grounded = false;

            if(gameObject.transform.parent = PC.transform)
            {
                PC_RB.velocity = Vector3.zero;
                PC_RB.gravityScale = 0;
            }
        }

        if(other.gameObject.tag == "Drop_Off_Zone" && !Ready_For_Drop)      // Drop off zone is just a boxcollider2D on an empty GO with the tag Drop_Off_Zone
        {
            gameObject.transform.parent = null;     // Detech Human from PC. PC is no longer the parent gameObject is independant
            Grounded = true;
        }

        if(other.gameObject.tag == "Drop_Off_Zone")
        {
            GameObject TextMeshGO = Instantiate(FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed

            GameManager.s_GM.SendMessage("Leader_Board_Score", ScoreBoardPoints);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
            Grounded = false;
    }

}
