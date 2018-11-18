using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Chaser : Base_Class
{
    [SerializeField]
    private Transform PC;       // Reference to the player

	// Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find Player
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Functions
        DoMove();
        Movement_Restriction();
	}

    protected override void DoMove()
    {
        mvelocity = transform.position = Vector3.MoveTowards(transform.position, PC.position, speed * Time.deltaTime);
    }

    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
    }

    // What needs to be done

    // Make NPC Shoot a Bullet Randomly at the player when within range
    // Make the NPC die
    // Add Points

}
