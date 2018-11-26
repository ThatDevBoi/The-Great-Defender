// Referneces 
// https://www.youtube.com/watch?v=2EFuWjTAqyk&t=250s <-- used to Helped with making the shooting system

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base_Class : MonoBehaviour
{
    #region IDE Components
    protected Rigidbody2D PC_RB;          // Reference to a Physics component Rigidbody2D
    protected Collider2D PC_BC;        // Reference to a Collision Component BoxCollider2D
    protected SpriteRenderer PC_SR;       // Used for debugging the gameObject wont work without this component
    #endregion

    #region Movement Variables PC
    [SerializeField]
    public float speed = 5f;           // Movement on the x axis
    [SerializeField]
    protected float max_Speed = 4;      // Max value for any object referencing to move. Cannot go past this value
    [SerializeField]
    protected float Yspeed = 2;             // Movement on the Y axis
    protected Vector3 mvelocity = Vector3.zero;       // A vector3 variable that will decide where the player moves. 
    [SerializeField]
    protected bool FacingRight = true;      // Player always spawns facing Right
    #endregion

    #region Shooting Variables
    [SerializeField]
    protected float travelSpeed = 10;  // How fast the ray will be moving
    [SerializeField]
    protected float range = 100;       // How far the raycast will travel before it cant go any further
    [SerializeField]
    protected float damage = 10;       // How bad it will hurt any NPC or Humanoid
    [SerializeField]
    protected Transform fire_position; // The origin of the raycast Where a ray is going to shoot from
    [SerializeField]
    protected Transform double_fire_position_1, double_fire_position_2 ;   // Used for when the player decides to shoot a different way. (Double taps input)
    [SerializeField]
    protected LayerMask whatTohit;
    [SerializeField]
    protected GameObject bulletPrefab,chargeShotPrefab;
    [SerializeField]
    protected bool DefaultShot = true;
    [SerializeField]
    protected bool doubleShoot = false;     // Used to shoot 2 raycasts at once instead of the default 1
    [SerializeField]
    protected bool chargeShoot = false;      // Used to fire a larger ray that covers more area
    #endregion

    #region NPC Variables
    protected static int add_points_for_charge = 2;      // Should go in gameManager
    #endregion


    #region Start Function
    // Use this for initialization
    protected virtual void Start ()
    {
        PC_RB = gameObject.AddComponent<Rigidbody2D>();     // Adding a rigidbody2D component to the gameObject
        PC_RB.isKinematic = true;           // Makes the rigidbody limited with graphics. turns off gravity and mass
        PC_SR = GetComponent<SpriteRenderer>();     // Find the sprite renderer on this gameObject
        //Debug.Assert(PC_SR != null, "Sprite Renderer Missing!");        // Calls an error when there is no sprite renderer
        PC_BC = gameObject.GetComponent<Collider2D>();           // Adds a BoxCollider to monitor Collision
        PC_BC.isTrigger = false;         // Makes the box collider attached to gameObject a trigger
        fire_position = GameObject.Find("Fire_Position").GetComponent<Transform>();       // Finds GameObject childed to player called Fire_Position. Needs its Transform
        double_fire_position_1 = GameObject.Find("Fire_Position_Double_Shot_Left_Wing").GetComponent<Transform>();
        double_fire_position_2 = GameObject.Find("Fire_Position_Double_Shot_Right_Wing").GetComponent<Transform>();
    }
    #endregion

    #region Update Function
    // Update is called once per frame
    void Update ()
    {
        DoMove();
        Movement_Restriction();
    }
    #endregion

    #region All Movement Functions
    #region Do Movement
    protected virtual void DoMove()
    {
        // Moves on the X axis
        float Thrust = Input.GetAxis("Horizontal") * speed * Time.deltaTime;        // Making the thrust variable control the x axis which will move with the speed variable and move with time
        transform.position += mvelocity * Time.deltaTime;       // The transform and position of the gameObject will equal to the Vector3 Variable using fixed time
        mvelocity += Quaternion.Euler(0, 0, transform.rotation.z) * transform.right * Thrust;       // Moves the PC gameObject along the x axis right and left
        //Debug.Log(Thrust);        // Shows the Thrust float value *Delete Later*
        if(Thrust > 0f && !FacingRight)     // When float value thrust is greater than 0 and were not facing right Flip the PC
        {
            Flip();     // Calls the flip function to - sclae by 1
        }
        else if (Thrust < 0f && FacingRight)        // However if the Thrust is less than 0 and facing right is true
        {
            Flip();     // Flip Function - 1 scale
        }

        // Moves on the Y axis
        float elevate = Input.GetAxis("Vertical") * Yspeed * Time.deltaTime;        // Make Variable control the Y axis using speed with time to move
        mvelocity += Quaternion.Euler(0, 0, 0) * transform.up * elevate;            // Moves the PC gameObject up on the y axis
    }
    #endregion

    #region Fixed Movement
    protected Vector3 Clamped_Move()
    {
        if(mvelocity.magnitude> max_Speed)  // if minimum speed is more than the max speed value

        {
            return mvelocity.normalized * max_Speed;    // Return the speed value
        }
        return mvelocity; // Return the Vector3 as mvelocity
    }
    #endregion

    #region GameObject Restriction
    protected virtual void Movement_Restriction()
    {
        // When the GameObject moves up or down on the Y axis
        if (transform.position.y <= -8)                                                              // If the Transform component position is more than or equal to -7.5
            transform.position = new Vector3(transform.position.x, -8, transform.position.z);        // The new position for any GameObject will be restricted to -7.5 (Down on the Y axis)
        else if(transform.position.y >= 14f)                                                           // However if the transform position is less than 7.5
            transform.position = new Vector3(transform.position.x, 14f, transform.position.z);         // The new position of any GameObject is restricted to 7.5 (Up on Y axis)

        // Screen Wrapping coordinates (X axis restriction)
        if(transform.position.x >= 50f)     // if the transforms position is greater then 70f
        {
            transform.position = new Vector3(-50f, 0, 0); // Then wrap the object and place gameobject at -70 on the x 
        }
        else if(transform.position.x <= -50f) // However if the transforms position is less than -70f
        {
            transform.position = new Vector3(50f, 0, 0); // Place gameobject at 70 on the x
        }
    }
    #endregion
    #endregion

    #region Fire Bullet Raycast Function
    protected virtual void Lazer_Beam()
    {
        Vector2 firepos = new Vector2(fire_position.position.x, fire_position.position.y);       // Vector2 that holds the fire position positions we can use to shoot from
        Vector2 direction = (FacingRight) ? Vector2.right : Vector2.left;       // The direction we can shoot the raycasts depending where the player is facing
        #region Default Shooting Logic
        if (DefaultShot)
        {
            // Default shooting logic
            RaycastHit2D Hit = Physics2D.Raycast(firepos, direction, range, whatTohit);
            // Collision Detection (For Now)
            if (Hit.collider != null)
            {
                Debug.Log("We hit the fucker");
            }
            Quaternion rot = (FacingRight) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);      // Rot allows the Game to know if the player faces right the bullet wont need rotation however needs to be flipped 180 degrees if its the opposite
            GameObject BulletPre = Instantiate(bulletPrefab, fire_position.position, rot);     // Spawning the bulletPrefab at the fireposition and taking considertion of rot.
            Destroy(BulletPre, 5f);     // Destroy Cloned Prefabs within 5 Seconds

        }
        #endregion
    }
    protected virtual void Double_Lazer_Beam()
    {
        #region Double Shoot Logic
        Vector2 direction = (FacingRight) ? Vector2.right : Vector2.left;       // The direction we can shoot the raycasts depending where the player is facing
        // Double Shooting Logic
        if (doubleShoot)
        {
            DefaultShot = false;
            Vector2 doublefireLeft = new Vector2(double_fire_position_1.position.x, double_fire_position_1.position.y);     // Making a vector2 for the fire point positions for double fire x and y axis
            RaycastHit2D DoubleHitLeft = Physics2D.Raycast(doublefireLeft, direction, range, whatTohit);
            Quaternion rot = (FacingRight) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);      // Rot allows the Game to know if the player faces right the bullet wont need rotation however needs to be flipped 180 degrees if its the opposite
            GameObject _BulletPre = Instantiate(bulletPrefab, double_fire_position_1.position, rot);     // 
            Destroy(_BulletPre, 2f);    // Destrosy prefab clone within 2 seconds
            //Debug.DrawRay(doublefireLeft, direction * range, Color.yellow, 1f);

            if (DoubleHitLeft.collider != null)     // If the ray hits a collider
            {
                Debug.Log("We hit the fucker with 2 BULLETS!!!!! REEEEEEE");        // Message that shows in the console
            }

            Vector2 doublefireRight = new Vector3(double_fire_position_2.position.x, double_fire_position_2.position.y);        // Making a vector2 for the fire point positions for double fire x and y axis
            RaycastHit2D DoubleHitRight = Physics2D.Raycast(doublefireRight, direction, range, whatTohit);      // A raycast that has a fire origin and what direction it can go facing left and right. Also how far the ray can go and making sure the object being hit is a object that can be hit on the layer mask
            GameObject BulletPre_ = Instantiate(bulletPrefab, double_fire_position_2.position, rot);     // Make a bullet to clone from the prefabs and where it'll spawn
            Destroy(BulletPre_, 2f);        // Destrosy prefab clone within 2 seconds
            //Debug.DrawRay(doublefireRight, direction * range, Color.yellow, 1f);
        }
        #endregion
    }
    protected virtual void ChargeShot()
    {
        #region Charge Shot Logic
        Vector2 direction = (FacingRight) ? Vector2.right : Vector2.left;       // The direction we can shoot the raycasts depending where the player is facing
        if (chargeShoot)
        {
            DefaultShot = false;
            Vector2 _Charge_shot = new Vector2(double_fire_position_1.position.x, double_fire_position_1.position.y);     // Making a vector2 for the fire point positions for double fire x and y axis
            RaycastHit2D DoubleHitLeft = Physics2D.Raycast(fire_position.position, direction, range, whatTohit);

            Quaternion rot = (FacingRight) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);      // Rot allows the Game to know if the player faces right the bullet wont need rotation however needs to be flipped 180 degrees if its the opposite
            GameObject _BulletPre = Instantiate(chargeShotPrefab, fire_position.position, rot);     // 
            Destroy(_BulletPre, 2f);    // Destrosy prefab clone within 2 seconds
            //Debug.DrawRay(doublefireLeft, direction * range, Color.yellow, 1f);

            if (DoubleHitLeft.collider != null)     // If the ray hits a collider
            {
                Debug.Log("He's been Game Ended");        // Message that shows in the console
            }
        }
        #endregion
    }
    #endregion

    #region Flipping 
    protected virtual void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }
    #endregion

    //#region Collision Detection
    //protected virtual void ObjectHit(Base_Class other_objects)
    //{
    //    Debug.LogFormat("{0} Hit By 1", name, other_objects);
    //}
    //#endregion

    //#region OnTriggerEnter
    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Base_Class m_otherObjects = collision.gameObject.GetComponent<Base_Class>();
    //    Debug.Assert(m_otherObjects != null, "other Objects is not BaseClass Compatible");  // Appears in console
    //    ObjectHit(m_otherObjects);      // Detects when an object hits the collider
    //}
    //#endregion
}