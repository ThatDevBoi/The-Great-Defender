using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Grabber : Base_Class
{
    public int points;

    protected override void Start()
    {
        base.Start();
    }

    public void Update()
    {
        
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
