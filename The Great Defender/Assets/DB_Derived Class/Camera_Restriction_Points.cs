﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Restriction_Points : MonoBehaviour
{
    Vector3 Velocity = Vector3.zero;        // Zeros out the velcoity 
    private Transform Player;               // Player Transform IDE Component reference
    private float smoothTime = .30f;        // How fast the Camera moves. Adds a drag effect
    [SerializeField]
    private bool yMaxEnable = false;        // Flag used for how much the player can move up on y axis
    [SerializeField]
    private float yMaxValue = 2.55f;            // values used for how much the player can move up on y axis (Used as coordinates)
    [SerializeField]
    private bool yMinEnable = false;        // Flag used for how much the player can move down on the Y Axis
    [SerializeField]
    private float yMinValue = -2.55f;            // Value of how much the player can move down on the Y axis (Used as coordinates)

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();          // Finding the player GameObject in the world
    }

    private void FixedUpdate()
    {
        Vector3 Playerpos = Player.position;
        // Vertical
        if (yMinEnable && yMaxEnable)           // If yMinEnabe and yMaxEnable are true
            Playerpos.y = Mathf.Clamp(Player.position.y, yMinValue, yMaxValue);     // The playerPos on the y axis clamps depending on the players position and min/Max Y values
        else if (yMinEnable)                   // However if only yMinEnable is true
            Playerpos.y =  Mathf.Clamp(Player.position.y, yMinValue, transform.position.y);     // Playerpos on the y axis is clamped  by only the Minvalue still seeing where the players transform position says
        else if (yMaxEnable)                  // However if only yManEnable is true 
            Playerpos.y = Mathf.Clamp(Player.position.y, transform.position.y, yMaxValue);      // Playerpos on the y axis is clamped by the MaxValue still referencing Players position
        Playerpos.z = transform.position.z;     // Allign the camera and the Players z position
        transform.position = Vector3.SmoothDamp(transform.position, Playerpos, ref Velocity, smoothTime);   // Using Smooth damp we will gradually change the camera transform position to the Players position based on the cameras transform velocity and out smooth time
    }
}