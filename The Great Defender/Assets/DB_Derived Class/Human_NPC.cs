using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_NPC : Base_Class
{
    public float HumanTimer = 5f;
    public GameObject NPC_Chaser;
    public Transform NPC_Abducter;
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true;
        mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));
    }

    // Update is called once per frame
    void Update ()
    {
        DoMove();
        HumanTimer -= Time.deltaTime;       // Decline timer 1 second Per frame

        if(transform.position.y >= 12)
        {
            Destroy(gameObject);

            GameObject Chaser_NPC = Instantiate(NPC_Chaser, transform.position, Quaternion.identity);
        }
	}

    protected override void DoMove()
    {
        transform.position += mvelocity * Time.deltaTime;
        if (HumanTimer <= 0)
        {
            HumanTimer = 5f;
            mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));      // Changes direction on the X axis randomly
        }
    }

    // Find parent and monitor parent 

    // When Parent Dies Trigger Boolean

    // If boolean true and player triggers NPC 

    // Add Points put Human Position to (0 -8, 0)


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
