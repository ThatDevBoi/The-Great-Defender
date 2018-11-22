using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Chaser : Base_Class
{
    public int turboPoints = 2;      // Points for Turbo Charge
    [SerializeField]
    private Transform PC;       // Reference to the player
    [SerializeField]
    private GameObject Critter_Prefab;

	// Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true; // Makes Circle Collider Trigger true
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

    // Make the NPC die
    // Add Points

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
            GameObject Crit = Instantiate(Critter_Prefab, transform.position, Quaternion.identity);
            GameManager.score += turboPoints;        // Add points when the NPC dies
        }
    }
}
