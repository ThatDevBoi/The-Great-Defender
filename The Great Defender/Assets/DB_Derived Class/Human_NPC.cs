using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_NPC : Base_Class
{
    // IDE
    public Transform IDE_trans_PC;
    public Transform IDE_trans_HumanHolder;

    // Ints
    private int int_ScoreBoardPoints = 600;      // value that is displayed as a score, referenced to the GameManager shown via UI Text

    // Floats
    // Falling Logic
    private float fl_falling_Start_height;
    public float fl_max_Safe_Height = 5;
    public float fl_fatal_Fall_Height = 10;

    // Bools
    public bool fl_Grounded = false, fl_Ready_For_Drop = false;

    // GameObjects
    public GameObject GO_NPC_Chaser_prefab;
    public GameObject GO_FlickingTextMesh;



    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        IDE_PC_BC.isTrigger = true;
        IDE_PC_RB.isKinematic = true;
        IDE_PC_RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        // Functions
        DoMove();
        Movement_Restriction();

        // Needs to be updated for it'll lose reference when PC dies
        IDE_trans_PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 
        IDE_trans_HumanHolder = GameObject.FindGameObjectWithTag("Human_Hold_Place").GetComponent<Transform>();
        // At this point on the Y axis the Human turns into a mutant NPC
        if (transform.position.y >= 9f)
        {
            Destroy(gameObject); GameManager.s_GM.int_NPC_Human_Count--; // Destroy but decline static int
            GameObject Chaser_NPC = Instantiate(GO_NPC_Chaser_prefab, transform.position, Quaternion.identity);   // Spawn Chaser NPC
        }
        if (!fl_Grounded)   // in the air
        {
            // Maximum height the human npc reaches in the air
            if (transform.position.y > fl_falling_Start_height) fl_falling_Start_height = transform.position.y;
        }
        else   // on the ground
        {
            // if the height from fallig is fatal
            if (fl_falling_Start_height - transform.position.y > fl_fatal_Fall_Height)
                Destroy(gameObject);    // kill this object
            // reset the start height
            fl_falling_Start_height = transform.position.y;
        }
        if (!fl_Grounded)
        {
            fl_Grounded = false;
            fl_Ready_For_Drop = true;
        }
        else if (fl_Grounded)
        {
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;  // Turn rigidbody back to kinematic so gravity doesnt prevent NPC Abducter childing this
            IDE_PC_RB.velocity = Vector3.zero;      // Xero out velocity of Rigidbody when it changes from dynamic to kinamatic. If not it'll fall through solid ground
            fl_Grounded = true;                    // Human is back on the ground 

            fl_Ready_For_Drop = false;
        }

        if (!fl_Ready_For_Drop)
        {
            gameObject.transform.parent = null;
        }

        if (fl_Ready_For_Drop && gameObject.transform.parent == IDE_trans_PC.transform)  // When the Human NPC is ready to be dropped back to the ground and the Human is a child to the PC
        {
            fl_falling_Start_height = transform.position.y;    // Whenever the NPC human became a child it resets the height of falling so NPC doesnt die when dropped off
        }
    }

    protected override void DoMove()
    {
        transform.position += mvelocity * Time.deltaTime;         
    }

    protected override void Movement_Restriction()
    {
        return;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);    GameManager.s_GM.int_NPC_Human_Count--;
        }

        if(other.gameObject.tag == "NPC_Abducter")       // NPC Needs to turn Grounded off if not the Human wont be able to be picked up
        {
            fl_Grounded = false;
        }

        if(other.gameObject.tag == "Ground")
        {
            fl_Grounded = true;
        }
        if(other.gameObject.tag == "Player" && fl_Ready_For_Drop && gameObject.transform.parent == null)
        {
            transform.position = IDE_trans_HumanHolder.transform.position;
            gameObject.transform.parent = IDE_trans_PC.transform;
            fl_Grounded = false;

            if(gameObject.transform.parent = IDE_trans_PC.transform)
            {
                IDE_PC_RB.velocity = Vector3.zero;
                IDE_PC_RB.gravityScale = 0;
            }
        }

        if(other.gameObject.tag == "Drop_Off_Zone" && !fl_Ready_For_Drop)      // Drop off zone is just a boxcollider2D on an empty GO with the tag Drop_Off_Zone
        {
            gameObject.transform.parent = null;     // Detech Human from PC. PC is no longer the parent gameObject is independant
            fl_Grounded = true;
        }

        if(other.gameObject.tag == "Drop_Off_Zone")
        {
            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed

            GameManager.s_GM.SendMessage("Leader_Board_Score", int_ScoreBoardPoints);       // Sends message to GameManager void to add points to ui text
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
            fl_Grounded = false;
    }
}
