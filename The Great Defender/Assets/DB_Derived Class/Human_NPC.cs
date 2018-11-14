using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_NPC : Base_Class
{
    public float HumanTimer = 5f;
	// Use this for initialization
	protected virtual void Start ()
    {
        base.Start();
        PC_RB.isKinematic = false;  // This Object needs to use physics
        PC_RB.gravityScale = 0;     // Makes gravity = nothing 
        mvelocity = Vector3.left;   // Changes movement vector so it moves left on start
        mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));
    }
	
	// Update is called once per frame
	void Update ()
    {

        HumanTimer -= Time.deltaTime;       // Decline timer 1 second Per frame

        if(HumanTimer <= 0)
        {
            HumanTimer = 5f;
            mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));      // Changes direction on the X axis randomly
        }
        DoMove();
	}

    protected override void DoMove()
    {
        base.DoMove();
    }

    protected override void ObjectHit(Base_Class other_objects)
    {
        base.ObjectHit(other_objects);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
    }
}
