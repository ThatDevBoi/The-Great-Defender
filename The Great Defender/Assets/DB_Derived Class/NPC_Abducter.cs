//Help needed references
// Used Asteroids workshop Week X to make my NPC abducter move mindlessly random

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Abducter : Base_Class
{
    [SerializeField]
    private int int_turboShotpoints = 1;   // Points gained when gameObject Dies used for turbo shot
    public int int_ScoreBoardPoints = 100;

    [SerializeField]
    private GameObject GO_FlickingTextMesh;
    [SerializeField]
    private GameObject GO_bullet;      // GameObject that will be fired/spanwed

    [SerializeField]
    private Slider ChargeBarSlider;

    [SerializeField]
    private float fl_timer = 2f;   // Ticks down for change of direction
    [SerializeField]
    private float fl_AbductTimer = 2f;
    [SerializeField]
    private float fl_mine_Dist = 2;    // Value to tell how far away the human is. Allows for gameObject to abduct within 2 or less. 2 or more means no child
    [SerializeField]
    private float fl_firerate;         // How fast the NPC will shoot
    [SerializeField]
    private float fl_nextFire;         // When the NPC can fire Next

    [SerializeField]
    private Transform trans_Fire_Pos; // Where Raycast is shot from
    [SerializeField]
    private Transform trans_human_Target;     // The transform of the Humanoids. This is not just 1 Transform component.

    // Abduction Logic
    [SerializeField]
    private bool bool_abduction_choice = false;  // Boolean to tell if NPC will go towards and child a human GameObject



    protected override void Start()
    {
        base.Start();       // Call the start function that belongs to the base class
        PC_BC.isTrigger = true;        // Makes sure the Abducters arent a trigger, If the Abducter isnt a trigger. then other gameObjects like this can kill eachother
        ChargeBarSlider = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();  // Find Slider Component
    }

    public void FixedUpdate()
    {
        // Functions
        DoMove();
        Movement_Restriction();
        Lazer_Beam();
        fl_timer -= Time.deltaTime;    //Tick the timer float down
        // For now this is used for when the NPC abducter is at the top of the screen the abduction choice is false so it can move freely once more
        if (gameObject.transform.position.y > 13) // This y value is the same as the base class restrictions. If its not declared then the NPC will just stay at 14.0y and never move freely again
        {
            fl_AbductTimer -= Time.deltaTime;  // Decline Abduct Timer
            if (fl_AbductTimer <= 0)   // When the Abduct timer value = 0
            {
                fl_AbductTimer = 2;    // Reset the timer
                bool_abduction_choice = false;   // // Say Boolean is false 
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
            bool_abduction_choice = true;
            if (Vector3.Distance(transform.position, trans_human_Target.position) <= fl_mine_Dist)   // and if the NPC is within range of the Human
            {
                // Childs the Human_Target GameObject to this gameObject
                trans_human_Target.transform.parent = gameObject.transform;
                // Mvelocity now moves gameObject up
                mvelocity = Vector2.up * speed;
                // Needs to be turned off when Human is childed for NPC inds Humans with Raycast hitting colliders. If theyre on then NPC will fight over the same NPC Human. Also it meas the player cant steal the NPC Human away from the gameObject
                trans_human_Target.GetComponent<BoxCollider2D>().enabled = false;
                trans_human_Target.GetComponent<CircleCollider2D>().enabled = false;
                // If the BoxCollider2D is turned off
                if (trans_human_Target.GetComponent<BoxCollider2D>().enabled = false)
                {
                    // Continue to move up
                    mvelocity = Vector2.up * speed;
                }
                // If the CircleCollider2D is turned off
                if (trans_human_Target.GetComponent<CircleCollider2D>().enabled = false)
                {
                    // Continue to move up
                    mvelocity = Vector2.up * speed;
                }
                else  // however if this isnt the case
                    // return nothing
                    return;
            }
            // Declared when not in distance the Boolean is false and the Human is no longer a child
            else if(Vector3.Distance(transform.position, trans_human_Target.position) >= fl_mine_Dist)
            {
                trans_human_Target.transform.parent = null;
            }
        }
    }
    // So the NPC can move around the scene
    protected override void DoMove()
    {
        transform.position += mvelocity * Time.deltaTime;
    }

    protected override void Lazer_Beam()
    {
        // Declaring the fire position 
        Vector2 firepos = new Vector2(trans_Fire_Pos.position.x, trans_Fire_Pos.position.y);
        // The Direction Of Fire
        Vector2 direction = Vector2.down;

        RaycastHit2D Hit = Physics2D.Raycast(firepos, direction, range, whatTohit);
        Debug.DrawRay(firepos, direction * range, Color.green, 1);      // Remove when fully finished

        if(Hit.collider != null)
        {
            bool_abduction_choice = true;    // Makes boolean flag true when hitting a human
            Debug.Log("We Hit Some Shit");
            if(bool_abduction_choice)
            {
                trans_human_Target = Hit.collider.gameObject.GetComponent<Transform>();
                mvelocity = Vector2.down * speed * 2;
            }
        }
    }

    // Needs to be called as if its not then the NPC can leave the play area
    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();    
    }

    public void OnBecameVisible()
    {
        if(Time.time > fl_nextFire)
        {
            Instantiate(GO_bullet, transform.position, Quaternion.identity);       // Spawn Bullet within the gameObjects transform position
            fl_nextFire = Time.time + fl_firerate;    // Reset the next time the Abducter can fire
        }
    }

    void OnDestroy()
    {
        // Resetting the Human NPC 
        trans_human_Target.transform.parent = null;       // Let the human NPC detech from the parent when parent dies
        trans_human_Target.GetComponent<Human_NPC>().enabled = true;      // Turn back on the Human_NPC script
                                                                    // Turn back on the Human NPC colliders
        trans_human_Target.GetComponent<Collider2D>().enabled = true;
        trans_human_Target.GetComponent<Collider2D>().isTrigger = true;   // Make the Collider a trigger
        trans_human_Target.GetComponent<CircleCollider2D>().enabled = true;
        // Make the Rigdbody dynamic so it will fall. When it gets to the ground it turns back to kinematic. This is controlled in the Human NPC script on line 39
        trans_human_Target.GetComponent<Rigidbody2D>().isKinematic = false;
        trans_human_Target.GetComponent<Rigidbody2D>().gravityScale = 0.2f;       // Gravity of a dynamic Rigidbody2D will be 0.2 
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")    // if another gameObject is got the tag Bullet
        {
            Destroy(gameObject);    GameManager.s_GM.enemy_Count--;   // Destroy the GameObject and subtract 1 from the Static count value in GM
            ChargeBarSlider.value += GameManager.score;       // Add int value to charge bar value

            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
