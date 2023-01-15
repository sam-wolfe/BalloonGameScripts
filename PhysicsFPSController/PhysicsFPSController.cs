using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PhysicsFPSController : MonoBehaviour
{
    
    [Header("Input Interface")] 
    [SerializeField] private PlayerControlInputManager _input;

    [Header("Player Settings")] 
    [SerializeField] private float speed = 10;
    
    private Rigidbody playerRB;
    
    void Start() {
        
        if (_input == null) throw new Exception("Input manager is not set on FPS controller.");

        playerRB = GetComponent<Rigidbody>();
        if (playerRB == null) throw new Exception(
            "PhysicsFPSController could not get a rigidbody. " +
            "PhysicsFPSController must be attached to a GameObject with a rigidbody.");

    }

    void FixedUpdate() {
        move();
    }

    private void move() {
        var move = _input.move * speed;
        var p = playerRB.gameObject.transform.position;
        
        // TODO clean up
        playerRB.MovePosition(new Vector3(p.x+move.x, p.y, p.z+move.y));
    } 
}
