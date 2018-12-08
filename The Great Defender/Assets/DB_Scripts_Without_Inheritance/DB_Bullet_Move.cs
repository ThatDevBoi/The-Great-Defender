using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is just used to move the PC bullet GameObject
/// </summary>
public class DB_Bullet_Move : Base_Class
{
    [SerializeField]
    private float fl_TravelSpeed = 40f;     // How fast is this bullet going to travel at

    private void Update()
    {
        // Function
        Movement_Restriction();

        transform.Translate(Vector3.right * Time.deltaTime * fl_TravelSpeed);      // Moves this gameObject Right with time and a speed value
        IDE_PC_BC.isTrigger = false;        // Starts collider as trigger
    }
    // When not rendered with the camera destroy gameObject
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected override void Movement_Restriction()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ways the gameObject can die if it hits any of these objects with the declared tags
        if(other.gameObject.tag == "NPC_Abducter")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "UFO")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "Critter")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "NPC_Chaser")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "Human")
        {
            Destroy(gameObject);
        }
    }
}
