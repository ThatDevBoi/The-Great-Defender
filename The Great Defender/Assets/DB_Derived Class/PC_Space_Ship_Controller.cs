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
    }
    #endregion

    #region Player Restricntions on Y axis
    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
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
    }
    #endregion
}