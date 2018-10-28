using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SpaceShip_Controller : Base_Class
{

	// Use this for initialization
	void Start ()
    {
        PC_RB = gameObject.AddComponent<Rigidbody2D>();     // Adding a rigidbody2D component to the gameObject
        PC_RB.isKinematic = true;           // Makes the rigidbody limited with graphics. turns off gravity and mass

        PC_SR = GetComponent<SpriteRenderer>();     // Find the sprite renderer on this gameObject
        Debug.Assert(PC_SR != null, "Sprite Renderer Missing!");        // Calls an error when there is no sprite renderer



        PC_BC = gameObject.AddComponent<BoxCollider2D>();           // Adds a BoxCollider to monitor Collision
        PC_BC.isTrigger = true;         // Makes the box collider attached to gameObject a trigger
        PC_BC.size = new Vector2(0.52f, 0.37f);     // Scales the size of the x and y of the BoxColliders size values 
    }
	
	// Update is called once per frame
	void Update ()
    {
        DoMove();
    }

    protected override void DoMove()
    {
        base.DoMove();

        transform.position += mvelocity * Time.deltaTime;
    }
}
