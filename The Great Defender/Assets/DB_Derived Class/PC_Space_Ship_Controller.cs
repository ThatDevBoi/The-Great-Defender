using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This Dervied Class allows for a playable Character to move around a scene when having Horizontal Input And can shoot bullets in different ways
/// 1 tap shoots one bullet 5 Taps shoot 2 bullets at the same time and when a Slider value meets required Value Mouse click can shoot a Charge Shot
/// The Class can also allow for ScreenWrapping and Movement Restrictions
/// This Derived class can be applied to a GameObject with the Tag Player the same as its layer
/// </summary>
public class PC_Space_Ship_Controller : Base_Class
{
    // IDE
    public Transform IDE_trans_Critterprefab;     // Transform IDE Compoenent of critterPrefab
    public Slider IDE_ChargeBar_Slider;            // Reference to the UI Slider
    // Ints
    public int int_ButtonCount = 0;
    public int int_swirlButtonCount = 0;
    // Floats
    private float fl_ButtonCooler = 0.1f;    // Half a second before reset
    private float fl_swirlButtonCooler = 0.3f;  // When the input is being pressed players have half a second so its not spammed
    // GameObjects
    public GameObject GO_explosion_Effect;

    #region Start Function
    // Use this for initialization
    protected override void Start ()
    {
        // Base Class Start Function
        base.Start();
        IDE_PC_BC.isTrigger = true; // BoxCollider will be a trigger

        // Find the other shooting points for DoubleShot
        IDE_trans_double_fire_position_1 = GameObject.Find("Fire_Position_Double_Shot_Left_Wing").GetComponent<Transform>();
        IDE_trans_double_fire_position_2 = GameObject.Find("Fire_Position_Double_Shot_Right_Wing").GetComponent<Transform>();
    }
    #endregion

    #region Update Function
    // Update is called once per frame
    void FixedUpdate ()
    {
        #region Functions
        // Functions To Be Called 
        DoMove();
        Movement_Restriction();
        #endregion


        if(IDE_ChargeBar_Slider == null && GameManager.s_GM.bl_Player_Dead == false && Time.time == 1.0)
        {
            Debug.Log("Finding The Slider For PC");
            IDE_ChargeBar_Slider = GameObject.Find("ChargeShotSlider").GetComponent<Slider>();
        }

        if (IDE_ChargeBar_Slider != null)
            Debug.Log("We Have The Slider Transform For The Humans");

        if (GameManager.int_Player_Lives == -1)
            Destroy(gameObject);


        // allows player to remove the critter NPC from their ship
        #region Swirl
        if (Input.GetKeyDown(KeyCode.W) && transform.FindChild("Critter_NPC(Clone)"))     // if the W key is pressed down and a child that is critter_NPC(Clone) is found on this gameObject
        {
            IDE_trans_Critterprefab = transform.GetChild(5).GetComponentInChildren<Transform>();    // Search children to find the Transform Reference
              if (int_swirlButtonCount > 0 && int_swirlButtonCount == 2)   // if the input key is pressed 2 or more times
              {
                 transform.Find("Critter_NPC(Clone)");      // Find the Critter in the heiary
                 IDE_trans_Critterprefab.transform.parent = null;      // Unchild the Critter
              }  
            
            else// However if following hasnt been done
            {
                IDE_trans_Critterprefab = null; // There is no Transform reference for the critter
                fl_swirlButtonCooler = 0.5f;   // Cooldown resets 
                int_swirlButtonCount += 1;      // Adds to int
            }
        }

        if (fl_swirlButtonCooler > 0)   // if the float value is greater then 0
            fl_swirlButtonCooler -= 1 * Time.deltaTime; // start to decline the float
        else                     // if not true
            int_swirlButtonCount = 0;   // button counts will be 0 resets

        #endregion

        #region Normal Shooting
        // Default Shooting
        // If space bar is pressed Fire Function is called from base class
        if (Input.GetButtonDown("Jump"))        // If user presses space
        {
            base.Lazer_Beam();      // Call Function in base Class
            bl_DefaultShot = true;     // Set boolean flag to true
        }
        #endregion

        #region Double Shoot
        if (Input.GetButtonDown("Jump"))    // if the space bar is pressed down
        {
            if(fl_ButtonCooler > 0 && int_ButtonCount == 4)     // and the float value is greater than 0 and the button has been pressed 4 times
            {
                // Allow for double shot function in base class
                bl_doubleShoot = true;
                base.Double_Lazer_Beam();
            }
            else   // however if, the if statement isnt true
            {
                bl_doubleShoot = false; // revert double shot flag
                fl_ButtonCooler = 0.3f; // reset float cooler
                int_ButtonCount += 1;   // Button count goes up by 1
            }
        }

        if (fl_ButtonCooler > 0)    // if float is greater then 0
        {
            fl_ButtonCooler -= 1 * Time.deltaTime;  // decrease Float
        }
        else   // if not
        {
            int_ButtonCount = 0;    // reset the button count
        }
        #endregion

        #region Charge Shot
        if(Input.GetButtonDown("Fire1"))       // if the payer presses the left mouse button
        {
            if(IDE_ChargeBar_Slider == null)
            {
                IDE_ChargeBar_Slider = GameObject.Find("ChargeShotSlider").GetComponent<Slider>();
                // Uses GameManager int score to charge Bar when enemies die
                if (IDE_ChargeBar_Slider != null && IDE_ChargeBar_Slider.value > 29)         // if the Slider Charge Bar Value is equal to 29 (Change Later to a more balanced score)
                {
                    bl_chargeShoot = true;          // Users can use the charge shot
                    IDE_ChargeBar_Slider.value = 0;
                    if (bl_chargeShoot)           // when the boolean is true
                    {
                        base.ChargeShot();    // Call the Function in Base Class
                    }
                }
                if (IDE_ChargeBar_Slider.value < 29)   // If the Score is less than 29
                {
                    bl_chargeShoot = false;    // Boolean flag is false
                    bl_DefaultShot = true;     // Normal way of shooting is enabled
                }
            }
            #endregion
        }

    }
    #endregion

    #region Base Class Do Move
    protected override void DoMove()
    {
        // Call base function for movement
        base.DoMove();
        // make sure the Mvelocity Vector3 is clamped so it cant go beyond its max speed 
        mvelocity = Clamped_Move();
    }
    #endregion

    #region Player Restricntions on Y axis
    protected override void Movement_Restriction()
    {
        // PC Restriction needs to be different compared to the NPCs as the grabber NPC can leave the stage of screen from camera.
        // This is used so humans when abducted can turn into a mutant without the player seeing this action

        // When the GameObject moves up or down on the Y axis
        if (transform.position.y <= -8f)                                                              // If the Transform component position is more than or equal to -8
            transform.position = new Vector3(transform.position.x, -8f, transform.position.z);        // The new position for any GameObject will be restricted to -8 (Down on the Y axis)
        else if (transform.position.y >= 8.5f)                                                           // However if the transform position is less than 8.5
            transform.position = new Vector3(transform.position.x, 8.5f, transform.position.z);         // The new position of any GameObject is restricted to 8.5 (Up on Y axis)

        // Screen Wrapping coordinates (X axis restriction)
        if (transform.position.x >= 50f)     // if the transforms position is greater then 70f
        {
            transform.position = new Vector3(-50f, transform.position.y, transform.position.z); // Then wrap the object and place gameobject at -50 on the x 
        }
        else if (transform.position.x <= -50f) // However if the transforms position is less than --50f
        {
            transform.position = new Vector3(50, transform.position.y, transform.position.z); // Place gameobject at 50 on the x
        }
    }
    #endregion

    #region Fire Function
    protected override void Lazer_Beam()
    {
        // bass class laser function
        base.Lazer_Beam();
    }
    #endregion

    #region Collsion
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ways gameObject can die
        if(other.gameObject.tag == "UFO")   // When gameObject meets a trigger with the GameObject tag being UFO
        {
            Destroy(gameObject);    // Destroy the gameObject
            GameManager.int_Player_Lives--;     // Take away a life from the GameManager
            GameManager.s_GM.bl_Player_Dead = true; // boolean flag for PC being Dead is true
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);   // spawn explosion effect
            Destroy(explosion, 2f); // recycle explosition effect after 2 seconds
        }

        if (other.gameObject.tag == "NPC_Abducter")
        {
            Destroy(gameObject);        // Destroy the PC for hitting the NPC
            GameManager.int_Player_Lives--; // Take away a life from the GameManager
            GameManager.s_GM.bl_Player_Dead = true; // boolean flag for PC being Dead is true
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);   // spawn explosion effect
            Destroy(explosion, 2f); // recycle explosition effect after 2 seconds
        }
        if (other.gameObject.tag == "NPC_Chaser")
        {
            Destroy(gameObject);    // Destroy the PC for hitting the NPC
            GameManager.int_Player_Lives--; // Take away a life from the GameManager
            GameManager.s_GM.bl_Player_Dead = true; // boolean flag for PC being Dead is true
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);   // spawn explosion effect
            Destroy(explosion, 2f); // recycle explosition effect after 2 seconds
        }
        if(other.gameObject.tag == "NPC_Bullet")
        {
            Destroy(gameObject);    // Destroy the PC for hitting the NPC
            GameManager.int_Player_Lives--; // Take away a life from the GameManager
            GameManager.s_GM.bl_Player_Dead = true; // boolean flag for PC being Dead is true
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);   // spawn explosion effect
            Destroy(explosion, 2f); // recycle explosition effect after 2 seconds
        }

    }
    #endregion
}