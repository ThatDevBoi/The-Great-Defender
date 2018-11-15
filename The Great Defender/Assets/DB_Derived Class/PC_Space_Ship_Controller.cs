using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Space_Ship_Controller : Base_Class
{
    // Double Tap Input Variables 
    public float ButtonCooler = .5f;    // Half a second before reset
    public int ButtonCount = 0;


    #region Start Function
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true;
    }
    #endregion

    #region Update Function
    // Update is called once per frame
    void Update ()
    {

        #region Functions
        // Functions To Be Called 
        DoMove();
        Movement_Restriction();
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
            if(ButtonCooler > 0 && ButtonCount == 3)
            {
                DefaultShot = false;
                doubleShoot = true;
                base.Double_Lazer_Beam();
            }
            else
            {
                doubleShoot = false;
                ButtonCooler = 0.5f;
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
            if (GameManager.score == 6)     // if the Gamemanger static socre int is equal to 6 (Change Later to a more balanced score)
            {
                chargeShoot = true;     // Users can use the charge shot
                GameManager.score = 0;  // reset the score so players cant reuse it 
                if (chargeShoot)        // when the boolean is true
                {
                    base.ChargeShot();  // Call the Function in Bass Class
                }
            }

            if(GameManager.score < 6)   // If the Score is less than 6
            {
                chargeShoot = false;// Boolean flag is false
                DefaultShot = true;// Normal way of shooting is enabled
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
        if (transform.position.y <= -7f)                                                              // If the Transform component position is more than or equal to -7.5
            transform.position = new Vector3(transform.position.x, -7f, transform.position.z);        // The new position for any GameObject will be restricted to -7.5 (Down on the Y axis)
        else if (transform.position.y >= 7f)                                                           // However if the transform position is less than 7.5
            transform.position = new Vector3(transform.position.x, 7f, transform.position.z);         // The new position of any GameObject is restricted to 7.5 (Up on Y axis)

        // Screen Wrapping coordinates (X axis restriction)
        if (transform.position.x >= 70f)     // if the transforms position is greater then 70f
        {
            transform.position = new Vector3(-70f, 0, 0); // Then wrap the object and place gameobject at -70 on the x 
        }
        else if (transform.position.x <= -70) // However if the transforms position is less than -70f
        {
            transform.position = new Vector3(70, 0, 0); // Place gameobject at 70 on the x
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
    protected override void ObjectHit(Base_Class other_objects)
    {
        base.ObjectHit(other_objects);

        PC_BC.enabled = false;

        if(PC_BC.enabled == false)
        {
            Destroy(gameObject);

            // Respawn if i have lifes

        // with no life gameoever scene appears
        }
    }
    #endregion
}