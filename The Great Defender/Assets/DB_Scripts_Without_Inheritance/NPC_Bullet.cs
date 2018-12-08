using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is just to allow the NPC bullet to move towards the player
/// Apply it to any GameObject you wish to classify as NPC_Bullet
/// </summary>
public class NPC_Bullet : MonoBehaviour
{

    private Rigidbody2D rb;
    [SerializeField]
    private Transform PC;   // Players Current Position

    [SerializeField]
    private float fl_moveSpeed = 7f;    // Move Speed of this GamObject
    

    private Vector2 moveDirection;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();     // Find the Rigidbody2D

        if(PC == null && GameManager.s_GM.bl_Player_Dead == false)  // when the PC Transform component is not found and the player is alive
        {
            PC = GameObject.Find("PC(Clone)").GetComponent<Transform>();      // Find the Players Transform 
        }

        if(PC != null && GameManager.s_GM.bl_Player_Dead == false)      // when we have the Transform Componenet and the player is alive
        {
            // move depending on where the player is - where this current object is
            moveDirection = (PC.transform.position - transform.position).normalized * fl_moveSpeed;
        }
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);    // Rigidbody2D velocity moves on the x and y directions
    }

    private void Update()
    {

        if (PC == null && GameManager.s_GM.bl_Player_Dead == true)      // when there is no Plaer Transform and the Player is dead
        {
            Debug.Log("NPC Bullet Script__We Dont Have The Players Transform"); // Show in console
        }

        if (GameManager.s_GM.bl_Player_Dead == true)    // when the players dead
        {
            Destroy(gameObject);        // Destroy me
        }
        else
            return;
    }

    public void OnBecameInvisible()
    {
        Destroy(gameObject);        // When the GameObject is not rendered within the camera then we remove from the scene
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")    // When the bullet hits the player
        {
            Destroy(gameObject);    // destroy gameObject
        }
    }
}
