using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotator : MonoBehaviour {

    private Rigidbody _playerRb;
    private Transform _ship;

    private float _angle;
    private Vector3 _lastForward;
    
    
    void Start() {
        _ship = transform;
        
    }

    void FixedUpdate() {

        if (_playerRb) {
            
            // Get angle between ship.forward last frame and ship.forward this frame
            // This is the delta
            _angle = Vector3.SignedAngle(_lastForward, _ship.forward, Vector3.up);
            
            Debug.Log(_angle);


            // apply delta to player
            var pa = _playerRb.rotation.eulerAngles;
            _playerRb.rotation = Quaternion.Euler(pa.x,
                                                  pa.y + _angle,
                                                  pa.z
            );

            // Debug.Log("Running rotation");
            
            // Save forward from this frame to read next frame
            _lastForward = _ship.forward;

        }
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Debug.Log(other.name + " has entered platform.");
            _playerRb = other.transform.parent.GetComponent<Rigidbody>();
            _lastForward = _ship.forward;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            Debug.Log(other.name + " has exited platform.");

            _playerRb = null;
        }
    }

}
