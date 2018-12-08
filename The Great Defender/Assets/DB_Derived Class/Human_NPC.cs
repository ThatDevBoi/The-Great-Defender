using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is a script used for a humanoid character that can die when falling from to high of a height. It also inherites from the Base Class
/// The Human can also be Picked up by the Player depending on the current state declared by booleans
/// it also spawns another NPC when at a desired height.These NPCs were going to move however i decided not to add this feature 
/// This script needs to be applied to a GameObject with a layer called Humanoid 
/// </summary>
public class Human_NPC : Base_Class
{
    // IDE
    public Transform IDE_trans_PC;      // Reference to the Players Transform
    public Transform IDE_trans_HumanHolder;     // reference for the Placeholder for when the PC Saves the NPC from falling

    // Ints
    private int int_ScoreBoardPoints = 600;      // value that is displayed as a score, referenced to the GameManager shown via UI Text

    // Floats
    // Falling Logic
    private float fl_falling_Start_height;      // value of when the NPC starts to fall it holds that position
    public float fl_max_Safe_Height = 5;        // The value that the player can fall from and be okay
    public float fl_fatal_Fall_Height = 10;     // value that if met NPC dies

    // Bools
    public bool bl_Grounded = false, bl_Ready_For_Drop = false;     // flag to be grounded and to be dropped off

    // GameObjects
    public GameObject GO_NPC_Chaser_prefab;     // Chaser Prefab reference
    public GameObject GO_FlickingTextMesh;      // TextMesh GameObject Reference



    // Use this for initialization
    protected override void Start ()
    {
        base.Start();       // Base class Start Function
        IDE_PC_BC.isTrigger = true;     // Collider is a trigger
        IDE_PC_RB.isKinematic = true;       // rigidbody2D is kinematic
        IDE_PC_RB.constraints = RigidbodyConstraints2D.FreezeRotation;      // Constrated on the Z Rotation of the rigidbody2d
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        // Functions
        DoMove();
        Movement_Restriction();
        // When no PC Transform is found and the player isnt dead
        if(IDE_trans_PC == null && GameManager.s_GM.bl_Player_Dead == false)
        {
            Debug.Log("Human Doesnt Have PC Transform");        // Show in console
            IDE_trans_PC = GameObject.Find("PC(Clone)").GetComponent<Transform>();  // Find the Player in the scene and get its Transform
        }

        if (IDE_trans_PC != null)   //  when the gameObject has the Players Transform
            Debug.Log("We Have The PC Transform For The Humans");       // Show in console
        // if the gameObject placeHolder transform is not found and the player isnt dead
        if(IDE_trans_HumanHolder == null && GameManager.s_GM.bl_Player_Dead == false)
        {
            Debug.Log("Human Doesnt Have its Player Characters Holding Position");  // Show in console
            IDE_trans_HumanHolder = GameObject.Find("Human_Child_Point").GetComponent<Transform>(); // Find the Transform which is located in the scene cholded onto the player
        }

        if (IDE_trans_HumanHolder != null)  // if we have the PlaceHolder Transform
            Debug.Log("Humna has its PC child point");      // Show in console

        if (GameManager.int_Player_Lives == -1)     // When the player has no more lifes
            Destroy(gameObject);        // Destroy this

        // At this point on the Y axis the Human turns into a mutant NPC
        if (transform.position.y >= 9f)
        {
            Destroy(gameObject); GameManager.s_GM.int_NPC_Human_Count--; // Destroy but decline static int
            GameObject Chaser_NPC = Instantiate(GO_NPC_Chaser_prefab, transform.position, Quaternion.identity);   // Spawn Chaser NPC
        }
        if (!bl_Grounded)   // in the air
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
        // if boolean grounded is false
        if (!bl_Grounded)
        {
            bl_Grounded = false;        // now false
            bl_Ready_For_Drop = true;       // can be collected and childed into the PlaceHolder is true
        }
        else if (bl_Grounded)       // However if grounded is true
        {
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;  // Turn rigidbody back to kinematic so gravity doesnt prevent NPC Abducter childing this
            IDE_PC_RB.velocity = Vector3.zero;      // Xero out velocity of Rigidbody when it changes from dynamic to kinamatic. If not it'll fall through solid ground
            bl_Grounded = true;                    // Human is back on the ground 

            bl_Ready_For_Drop = false;
        }
        // when boolean is false
        if (!bl_Ready_For_Drop)
        {
            // human is no longer a child
            gameObject.transform.parent = null;
        }

        if(IDE_trans_PC != null && GameManager.s_GM.bl_Player_Dead == false)    // if gameObject has Transform componenet of the Player and the player isnt dead
        {
            if (bl_Ready_For_Drop && gameObject.transform.parent == IDE_trans_PC.transform && GameManager.s_GM.bl_Player_Dead == false)  // When the Human NPC is ready to be dropped back to the ground and the Human is a child to the PC
            {
                fl_falling_Start_height = transform.position.y;    // Whenever the NPC human became a child it resets the height of falling so NPC doesnt die when dropped off
            }
        }
    }
    // Base class for movement needs to be in the class without it the Humans move when the player does
    protected override void DoMove()
    {
        return;      
    }
    // base class of movement restrictions
    protected override void Movement_Restriction()
    {
        return;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")       // if we hit a gameObject called bullet
        {
            // we kill object       // Take away a value from how many humans there are
            Destroy(gameObject);    GameManager.s_GM.int_NPC_Human_Count--;
        }

        if(other.gameObject.tag == "NPC_Abducter")       // NPC Needs to turn Grounded off if not the Human wont be able to be picked up
        {
            bl_Grounded = false;        // flag of gameObject being on the ground is false
        }

        if(other.gameObject.tag == "Ground")    // if gameObject is colliding with a object called Ground via tag
        {
            bl_Grounded = true;     // boolean is true
        }
        // if we are hitting player and bl of ready to drop is true and gameObject doesnt have a parent
        if(other.gameObject.tag == "Player" && bl_Ready_For_Drop && gameObject.transform.parent == null)
        {
            transform.position = IDE_trans_HumanHolder.transform.position;  // Position of the gameObject is now the human placeholder Transform which is a child of the Player
            gameObject.transform.parent = IDE_trans_PC.transform;       // gameObject then gets childed
            bl_Grounded = false;        // we're no longer grounded either

            if(gameObject.transform.parent = IDE_trans_PC.transform)
            {
                IDE_PC_RB.velocity = Vector3.zero;      // Zero out the velocity on the Rigidbody2D (When the NPC Abductor dies the Rigidbody2D of this object becomes Dynamic instead of kinamatic)
                IDE_PC_RB.gravityScale = 0;     // Gravity value is zero
            }
        }

        if(other.gameObject.tag == "Drop_Off_Zone" && !bl_Ready_For_Drop)      // Drop off zone is just a boxcollider2D on an empty GO with the tag Drop_Off_Zone
        {
            gameObject.transform.parent = null;     // Detech Human from PC. PC is no longer the parent gameObject is independant
            bl_Grounded = true;
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
            bl_Grounded = false;
    }
}
