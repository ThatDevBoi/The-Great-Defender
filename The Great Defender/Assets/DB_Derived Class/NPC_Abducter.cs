//Help needed references
// Used Asteroids workshop Week X to make my NPC abducter move mindlessly random

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Abducter : Base_Class
{
    [SerializeField]
    private AudioSource NPC_as;

    [SerializeField]    // removed
    private int int_turboShotpoints = 1;   // Points gained when gameObject Dies used for turbo shot
    public int int_ScoreBoardPoints = 100;

    [SerializeField]
    private GameObject GO_FlickingTextMesh;
    [SerializeField]
    private GameObject GO_bullet;      // GameObject that will be fired/spanwed
    public GameObject Human_prefab;

    [SerializeField]    // Remove
    private Slider ChargeBarSlider;

    [SerializeField]    // remove
    private float fl_timer = 2f;   // Ticks down for change of direction
    [SerializeField]    // remove
    private float fl_AbductTimer = 2f;
    [SerializeField]
    private float fl_mine_Dist = 2;    // Value to tell how far away the human is. Allows for gameObject to abduct within 2 or less. 2 or more means no child
    private float fl_shootRay_time = 3;
    private float fl_Abductionsound_timer = 2f;
    [SerializeField]
    private Transform trans_Fire_Pos; // Where Raycast is shot from
    [SerializeField]    // Remove
    private Transform trans_human_Target;     // The transform of the Humanoids. This is not just 1 Transform component.

    // Abduction Logic
    [SerializeField]    // Remove
    private bool bool_abduction_choice = false;  // Boolean to tell if NPC will go towards and child a human GameObject

    private void Start()
    {
        PC_RB = gameObject.AddComponent<Rigidbody2D>();
        PC_RB.isKinematic = true;
        PC_SR = GetComponent<SpriteRenderer>();
        PC_BC = gameObject.GetComponent<Collider2D>();
        PC_BC.isTrigger = true;        // Makes sure the Abducters arent a trigger, If the Abducter isnt a trigger. then other gameObjects like this can kill eachother
        
    }

    public void FixedUpdate()
    {
        // Functions
        DoMove();
        Movement_Restriction();
        Lazer_Beam();
        ChargeBarSlider = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();  // Find Slider Component
        fl_shootRay_time -= Time.deltaTime;


        if(GameManager.s_GM.Player_Dead == true)
        {
            speed = 0;
            max_Speed = 0;
        }
        else if(GameManager.s_GM.Player_Dead == false)
        {
            speed = 1;
            max_Speed = 2;
        }
        if (bool_abduction_choice)
        {
            fl_Abductionsound_timer -= Time.deltaTime;
            if(fl_Abductionsound_timer<= 0)
            {
                fl_Abductionsound_timer = 10;
                NPC_as.Play();
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
                mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));  // Move Randomly in the game
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
                mvelocity = Vector2.up * speed;
            }
            // Declared when not in distance the Boolean is false and the Human is no longer a child
            else if(Vector3.Distance(transform.position, trans_human_Target.position) >= fl_mine_Dist)
            {
                NPC_as.volume = .5f;
                trans_human_Target.transform.parent = null;
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

        if(fl_shootRay_time <= 0)
        {
            fl_shootRay_time = 3;
            // Ray Origin is fire position, being fired down (-y) witin a set and using a LayerMask to decide what the ray detects
            RaycastHit2D Hit = Physics2D.Raycast(firepos, direction, range, whatTohit);
            Debug.DrawRay(firepos, direction * range, Color.green, 1);      // Remove when fully finished
            
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
                    mvelocity = Vector2.down * speed * 2;
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
        trans_human_Target.transform.parent = null;       // Let the human NPC detech from the parent when parent dies
        Human_prefab.layer = 10;
        trans_human_Target.GetComponent<Human_NPC>().enabled = true;      // Turn back on the Human_NPC script
        trans_human_Target.GetComponent<BoxCollider2D>().enabled = true;  // Turn back on the Human NPC colliders
        trans_human_Target.GetComponent<CircleCollider2D>().enabled = true;
        trans_human_Target.GetComponent<Collider2D>().isTrigger = true;   // Make the Collider a trigger
        // Make the Rigdbody dynamic so it will fall. When it gets to the ground it turns back to kinematic. This is controlled in the Human NPC script on line 39
        trans_human_Target.GetComponent<Rigidbody2D>().isKinematic = false;
        trans_human_Target.GetComponent<Rigidbody2D>().gravityScale = 0.2f;       // Gravity of a dynamic Rigidbody2D will be 0.2 
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")    // if another gameObject is got the tag Bullet
        {
            Destroy(gameObject);    GameManager.enemy_Count--;   // Destroy the GameObject and subtract 1 from the Static count value in GM
            ChargeBarSlider.value += GameManager.s_GM.score;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", int_ScoreBoardPoints);

            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
