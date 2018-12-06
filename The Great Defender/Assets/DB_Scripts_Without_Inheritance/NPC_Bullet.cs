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
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find the Players Transform
        moveDirection = (PC.transform.position - transform.position).normalized * fl_moveSpeed;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
	}

    private void Update()
    {
        if (GameManager.s_GM.bl_Player_Dead)
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
