using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NPC_Chaser : Base_Class
{
    [SerializeField]
    private int turboPoints = 2;      // Points for Turbo Charge
    [SerializeField]
    private int ScoreBoardPoints = 250;
    [SerializeField]
    private GameObject FlickingTextMesh;
    [SerializeField]
    private Transform PC;       // Reference to the player
    [SerializeField]
    private GameObject Critter_Prefab;
    [SerializeField]
    private Slider ChargeBar;

	// Use this for initialization
    protected override void Start ()
    {
        base.Start();
        PC_BC.isTrigger = true; // Makes Circle Collider Trigger true
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find Player
        ChargeBar = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();  // Find Slider Component
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // Functions
        DoMove();
        Movement_Restriction();

    }

    protected override void DoMove()
    {

        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();      // Find Player
        transform.position += mvelocity * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, PC.position, speed * Time.deltaTime);
    }

    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
            GameObject Crit = Instantiate(Critter_Prefab, transform.position, Quaternion.identity);
            ChargeBar.value += GameManager.s_GM.score;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", ScoreBoardPoints);

            GameObject TextMeshGO = Instantiate(FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
