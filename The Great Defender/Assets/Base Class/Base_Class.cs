// Referneces 
// https://www.youtube.com/watch?v=2EFuWjTAqyk&t=250s <-- used to Helped with making the shooting system

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class will be ther Base for all over Dervied classes to inherite from. In this class i made movement for a PC and NPC to use. A shooting function with Movement Restrictions
/// The class also holds references to important IDE Componenets like a Rigidbody2D and Collider2D so each Dervied class can have a applied IDE component however may need more if needed
/// This class cant be applied to a GameObject its jut used for inheritance 
/// </summary>
public abstract class Base_Class : MonoBehaviour
{
    #region Variables
    // IDE Components
    protected Rigidbody2D IDE_PC_RB;          // Reference to a Physics component Rigidbody2D
    protected Collider2D IDE_PC_BC;        // Reference to a Collision Component BoxCollider2D
    protected SpriteRenderer IDE_PC_SR;       // Used for debugging the gameObject wont work without this component
    [SerializeField]
    protected LayerMask IDE_layerMask_whatTohit;  // LayerMask to define what a raycast can hit (Used For Shooting Logic)
    [SerializeField]
    protected Transform IDE_trans_fire_position; // The origin of the raycast Where a ray is going to shoot from (Shooting IDE Component)
    [SerializeField]
    protected Transform IDE_trans_double_fire_position_1, IDE_trans_double_fire_position_2;   // Used for when the player decides to shoot a different way. (Double taps input) (Shooting IDE Components)


    // floats
    [SerializeField]
    public float fl_movement_speed = 5f;           // Movement on the x axis (Moving Variable)
    [SerializeField]
    protected float fl_max_movement_Speed = 4;      // Max value for any object referencing to move. Cannot go past this value (Moving Variable)
    [SerializeField]
    protected float fl_movement_Yspeed = 2;             // Movement on the Y axis (Moving Variable)
    [SerializeField]
    protected float fl_range = 100;       // How far the raycast will travel before it cant go any further (Shooting Variable)

    // bools
    protected bool bl_FacingRight = true;      // Player always spawns facing Right (Used instead of switching sprites just flip the current sprite) (Movement Bool)
    protected bool bl_DefaultShot = true;      // Regular shooting just tapping space bar (Shooting Bool)
    protected bool bl_doubleShoot = false;     // Used to shoot 2 GameObjects at once instead of the just 1 GameObject (Shooting Bool)
    protected bool bl_chargeShoot = false;      // Used to fire a larger ray that covers more area (Shooting Bool)
    // GameObjects
    [SerializeField]
    protected GameObject GO_bulletPrefab, GO_chargeShotPrefab;     // The 2 types of GameObjects that the PC can use to shoot with (Shooting GameObjects)

    // Vectors
    protected Vector3 mvelocity = Vector3.zero;       // A vector3 variable that will decide where the player moves. (Movement Variable)
    #endregion

    #region Start Function
    // Use this for initialization
    protected virtual void Start ()
    {
        IDE_PC_RB = gameObject.AddComponent<Rigidbody2D>();     // Adding a rigidbody2D component to the gameObject
        IDE_PC_RB.isKinematic = true;           // Makes the rigidbody limited with graphics. turns off gravity and mass
        IDE_PC_SR = GetComponent<SpriteRenderer>();     // Find the sprite renderer on this gameObject
        //Debug.Assert(PC_SR != null, "Sprite Renderer Missing!");        // Calls an error when there is no sprite renderer
        IDE_PC_BC = gameObject.GetComponent<Collider2D>();           // Adds a BoxCollider to monitor Collision
        IDE_PC_BC.isTrigger = false;         // Makes the box collider attached to gameObject a trigger
        bl_DefaultShot = true;
        bl_doubleShoot = false;
        bl_chargeShoot = false;
    }
    #endregion

    #region Update Function
    // Update is called once per frame
    void Update ()
    {
    // Functions
        DoMove();
        Movement_Restriction();
    }
    #endregion

    #region All Movement Functions
    
    #region Do Movement
    protected virtual void DoMove()
    {
        // Moves on the X axis holding player input
        float Thrust = Input.GetAxis("Horizontal") * fl_movement_speed * Time.deltaTime;        // Making the thrust variable control the x axis which will move with the speed variable and move with time
        transform.position += mvelocity * Time.deltaTime;       // The transform and position of the gameObject will equal to the Vector3 Variable using fixed time
        mvelocity += Quaternion.Euler(0, 0, transform.rotation.z) * transform.right * Thrust;       // Moves the PC gameObject along the x axis right and left
        if(Thrust > 0f && !bl_FacingRight)     // When float value thrust is greater than 0 and were not facing right Flip the PC
        {
            Flip();     // Calls the flip function to - sclae by 1
        }
        else if (Thrust < 0f && bl_FacingRight)        // However if the Thrust is less than 0 and facing right is true
        {
            Flip();     // Flip Function - 1 scale
        }

        // Moves on the Y axis
        float elevate = Input.GetAxis("Vertical") * fl_movement_Yspeed * Time.deltaTime;        // Make Variable control the Y axis using speed with time to move
        mvelocity += Quaternion.Euler(0, 0, 0) * transform.up * elevate;            // Moves the PC gameObject up on the y axis
    }
    #endregion

    #region Fixed Movement
    protected virtual Vector3 Clamped_Move()
    {
        if(mvelocity.magnitude > fl_max_movement_Speed)  // if minimum speed is more than the max speed value

        {
            return mvelocity.normalized * fl_max_movement_Speed;    // Return the speed value
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
        else if(transform.position.y >= 8.5f)                                                           // However if the transform position is less than 7.5
            transform.position = new Vector3(transform.position.x, 8.5f, transform.position.z);         // The new position of any GameObject is restricted to 7.5 (Up on Y axis)

        // Screen Wrapping coordinates (X axis restriction)
        if (transform.position.x >= 50f)     // if the transforms position is greater then 50
        {
            transform.position = new Vector3(-50f, transform.position.y, transform.position.z); // Then wrap the object and place gameobject at -50 on the x 
        }
        else if (transform.position.x <= -50f) // However if the transforms position is less than -50
        {
            transform.position = new Vector3(50, transform.position.y, transform.position.z); // Place gameobject at 50 on the x
        }
    }
    #endregion
    #endregion

    #region Fire Bullet Raycast Function
    protected virtual void Lazer_Beam()
    {
        Vector2 firepos = new Vector2(IDE_trans_fire_position.position.x, IDE_trans_fire_position.position.y);       // Vector2 that holds the fire position positions we can use to shoot from
        Vector2 direction = (bl_FacingRight) ? Vector2.right : Vector2.left;       // The direction we can shoot the raycasts depending where the player is facing
        #region Default Shooting Logic
        if (bl_DefaultShot)
        {
            // Default shooting logic
            RaycastHit2D Hit = Physics2D.Raycast(firepos, direction, fl_range, IDE_layerMask_whatTohit);
            Quaternion rot = (bl_FacingRight) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);      // Rot allows the Game to know if the player faces right the bullet wont need rotation however needs to be flipped 180 degrees if its the opposite
            GameObject BulletPre = Instantiate(GO_bulletPrefab, IDE_trans_fire_position.position, rot);     // Spawning the bulletPrefab at the fireposition and taking considertion of rot.
            Destroy(BulletPre, 5f);     // Destroy Cloned Prefabs within 5 Seconds

        }
        #endregion
    }
    protected virtual void Double_Lazer_Beam()
    {
        #region Double Shoot Logic
        Vector2 direction = (bl_FacingRight) ? Vector2.right : Vector2.left;       // The direction we can shoot the raycasts depending where the player is facing
        // Double Shooting Logic
        if (bl_doubleShoot)
        {
            bl_DefaultShot = false;
            Vector2 doublefireLeft = new Vector2(IDE_trans_double_fire_position_1.position.x, IDE_trans_double_fire_position_1.position.y);     // Making a vector2 for the fire point positions for double fire x and y axis
            RaycastHit2D DoubleHitLeft = Physics2D.Raycast(doublefireLeft, direction, fl_range, IDE_layerMask_whatTohit);
            Quaternion rot = (bl_FacingRight) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);      // Rot allows the Game to know if the player faces right the bullet wont need rotation however needs to be flipped 180 degrees if its the opposite
            GameObject _BulletPre = Instantiate(GO_bulletPrefab, IDE_trans_double_fire_position_1.position, rot);     // 
            Destroy(_BulletPre, 2f);    // Destrosy prefab clone within 2 seconds

            Vector2 doublefireRight = new Vector3(IDE_trans_double_fire_position_2.position.x, IDE_trans_double_fire_position_2.position.y);        // Making a vector2 for the fire point positions for double fire x and y axis
            RaycastHit2D DoubleHitRight = Physics2D.Raycast(doublefireRight, direction, fl_range, IDE_layerMask_whatTohit);      // A raycast that has a fire origin and what direction it can go facing left and right. Also how far the ray can go and making sure the object being hit is a object that can be hit on the layer mask
            GameObject BulletPre_ = Instantiate(GO_bulletPrefab, IDE_trans_double_fire_position_2.position, rot);     // Make a bullet to clone from the prefabs and where it'll spawn
            Destroy(BulletPre_, 2f);        // Destroy prefab clone within 2 seconds
        }
        #endregion
    }
    protected virtual void ChargeShot()
    {
        #region Charge Shot Logic
        Vector2 direction = (bl_FacingRight) ? Vector2.right : Vector2.left;       // The direction we can shoot the raycasts depending where the player is facing
        if (bl_chargeShoot)
        {
            bl_DefaultShot = false;
            Vector2 _Charge_shot = new Vector2(IDE_trans_double_fire_position_1.position.x, IDE_trans_double_fire_position_1.position.y);     // Making a vector2 for the fire point positions for double fire x and y axis
            RaycastHit2D DoubleHitLeft = Physics2D.Raycast(IDE_trans_fire_position.position, direction, fl_range, IDE_layerMask_whatTohit);

            Quaternion rot = (bl_FacingRight) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);      // Rot allows the Game to know if the player faces right the bullet wont need rotation however needs to be flipped 180 degrees if its the opposite
            GameObject _BulletPre = Instantiate(GO_chargeShotPrefab, IDE_trans_fire_position.position, rot);     // 
            Destroy(_BulletPre, 2f);    // Destrosy prefab clone within 2 seconds
        }
        #endregion
    }
    #endregion

    #region Flipping 
    protected virtual void Flip()
    {
        bl_FacingRight = !bl_FacingRight; 
        Vector3 temp = transform.localScale;    // new Vector3 variable that equals the transform scale which is relevant to the parent gameObject
        temp.x *= -1;   // Revert temps scale on the x axis by -1 flips left
        transform.localScale = temp;    // Make sure that the transform of the parents scale equals temp Vector Value
    }
    #endregion
}
