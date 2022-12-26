using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipPrototype : MonoBehaviour {

    [SerializeField]
    // movement speed of boat
    private float speed = 2f;
    
    // For tracking relative position to apply transforms to after game starts (so boat doesnt
    // drift over time.
    private Vector3 originalPosition;

    // Bools for movement state
    public bool BackAndForth = false;
    public bool goUp = false;

    // Takes target altitude float and makes Vector3 from it for Lerp
    private Vector3 altitude;
    
    // For lerp time
    private float elapsedTime = 0;
    private float timePassed = 0;

    // The target altitude set in inspector
    [SerializeField] private float targetAltitude;
    
    // This is the Lerp ending position
    private Vector3 vectorTargetAltitude;
    
    // This is so the Lerp can have a starting position when the button is pressed
    private Vector3 startAltitudeChangePos;

    private void Start() {
        originalPosition = transform.position;
        startAltitudeChangePos = transform.position;
        vectorTargetAltitude = new Vector3(transform.position.x, targetAltitude, transform.position.z);
    }

    // Update is called once per frame
    void Update() {

        if (BackAndForth) {
            UpdateBackAndForthPosition();
        }

        ProcessAltitudeChange();
    }

    void ProcessAltitudeChange() {

        elapsedTime += Time.deltaTime;
        float percentComplete = elapsedTime / 2;
        if (goUp) {
            altitude = vectorTargetAltitude;
        }
        else {
            altitude = originalPosition;
        }

        if (percentComplete < 1) {
            // This conditional is a hack to not let the position get updated after the 
            // altitude change is complete. Otherwise the ship wont go back and forth.
            transform.position = Vector3.Slerp(startAltitudeChangePos, altitude, percentComplete);
        }
    }
    
    void UpdateBackAndForthPosition() {
        timePassed += Time.deltaTime;

        var _tp = transform.position;

        var only_one = Mathf.Sin(timePassed);
        var newPos = originalPosition.z + (only_one*speed);

        transform.position = 
            // new Vector3((Mathf.Sin(timePassed)*distanceFactor)+_tp.x, _tp.y, _tp.z);
            new Vector3(_tp.x, _tp.y, newPos);
    }

    public void makeBoatUP() {
        if (!goUp) {
            startAltitudeChangePos = transform.position;
            goUp = true;
            elapsedTime = 0;
        }
    }
    
    public void makeBoatDOWN() {
        if (goUp) {
            startAltitudeChangePos = transform.position;
            goUp = false;
            elapsedTime = 0;
        }
    }
    
}
