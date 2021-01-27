﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dash : MonoBehaviour
{
    public Vector3 dash;
    public const float maxDashTime = 2.5f; // Time we dash
    public float dashDistance = 5f; //Dash distance
    public float dashStoppingSpeed = 0.1f; //For the cooldown
    float currentDashTime = maxDashTime;
    private float dashSpeed = 6; //Dash speed
    public CharacterController controller;
    //Cooldown mechanics sets at 7.5 at default
    public float coolDown = 7.5f;
    public float nextFire = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > nextFire)
        { //Checks input and if the cooldown is ready
            currentDashTime = 0; //Initiate dash time
            nextFire = Time.time + coolDown; //Starts the cooldown
        }

        if (currentDashTime < maxDashTime)

        { 
            if (Input.GetKey(KeyCode.LeftShift)) // GetKey > GetKeyDown for key combinations 
            {
                if (Input.GetKey(KeyCode.A))
                {
                    dash = -transform.right * dashDistance; //Moves left
                } 
                if (Input.GetKey(KeyCode.S))
                {
                    dash = -transform.forward * dashDistance; //Moves backwards
                } 
                if (Input.GetKey(KeyCode.D))
                {
                    dash = transform.right * dashDistance; //Moves right
                }
                else//This is the default (forward) dash, if W or no other key is pressed
                {
                    dash = transform.forward * dashDistance; //Moves forward
                } 
            }
            currentDashTime += dashStoppingSpeed;
        }
    

        
        else
        {
            dash = Vector3.zero; //Stops the player
        }

        controller.Move(dash * Time.deltaTime * dashSpeed); //Moves character controller
    }
}
