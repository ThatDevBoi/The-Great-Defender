﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Space_Ship_Controller : Base_Class
{
    #region Start Function
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
    }
    #endregion


    #region Update Function
    // Update is called once per frame
    void Update ()
    {
        // Functions To Be Called 
        DoMove();
        Movement_Restriction();
	}
    #endregion


    #region Base Class Do Move
    protected override void DoMove()
    {
        base.DoMove();
    }
    #endregion


    protected override void Movement_Restriction()
    {
        base.Movement_Restriction();
    }

}