﻿//Help needed references
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
    [SerializeField]
    private Transform Fire_Pos;


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
        Lazer_Beam();
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
            Debug.Log("We Hit Some Shit");
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