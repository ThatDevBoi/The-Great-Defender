using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC_Space_Ship_Controller : Base_Class
{
    // IDE
    public Transform IDE_trans_Critterprefab;     // Transform IDE Compoenent of critterPrefab
    public Slider IDE_ChargeBar_Slider;            // Reference to the UI Slider
    // Ints
    public int int_ButtonCount = 0;
    public int int_swirlButtonCount = 0;
    // Floats
    public float fl_ButtonCooler = 0.5f;    // Half a second before reset
    public float fl_swirlButtonCooler = 0.5f;  // When the input is being pressed players have half a second so its not spammed
    // GameObjects
    public GameObject GO_explosion_Effect;

    #region Start Function
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        IDE_PC_BC.isTrigger = true;

        bl_DefaultShot = true;
        bl_doubleShoot = false;
        bl_chargeShoot = false;
        // Find the other shooting points
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

        IDE_ChargeBar_Slider = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();      // Find slider Charge Bar Slider isnt active on start needs to be uodated

        // allows player to remove the critter NPC from their ship
        #region Swirl
        if(Input.GetKeyDown(KeyCode.W))     // if the W key is pressed down
        {
            if(int_swirlButtonCount > 0 && int_swirlButtonCount == 2)   // if the input key is pressed 2 or more times
            {
                IDE_trans_Critterprefab = GameObject.FindGameObjectWithTag("Critter").GetComponent<Transform>();  // Find the Critter GameObject Transform
                transform.Find("Critter_NPC");      // Find the Critter in the heiary
                IDE_trans_Critterprefab.transform.parent = null;      // Unchild the Critter
            }
            else// However if following hasnt been done
            {
                fl_swirlButtonCooler = 0.5f;   // Cooldown resets 
                int_swirlButtonCount += 1;      // Adds to int
            }
        }

        if (fl_swirlButtonCooler > 0)
            fl_swirlButtonCooler -= 1 * Time.deltaTime;
        else
            int_swirlButtonCount = 0;

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
        if (Input.GetButtonDown("Jump"))
        {
            if(fl_ButtonCooler > 0 && int_ButtonCount == 4)
            {
                bl_doubleShoot = true;
                base.Double_Lazer_Beam();
            }
            else
            {
                bl_doubleShoot = false;
                fl_ButtonCooler = 0.3f;
                int_ButtonCount += 1;
            }
        }

        if (fl_ButtonCooler > 0)
        {
            fl_ButtonCooler -= 1 * Time.deltaTime;
        }
        else
        {
            int_ButtonCount = 0;
        }
        #endregion

        #region Charge Shot
        if(Input.GetButtonDown("Fire1"))       // if the payer presses the left mouse button
        {
            // Uses GameManager int score to charge Bar when enemies die
            if (IDE_ChargeBar_Slider.value > 29)         // if the Slider Charge Bar Value is equal to 29 (Change Later to a more balanced score)
            {
                bl_chargeShoot = true;          // Users can use the charge shot
                IDE_ChargeBar_Slider.value = 0;
                if (bl_chargeShoot)           // when the boolean is true
                {
                    base.ChargeShot();    // Call the Function in Base Class
                }
            }
            if(IDE_ChargeBar_Slider.value < 29)   // If the Score is less than 29
            {
                bl_chargeShoot = false;    // Boolean flag is false
                bl_DefaultShot = true;     // Normal way of shooting is enabled
            }
        }
        #endregion
    }
    #endregion

    #region Base Class Do Move
    protected override void DoMove()
    {
        base.DoMove();

        mvelocity = Clamped_Move();
    }
    #endregion

    #region Player Restricntions on Y axis
    protected override void Movement_Restriction()
    {
        // PC Restriction needs to be different compared to the NPCs as the grabber NPC can leave the stage of screen from camera.
        // This is used so humans when abducted can turn into a mutant without the player seeing this action

        // When the GameObject moves up or down on the Y axis
        if (transform.position.y <= -8f)                                                              // If the Transform component position is more than or equal to -7.5
            transform.position = new Vector3(transform.position.x, -8f, transform.position.z);        // The new position for any GameObject will be restricted to -7.5 (Down on the Y axis)
        else if (transform.position.y >= 8.5f)                                                           // However if the transform position is less than 7.5
            transform.position = new Vector3(transform.position.x, 8.5f, transform.position.z);         // The new position of any GameObject is restricted to 7.5 (Up on Y axis)

        // Screen Wrapping coordinates (X axis restriction)
        if (transform.position.x >= 50f)     // if the transforms position is greater then 70f
        {
            transform.position = new Vector3(-50f, transform.position.y, transform.position.z); // Then wrap the object and place gameobject at -70 on the x 
        }
        else if (transform.position.x <= -50f) // However if the transforms position is less than -70f
        {
            transform.position = new Vector3(50, transform.position.y, transform.position.z); // Place gameobject at 70 on the x
        }
    }
    #endregion

    #region Fire Function
    protected override void Lazer_Beam()
    {
        base.Lazer_Beam();
    }
    #endregion

    #region Collsion
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "UFO")
        {
            Destroy(gameObject);
            GameManager.int_Player_Lives--;
            GameManager.s_GM.bl_Player_Dead = true;
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        if (other.gameObject.tag == "NPC_Abducter")
        {
            Destroy(gameObject);        // Destroy the PC for hitting the NPC
            GameManager.int_Player_Lives--;
            GameManager.s_GM.bl_Player_Dead = true;
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        if (other.gameObject.tag == "NPC_Chaser")
        {
            Destroy(gameObject);
            GameManager.int_Player_Lives--;
            GameManager.s_GM.bl_Player_Dead = true;
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        if(other.gameObject.tag == "NPC_Bullet")
        {
            Destroy(gameObject);
            GameManager.int_Player_Lives--;
            GameManager.s_GM.bl_Player_Dead = true;
            GameObject explosion = Instantiate(GO_explosion_Effect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

    }
    #endregion
}