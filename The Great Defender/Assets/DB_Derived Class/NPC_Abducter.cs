//Help needed references
// Used Asteroids workshop Week X to make my NPC abducter move mindlessly random

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Abducter : Base_Class
{
    public int points;
    [SerializeField]
    protected float x_npc_speed = 5f;
    [SerializeField]
    protected float y_npc_speed = 2f;
    [SerializeField]
    protected float z_npc_move = 4f;        // Dont change value. Speed is balanced at 4
    [SerializeField]
    protected Transform Target;
    [SerializeField]
    private float timer = 2f;

    protected override void Start()
    {
        base.Start();       // Call the start function that belongs to the base class
        PC_BC.isTrigger = false;        // Makes sure the Abducters arent a trigger, If the Abducter isnt a trigger. then other gameObjects like this can kill eachother
        mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed)); // On start move this gameObject randomly with a random speed
    }

    private void Update()
    {
        DoMove();
        Movement_Restriction();

        // Random Movement      // NPC in defender doesnt follow the player instead the abducter NPC move randomly and pick out a human to abduct 
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = 2;
            mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));
        }
        
        
    }
    // So the NPC can move around the scene
    protected override void DoMove()
    {
        transform.position += mvelocity * Time.deltaTime;
    }
    // Needs to be called as if its not then the NPC can leave the play area
    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();        
    }

    protected override void ObjectHit(Base_Class other_objects)
    {
        base.ObjectHit(other_objects);
        PC_BC.enabled = false;

        if(PC_BC.enabled == false)
        {
            Destroy(gameObject);

            GameManager.s_GM.SendMessage("ScorePoints", points);
        }
    }
}
