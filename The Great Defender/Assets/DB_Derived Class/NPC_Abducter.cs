//Help needed references
// Used Asteroids workshop Week X to make my NPC abducter move mindlessly random

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Abducter : Base_Class
{
    public int points =2;
    [SerializeField]
    private float timer = 2f;   // Ticks down for change of direction
    [SerializeField]
    private Transform Fire_Pos; // Where Raycast is shot from

    // Abduction Logic
    [SerializeField]
    private bool abduction_choice = false;
    [SerializeField]
    private Transform human_Target;
    [SerializeField]
    private float mine_Dist = 2;


    protected override void Start()
    {
        base.Start();       // Call the start function that belongs to the base class
        PC_BC.isTrigger = true;        // Makes sure the Abducters arent a trigger, If the Abducter isnt a trigger. then other gameObjects like this can kill eachother
        //mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed)); // On start move this gameObject randomly with a random speed
    }

    public void Update()
    {
        // Functions
        DoMove();
        Movement_Restriction();
        Lazer_Beam();
        timer -= Time.deltaTime;    //Tick the timer float down
        if (!abduction_choice)   // If boolean flag is false
        {
            abduction_choice = false;
            if (timer <= 0) // if boolean is false and timer = 0
            {
                timer = 2; // Reset the timer 
                mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));  // Move Randomly in the game
            }
        }
        else if(abduction_choice)
        {
            if (Vector3.Distance(transform.position, human_Target.position) <= mine_Dist)
            {
                // Childs the Human_Target GameObject to this gameObject
                human_Target.transform.parent = gameObject.transform;
                // Mvelocity now moves gameObject up
                mvelocity = Vector2.up;
                // Booleans of if the npc chaser is abducting a gameObject is now true
                abduction_choice = true;
            }
            // Declared when not in distance the Boolean is false and the Human is no longer a child
            else if(Vector3.Distance(transform.position, human_Target.position) >= mine_Dist)
            {
                abduction_choice = false;
                human_Target.transform.parent = null;
            }
        }
        // For now this is used for when the NPC abducter is at the top of the screen the abduction choice is false so it can move freely once more
        if(transform.position.y <= 10)
        {
            abduction_choice = false;
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
        Vector2 firepos = new Vector2(Fire_Pos.position.x, Fire_Pos.position.y);
        // The Direction Of Fire
        Vector2 direction = Vector2.down;

        RaycastHit2D Hit = Physics2D.Raycast(firepos, direction, range, whatTohit);
        Debug.DrawRay(firepos, direction * range, Color.red, 1);

        if(Hit.collider != null)
        {
            abduction_choice = true;    // Makes boolean flag true when hitting a human
            Debug.Log("We Hit Some Shit");
            if(abduction_choice)
            {
                human_Target = Hit.collider.gameObject.GetComponent<Transform>();
                mvelocity = Vector2.down;
            }
        }
        #region Note
        // Dont Add A Prefab to find a Human. For One It'll just kill the human.
        // Secondly the behavouir of how the NPC abducter moves can kill it when it bumps into its open bullet. 
        // thirdly using an invisiable ray just adds feedback to the player for the player will never know what human is being a bducted
        #endregion
    }

    // Needs to be called as if its not then the NPC can leave the play area
    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();    
    }

    //protected override void ObjectHit(Base_Class other_objects)
    //{
    //    base.ObjectHit(other_objects);
    //    PC_BC.enabled = false;

    //    if(PC_BC.enabled == false)
    //    {
    //        Destroy(gameObject);

    //        GameManager.s_GM.SendMessage("ScorePoints", points);
    //    }
    //}

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
    }
}
