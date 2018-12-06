using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if(PC == null && GameManager.s_GM.bl_Player_Dead == false)
        {
            PC = GameObject.Find("PC(Clone)").GetComponent<Transform>();      // Find the Players Transform 
        }

        if(PC != null && GameManager.s_GM.bl_Player_Dead == false)
        {
            moveDirection = (PC.transform.position - transform.position).normalized * fl_moveSpeed;
        }
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
    }

    private void Update()
    {

        if (PC == null && GameManager.s_GM.bl_Player_Dead == true)
        {
            Debug.Log("NPC Bullet Script__We Dont Have The Players Transform");
        }

        if (GameManager.s_GM.bl_Player_Dead == true)
        {
            Destroy(gameObject);
        }
        else
            return;
    }

    public void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Hit");
            Destroy(gameObject);
        }
    }
}
