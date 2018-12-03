using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Bullet : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 7f;
    private Rigidbody2D rb;
    [SerializeField]
    private Transform PC;
    private Vector2 moveDirection;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();     // Find the Rigidbody2D
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find the Players Transform
        moveDirection = (PC.transform.position - transform.position).normalized * moveSpeed;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
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
