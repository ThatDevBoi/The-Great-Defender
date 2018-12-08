using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This Dervied class makes a UFO NPC move around the scene when spawned declared by the GameManager
/// It moves fast in different directions and can screenwrap keeping to the Movement Restrictions set
/// This Dervied Class Can Be Applied to a GameObject With the Tag UFO
/// </summary>
public class NPC_UFO : Base_Class
{
    // IDE
    [SerializeField]
    private Slider IDE_ChargeBarSlider;

    // ints
    private int int_turboShotPoints = 1;     // int value that makes charge shot slider value go up
    private int int_ScoreBoardPoints = 500;     // Score that shows on TextMesh GameObject and shown on the scoreboard

    // Floats
    private float fl_next_time_to_move = 0.5f;   // float value to allow to gameObject to change direction when the value hits 
    private float fl_UFO_LifeTimer = 20;    // When this float value hits 0 the gameObject dies

    // GameObjects
    [SerializeField]
    private GameObject GO_FlickingTextMesh; // TeshMesh GameObject that appears when the player kills this gameObject
    [SerializeField]
    private GameObject GO_bullet;       // Bullet GameObject that this gameObject shoots at the player


    // Use this for initialization
    protected override void Start ()
    {
        base.Start();   // Calls Base Class Start Function
        IDE_PC_BC.isTrigger = true;     // Collider attached to this gameObject is a trigger
        mvelocity = new Vector2(Random.Range(fl_movement_speed, fl_movement_speed), Random.Range(-fl_movement_speed, fl_movement_speed));   // On start computer decides a random position and movement speed
        IDE_ChargeBarSlider = GameObject.FindGameObjectWithTag("Turbo_Shot_Bar").GetComponent<Slider>();  // Find Slider Component needs to be updated UI slider isnt active on start
        if (GameManager.int_Player_Lives == -1)
            IDE_ChargeBarSlider = null;

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {

        //Functions
        DoMove();
        Movement_Restriction();
        fl_UFO_LifeTimer -= Time.deltaTime;        // When spawned Decrease the timer float
        fl_next_time_to_move -= Time.deltaTime;        // Decrease Float with time
        if (fl_next_time_to_move <= 0)
        {
            mvelocity = new Vector2(Random.Range(fl_movement_speed, fl_movement_speed), Random.Range(-fl_movement_speed, fl_movement_speed));   // Can change the speed at random value when timer is 0
            fl_next_time_to_move = .5f;  // Reset timer
        }

        if(GameManager.s_GM.bl_Player_Dead == true)
        {
            fl_movement_speed = 0;
            fl_max_movement_Speed = 0;
        }
        else if(GameManager.s_GM.bl_Player_Dead == false)
        {
            fl_movement_speed = 2;
            fl_max_movement_Speed = 7;
        }

        if(fl_UFO_LifeTimer <= 0)      // When timer hits 0
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
    }


    public void OnBecameVisible()
    {
        Instantiate(GO_bullet, transform.position, Quaternion.identity);       // Spawn Bullet within the gameObjects transform position 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);        // Kills UFO
            IDE_ChargeBarSlider.value += GameManager.s_GM.int_turbo_Shot_score_monitor;       // Add int value to charge bar value
            GameManager.s_GM.SendMessage("Leader_Board_Score", int_ScoreBoardPoints);

            GameObject TextMeshGO = Instantiate(GO_FlickingTextMesh, transform.position, Quaternion.identity); // Spawn Text Mesh Object
            TextMeshGO.GetComponent<TextMesh>().text = int_ScoreBoardPoints.ToString();   // Find the Text Mesh Component so the score can be shown 
            Destroy(TextMeshGO, 1.25f); // Destroy when 1.25 seconds have passed
        }
    }
}
