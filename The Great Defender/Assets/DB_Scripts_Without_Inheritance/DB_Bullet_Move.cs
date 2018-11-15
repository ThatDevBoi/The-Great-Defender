using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_Bullet_Move : Base_Class
{
    [SerializeField]
    private float TravelSpeed = 20f;     // How fast is this bullet going to travel at

    private void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * travelSpeed);      // Moves this gameObject Right with time and a speed value
        PC_BC.isTrigger = true;
    }

    protected override void ObjectHit(Base_Class other_objects)
    {
        base.ObjectHit(other_objects);

        PC_BC.enabled = false;

        if(PC_BC.enabled == false)
        {
            Destroy(gameObject);
        }
    }
}
