using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  The charge shot script is very very small however used in a fashion where nothing can destroy the gameObject. The idea for this was its a super charged blast
///  This can only be spawned by the player when the charge bar slider = a declared value. This is just the class of how it moves
/// </summary>
public class ChargeShot_Bullet : MonoBehaviour
{


    private float TravelSpeed = 25;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.right * TravelSpeed * Time.deltaTime);      // Moves right with speed and time
        Destroy(gameObject, 3f);        // Destroy within 3 seconds
	}
}
