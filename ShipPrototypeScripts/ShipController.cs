using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipController : MonoBehaviour {
    
    [SerializeField]
    private float speed = 20f;

    // TODO make interface that inputs share
    [SerializeField] private GamePadInputManager _input;
    
    private Vector2 move;
    private float altitude;

    private Rigidbody rb;

    private Vector3 targetAltitude;

    [SerializeField]
    // Test for gizmo, remove
    private BoxCollider shipFloorCollider;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        setAltitudeGizmoInitialPos();
    }

    void Update() {
        move = _input.move;
        altitude = _input.altitude;
    }

    private void FixedUpdate() {
        moveShipHorizonal();
        moveToTargetAltitude();
        updateTargetAltitue();
    }

    // TODO change this to handle vertical motion, diff method for hor
    private void moveShipHorizonal() {
        Vector3 m_Input = new Vector3(move.x, 0, move.y);
        
        // var new_pos = transform.position + (m_Input * speed * Time.deltaTime);

        rb.AddForce(m_Input * speed, ForceMode.Force);
        
        // NOTE: I think move position will be better for horizontal motion.
        // Oh but maybe not, then we have to manually turn it off if we collide into
        //  a cliff or something. But maybe that wont be so hard and will help prevent
        //  a player getting stuck/gitching through a wall if force is continually
        //  applied by addforce. Needs testing.
    }
    
    private void moveToTargetAltitude() {

        // We only want the difference in altitute between target and 
        // current position, so zero out the other axis's (axees? axies?)
        var heightVector = new Vector3(0, transform.position.y, 0);
        var targetAltitudeY = new Vector3(0, targetAltitude.y, 0);

        var distanceToTarget = targetAltitudeY - heightVector;

        // Debug.Log(distanceToTarget);

        if (distanceToTarget.y > 0) {
            var newPos = distanceToTarget * (speed * Time.deltaTime);
        
            rb.AddForce(newPos * speed, ForceMode.Force);
        }
    }

    private void updateTargetAltitue() {
        var shipPosition = transform.position;
        var col = shipFloorCollider;
        
        targetAltitude = new Vector3(
                shipPosition.x-col.size.x/2, 
                targetAltitude.y + (altitude * speed * Time.deltaTime), 
                shipPosition.z-col.size.z/2
            );
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetAltitude, 1f);
    }

    private void setAltitudeGizmoInitialPos() {
        var col = shipFloorCollider;
        
        Debug.Log(col.size.x);
        Debug.Log(col.size.z);
        
        targetAltitude = new Vector3(
            transform.position.x-col.size.x/2, 0, transform.position.z-col.size.z/2
        );
    }

}
