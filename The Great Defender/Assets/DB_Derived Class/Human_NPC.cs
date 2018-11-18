using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_NPC : Base_Class
{
    public float HumanTimer = 5f;
    public static float Gravity_Value = 1;
    public GameObject NPC_Chaser;
	// Use this for initialization
	void Start ()
    {
        mvelocity = Vector3.left;   // Changes movement vector so it moves left on start
        mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));
        base.Start();
        PC_RB.isKinematic = true;  // This Object needs to use physics
        PC_RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        PC_BC.isTrigger = true;
    }
	
	// Update is called once per frame
	void Update ()
    {

        HumanTimer -= Time.deltaTime;       // Decline timer 1 second Per frame
        DoMove();

        if(transform.position.y >= 8.0)
        {
            Destroy(gameObject);

            GameObject Chaser_NPC = Instantiate(NPC_Chaser, transform.position, Quaternion.identity);
        }
	}

    protected override void DoMove()
    {
        if (HumanTimer <= 0)
        {
            HumanTimer = 5f;
            mvelocity = new Vector2(Random.Range(-speed, speed), Random.Range(0, -0));      // Changes direction on the X axis randomly
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
