using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This Derived class uses the Base Class for inheritance however this class is able to abduct humanoid NPCs which it detects with a raycast and using a layer mask to tell what a human is
/// It moves around Randomly and changes direction during play. 
/// NPC can also fire bullets towards the player when the gameObject is be8ing Rendered on screen by a camera
/// The NPCs can also screen wrap and are limited to the movement restrictions ive set
/// This needs to be applied to a GameObject with the tag NPC_Abdcuter and the Layer NPC
/// </summary>
public class NPC_Abducter : Base_Class
{
    // IDE Components
    [SerializeField]
    private Transform trans_Fire_Pos;       // Where Raycast is shot from
    private Transform trans_human_Target;     // The transform of the Humanoids. The Raycast Finds this Component. So when Transform Component is destroyed
                                              // This GameObject can find another Humans Transform
    [SerializeField]
    private AudioSource NPC_as;     // Audio source reference used for abduction sound so player is notified
    [SerializeField]
    private Slider ChargeBarSlider; // Slider that monitors the Players Turbo Shot. GameObject needs this so int value makes the slider increase value

    // ints
    private int int_turboShotpoints = 1;       // Points gained when gameObject Dies used for turbo shot
    private int int_ScoreBoardPoints = 100;    // int visually shown on the TextMesh GameObject

    // floats
    private float fl_timer = 2f;   // Ticks down for gameObject to change of direction
    private float fl_AbductTimer = 2f;  // Used to revert Abduction Bool when the timer hits 0 boolean is false. This float declines when gameObject is at a certain position
    private float fl_mine_Dist = 2;    // Value to tell how far away the human is. Allows for gameObject to abduct within 2 or less. 2 or more means no child
    private float fl_shootRay_time = 3;     // Instead of a constant ray that'll take up CPU process, timer ticks down and shoots 1 ray when the timer = 0
    private float fl_Abduction_sound_timer = 2f;    // Ticks down when gameObject is about to abduct a human so a wav in the NPC_as(Audio Source will play)
    // Bools
    private bool bool_abduction_choice = false;  // Boolean to tell if NPC will go towards and child a human GameObject

    // GameObjects
    [SerializeField]
    private GameObject GO_FlickingTextMesh;     // TextMesh that has a Flicking Annimation used to display points when Player kills this
    [SerializeField]
    private GameObject GO_bullet;               // GameObject that will be fired/spanwed
    [SerializeField]
    private GameObject Human_prefab;            // Reference to the human prefab 


    private void Start()
    {
        IDE_PC_RB = gameObject.AddComponent<Rigidbody2D>();     // Add Rigidbody2D IDE Component
        IDE_PC_RB.isKinematic = true;       // Rigidbody is only used for collision doesnt need all physics, turn on Kinematic 
        IDE_PC_SR = GetComponent<SpriteRenderer>();     // Find Sprite Renderer on this gameObject   
        IDE_PC_BC = gameObject.GetComponent<Collider2D>();      // Find any Collider2D component attached to this gameObject
        IDE_PC_BC.isTrigger = true;        // Makes sure the Abducters arent a trigger, If the Abducter isnt a trigger. then other gameObjects like this can kill eachother
        if (GameManager.int_Player_Lives == -1)
            ChargeBarSlider = null;
        ChargeBarSlider = GameObject.Find("ChargeShotSlider").GetComponent<Slider>();
    }

    public void FixedUpdate()
    {
        // Functions
        DoMove();
        Movement_Restriction();
        Lazer_Beam();
        fl_shootRay_time -= Time.deltaTime;

        if (GameManager.int_Player_Lives == -1)
            ChargeBarSlider = null;

        if (GameManager.s_GM.bl_Player_Dead == true)        // If the boolean inside the GameManager notifying the Player is dead is true
        {
            // Dont Allow for movement
            fl_movement_speed = 0;      
            fl_max_movement_Speed = 0;
        }
        else if(GameManager.s_GM.bl_Player_Dead == false)      // However if the boolean is now false and the player is now back to being alive
        {
            // turn movement back on
            fl_movement_speed = 1;
            fl_max_movement_Speed = 2;
        }
        // when the boolean abduction is true
        if (bool_abduction_choice)
        {
            // Tick float down so it declines
            fl_Abduction_sound_timer -= Time.deltaTime;
            if(fl_Abduction_sound_timer<= 0)    // When the value is equal to 0 or more
            {
                fl_Abduction_sound_timer = 10;  // reset float value
                NPC_as.Play();      // Play wave sound
            }
        }

        if (trans_human_Target == null) // If the Abducotr no longer has a reference or never did
            bool_abduction_choice = false;  // Abduction is false

        fl_timer -= Time.deltaTime;    //Tick the timer float down
        // For now this is used for when the NPC abducter is at the top of the screen the abduction choice is false so it can move freely once more
        if (gameObject.transform.position.y > 10) // This y value is the same as the base class restrictions. If its not declared then the NPC will just stay at 14.0y and never move freely again
        {
            fl_AbductTimer -= Time.deltaTime;  // Decline Abduct Timer
            if (fl_AbductTimer <= 0)          // When the Abduct timer value = 0
            {
                fl_AbductTimer = 2;             // Reset the timer
                bool_abduction_choice = false;  // Say Boolean is false 
            }
        }
        // Abducting a Human Behavouir
        if (!bool_abduction_choice)   // If boolean flag is false
        {
            // Booleans of if the npc chaser is abducting a gameObject is now true
            bool_abduction_choice = false;
            if (fl_timer <= 0) // if boolean is false and timer = 0
            {
                fl_timer = 2; // Reset the timer 
                mvelocity = new Vector2(Random.Range(-fl_movement_speed, fl_movement_speed), Random.Range(-fl_movement_speed, fl_movement_speed));  // Move Randomly in the game
            }
        }
        else if(bool_abduction_choice)       // However if flag is true
        {
            //bool_abduction_choice = true;
            if (Vector3.Distance(transform.position, trans_human_Target.position) <= fl_mine_Dist)   // and if the NPC is within range of the Human
            {
                // Childs the Human_Target GameObject to this gameObject
                trans_human_Target.transform.parent = gameObject.transform;
                // Mvelocity now moves gameObject up
                mvelocity = Vector2.up * fl_movement_speed;
            }
            // Declared when not in distance the Boolean is false and the Human is no longer a child
            else if(Vector3.Distance(transform.position, trans_human_Target.position) >= fl_mine_Dist)
            {
                trans_human_Target.transform.parent = null;     // Human wont be parented to this gameObject
            }
        }
    }
    // So the NPC can move around the scene
    protected override void DoMove()
    {
        transform.position += mvelocity * Time.deltaTime;
    }
    // Using Racast to detech the Human but also decide where the Ray is fired from and in what direction
    protected override void Lazer_Beam()
    {
        // Declaring the fire position 
        Vector2 firepos = new Vector2(trans_Fire_Pos.position.x, trans_Fire_Pos.position.y);
        // The Direction Of Fire is down -y
        Vector2 direction = Vector2.down;
        if(fl_shootRay_time <= 0)       // When the float value = 0 or more
        {
            fl_shootRay_time = 3;   // reset value
            // Ray Origin is fire position, being fired down (-y) witin a set and using a LayerMask to decide what the ray detects
            RaycastHit2D Hit = Physics2D.Raycast(firepos, direction, fl_range, IDE_layerMask_whatTohit);
            Debug.DrawRay(firepos, direction * fl_range, Color.green, 1);      // Remove when fully finished
            
            if (Hit.collider != null)   // if the raycast hits a collider within its fixed LayerMask settings (What the Ray is detecting)
            {
                bool_abduction_choice = true;    // Makes boolean flag true when hitting a human we can now abduct
                if (bool_abduction_choice)
                {
                    Human_prefab = GameObject.Find("Humanoid(Clone)");      // Finds its human with the raycast
                    // Find the Transform Component of the Human hit with the ray
                    trans_human_Target = Hit.collider.gameObject.GetComponent<Transform>();
                    // Change Humanoid layer so no other gameObjects with this script attached can find that taken human
                    Human_prefab.layer = 0;
                    // gameObject goes down so abduction Logic shown in FixedUpdate can be exacuted
                    // This logic is when gameObject is within a distance of human, its childed and the gameObject does up
                    mvelocity = Vector2.down * fl_movement_speed * 2;
                }
            }
        }
    }

    //// Needs to be called as if its not then the NPC can leave the play area
    protected override void Movement_Restriction()
    {
        // Found a Human Transform
        if(bool_abduction_choice)       // When this booloean is true
        {
            if (transform.position.y >= 11f)                                                           // if the gameObjects Transform is less than 11 on the y Axis (Up)
                transform.position = new Vector3(transform.position.x, 11f, transform.position.z);         // 11 on the Y is as far as this gameObject can move up it cannot reach 12 or beyond
                                                                                                           // When the GameObject moves up or down on the Y axis
            if (transform.position.y <= -8)                                                              // if the gameObjects Transform is Greater than -8 on the y Axis (Down)
                transform.position = new Vector3(transform.position.x, -8, transform.position.z);        // -8 on the Y acis is as far as this gameObject can move. It cannot go any further than -8f

            // Screen Wrapping coordinates (X axis restriction)
            if (transform.position.x >= 50f)     // if the transforms position is greater then 50f
            {
                transform.position = new Vector3(-50f, transform.position.y, 0); // Then wrap the object and place gameobject at -70 on the x 
            }
        }
        // Still trying to find a Human Transform
        else if(!bool_abduction_choice) // However if the bool is false
        {
            // When the GameObject moves up or down on the Y axis
            if (transform.position.y <= -5f)                                                              // The Transform is restricted to -5f (Human Judgement value. Dont change. if any higher the NPC abducter overlaps the human when only detecting them)
                transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
            else if (transform.position.y >= 7f)                                                           // however if the gameObject transform is less than 7 on the y (Up)
                transform.position = new Vector3(transform.position.x, 7f, transform.position.z);         // Restricted to 7 (up) on the Y

            // Screen Wrapping coordinates (X axis restriction)
            if (transform.position.x >= 50f)     // if the transforms position is greater then 70f
            {
                transform.position = new Vector3(-50f, transform.position.y, transform.position.z); // Then wrap the object and place gameobject at -50 on the x 
            }
            else if (transform.position.x <= -50f) // However if the transforms position is less than -50f
            {
                transform.position = new Vector3(50, transform.position.y, transform.position.z); // Place gameobject at 50 on the x
            }
        }
    }
    // When the gameObject is rendered ons screen
    public void OnBecameVisible()
    {
            Instantiate(GO_bullet, transform.position, Quaternion.identity);       // Spawn Bullet within the gameObjects transform position  
    }
    // When im about to die
    void OnDestroy()
    {
        // Resetting the Human NPC 
        if (trans_human_Target != null)      // Let the human NPC detech from the parent when parent dies
        {
            trans_human_Target.transform.parent = null;
            Human_prefab.layer = 10;                            // When human deteches from the NPC Abductor Chnage Layer Back to 10 which is humanoid
            trans_human_Target.GetComponent<Human_NPC>().enabled = true;      // Turn back on the Human_NPC script
            trans_human_Target.GetComponent<BoxCollider2D>().enabled = true;  // Turn back on the Human NPC colliders
            trans_human_Target.GetComponent<CircleCollider2D>().enabled = true;
            trans_human_Target.GetComponent<Collider2D>().isTrigger = true;   // Make the Collider a trigger
                                                                              // Make the Rigdbody dynamic so it will fall. When it gets to the ground it turns back to kinematic. This is controlled in the Human NPC script on line 39
            trans_human_Target.GetComponent<Rigidbody2D>().isKinematic = false;
            trans_human_Target.GetComponent<Rigidbody2D>().gravityScale = 0.2f;       // Gravity of a dynamic Rigidbody2D will be 0.2 
        }
        else if (trans_human_Target == null)
            return;
        

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")    // if another gameObject is got the tag Bullet
        {
            Destroy(gameObject);    GameManager.int_enemy_Count--;   // Destroy the GameObject and subtract 1 from the Static count value in GM
            ChargeBarSlider.value += GameManager.s_GM.int_turbo_Shot_score_monitor;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", int_ScoreBoardPoints);

            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
