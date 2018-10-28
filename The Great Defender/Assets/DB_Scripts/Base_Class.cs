using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base_Class : MonoBehaviour
{
    #region IDE Components
    protected Rigidbody2D PC_RB;          // Reference to a Physics component Rigidbody2D
    protected BoxCollider2D PC_BC;        // Reference to a Collision Component BoxCollider2D
    protected SpriteRenderer PC_SR;       // Used for debugging the gameObject wont work without this component
    #endregion

    #region Movement Variables
    [SerializeField]
    protected float speed = 5f;           // Movement on the x axis
    [SerializeField]
    protected float Yspeed = 2;             // Movement on the Y axis
    protected Vector3 mvelocity = Vector3.zero;       // A vector3 variable that will decide where the player moves. 
    #endregion


    #region Start Function
    // Use this for initialization
    protected virtual void Start ()
    {
        PC_RB = gameObject.AddComponent<Rigidbody2D>();     // Adding a rigidbody2D component to the gameObject
        PC_RB.isKinematic = true;           // Makes the rigidbody limited with graphics. turns off gravity and mass
        PC_SR = GetComponent<SpriteRenderer>();     // Find the sprite renderer on this gameObject
        Debug.Assert(PC_SR != null, "Sprite Renderer Missing!");        // Calls an error when there is no sprite renderer
        PC_BC = gameObject.AddComponent<BoxCollider2D>();           // Adds a BoxCollider to monitor Collision
        PC_BC.isTrigger = true;         // Makes the box collider attached to gameObject a trigger
        PC_BC.size = new Vector2(0.52f, 0.37f);     // Scales the size of the x and y of the BoxColliders size values 
	}
    #endregion


    #region Update Function
    // Update is called once per frame
    void Update ()
    {
        DoMove();
    }
    #endregion


    #region Do Movement
    protected virtual void DoMove()
    {
        // Moves on the X axis
        float Thrust = Input.GetAxis("Horizontal") * speed * Time.deltaTime;        // Making the thrust variable control the x axis which will move with the speed variable and move with time
        transform.position += mvelocity * Time.deltaTime;       // The transform and position of the gameObject will equal to the Vector3 Variable using fixed time
        mvelocity += Quaternion.Euler(0, 0, transform.rotation.z) * transform.right * Thrust;       // Moves the PC gameObject along the x axis right and left

        // Moves on the Y axis
        float elevate = Input.GetAxis("Vertical") * Yspeed * Time.deltaTime;        // Make Variable control the Y axis using speed with time to move
        mvelocity += Quaternion.Euler(0, 0, 0) * transform.up * elevate;            // Moves the PC gameObject up on the y axis
    }
    #endregion


    #region GameObject Restriction
    protected virtual void Movement_Restriction()
    {
        // When the GameObject moves up or down on the Y axis
        if (transform.position.y <= -7.2f)                                                              // If the Transform component position is more than or equal to -7.5
            transform.position = new Vector3(transform.position.x, -7.2f, transform.position.z);        // The new position for any GameObject will be restricted to -7.5 (Down on the Y axis)
        else if(transform.position.y >= 7.2f)                                                           // However if the transform position is less than 7.5
            transform.position = new Vector3(transform.position.x, 7.2f, transform.position.z);         // The new position of any GameObject is restricted to 7.5 (Up on Y axis)

    }
    #endregion
}
