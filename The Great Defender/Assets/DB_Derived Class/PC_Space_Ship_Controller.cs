using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC_Space_Ship_Controller : Base_Class
{
    // Double Tap Input Variables 
    public float ButtonCooler = 0.5f;    // Half a second before reset
    public int ButtonCount = 0;
    public float swirlButtonCooler = 0.5f;
    public int swirlButtonCount = 0;

    public Transform Critterprefab;     // Transform IDE Compoenent of critterPrefab
    public Slider ChargeBar;            // Reference to the UI Slider

    #region Start Function
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true;

        DefaultShot = true;
        doubleShoot = false;
        chargeShoot = false;
    }
    #endregion

    #region Update Function
    // Update is called once per frame
    void FixedUpdate ()
    {

        ChargeBar = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();      // Find slider Charge Bar

        #region Functions
        // Functions To Be Called 
        DoMove();
        Movement_Restriction();
        #endregion

        #region Swirl
        if(Input.GetKeyDown(KeyCode.W))     // if the W key is pressed down
        {
            if(swirlButtonCount > 0 && swirlButtonCount == 2)   // if the input key is pressed 2 or more times
            {
                Critterprefab = GameObject.FindGameObjectWithTag("Critter").GetComponent<Transform>();  // Find the Critter GameObject Transform
                transform.Find("Critter_NPC");      // Find the Critter in the heiary
                Critterprefab.transform.parent = null;      // Unchild the Critter
            }
            else// However if following hasnt been done
            {
                swirlButtonCooler = 0.5f;   // Cooldown resets 
                swirlButtonCount += 1;      // Adds to int
            }
        }

        if (swirlButtonCooler > 0)
            swirlButtonCooler -= 1 * Time.deltaTime;
        else
            swirlButtonCount = 0;

        #endregion

        #region Normal Shooting
        // Default Shooting
        // If space bar is pressed Fire Function is called from base class
        if (Input.GetButtonDown("Jump"))        // If user presses space
        {
            base.Lazer_Beam();      // Call Function in base Class
            DefaultShot = true;     // Set boolean flag to true
        }
        #endregion

        // Add fire rate to double so it cannot be spammed
        #region Double Shoot
        if (Input.GetButtonDown("Jump"))
        {
            if(ButtonCooler > 0 && ButtonCount == 4)
            {
                DefaultShot = false;
                doubleShoot = true;
                base.Double_Lazer_Beam();
            }
            else
            {
                DefaultShot = true;
                doubleShoot = false;
                ButtonCooler = 0.3f;
                ButtonCount += 1;
            }
        }

        if (ButtonCooler > 0)
        {
            ButtonCooler -= 1 * Time.deltaTime;
        }
        else
        {
            ButtonCount = 0;
        }
        #endregion

        #region Charge Shot
        if(Input.GetButtonDown("Fire1"))       // if the payer presses the left mouse button
        {
            // Uses GameManager int score to charge Bar when enemies die
            if (ChargeBar.value > 29)         // if the Slider Charge Bar Value is equal to 29 (Change Later to a more balanced score)
            {
                chargeShoot = true;          // Users can use the charge shot
                ChargeBar.value = 0;
                if (chargeShoot)           // when the boolean is true
                {
                    base.ChargeShot();    // Call the Function in Base Class
                }
            }
            if(ChargeBar.value < 29)   // If the Score is less than 29
            {
                chargeShoot = false;    // Boolean flag is false
                DefaultShot = true;     // Normal way of shooting is enabled
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
        else if (transform.position.y >= 7f)                                                           // However if the transform position is less than 7.5
            transform.position = new Vector3(transform.position.x, 7f, transform.position.z);         // The new position of any GameObject is restricted to 7.5 (Up on Y axis)

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
            GameManager.Player_Lives--;
            GameManager.s_GM.Player_Dead = true;
        }

        if (other.gameObject.tag == "NPC_Abducter")
        {
            Destroy(gameObject);        // Destroy the PC for hitting the NPC
            GameManager.Player_Lives--;
            GameManager.s_GM.Player_Dead = true;
        }
        if (other.gameObject.tag == "NPC_Chaser")
        {
            Destroy(gameObject);
            GameManager.Player_Lives--;
            GameManager.s_GM.Player_Dead = true;
        }
        if(other.gameObject.tag == "NPC_Bullet")
        {
            Destroy(gameObject);
            GameManager.Player_Lives--;
            GameManager.s_GM.Player_Dead = true;
        }

    }
    #endregion
}