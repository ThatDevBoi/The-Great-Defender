using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NPC_UFO : Base_Class
{
    [SerializeField]
    public int turboShotPoints = 1;
    [SerializeField]
    public int ScoreBoardPoints = 500;
    [SerializeField]
    private GameObject FlickingTextMesh;
    [SerializeField]
    private float Timer = .5f;
    [SerializeField]
    private float LifeTimer = 20;
    [SerializeField]
    private float nextFire;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Slider ChargeBar;


    // Use this for initialization
    protected override void Start ()
    {
        base.Start();   // Calls Base Class Start Function
        PC_BC.isTrigger = true;     // Collider attached to this gameObject is a trigger
        mvelocity = new Vector2(Random.Range(speed, speed), Random.Range(-speed, speed));   // On start computer decides a random position and movement speed
        ChargeBar = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();  // Find Slider Component

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
         //Functions
        DoMove();
        Movement_Restriction();
        LifeTimer -= Time.deltaTime;        // When spawned Decrease the timer float
        Timer -= Time.deltaTime;        // Decrease Float with time
        if (Timer <= 0)
        {
            mvelocity = new Vector2(Random.Range(speed, speed), Random.Range(-speed, speed));   // Can change the speed at random value when timer is 0
            Timer = .5f;  // Reset timer
        }

        if(LifeTimer <= 0)      // When timer hits 0
        {
            Destroy(gameObject);        // Destroy this gameObject
        }
    }

    protected override void DoMove()
    {
        transform.position += mvelocity * Time.deltaTime;
    }

    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();

        // Movement Restrictions need refining
    }


    public void OnBecameVisible()
    {
        if (Time.time > nextFire)
        {
            Instantiate(bullet, transform.position, Quaternion.identity);       // Spawn Bullet within the gameObjects transform position
            nextFire = Time.time + fireRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);        // Kills UFO
            ChargeBar.value += GameManager.score;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", ScoreBoardPoints);

            GameObject TextMeshGO = Instantiate(FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
