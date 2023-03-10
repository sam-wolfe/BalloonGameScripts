using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadInputManager : MonoBehaviour {

    public float altitude { get; private set; }
    public Vector2 move { get; private set; }
    public float sails { get; private set; }
    
    public void OnAltitude(InputAction.CallbackContext context) {
        float newAltitude = context.ReadValue<float>();

        altitude = newAltitude;
        // Debug.Log(altitude);
    }
    
    public void OnMove(InputAction.CallbackContext context) {
        Vector2 newMovement = context.ReadValue<Vector2>();

        move = newMovement;
        // Debug.Log(move);
    }
    
    public void OnSail(InputAction.CallbackContext context) {
        sails = context.ReadValue<float>();;
        // Debug.Log(sails);
    }
}
