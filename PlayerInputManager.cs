using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PlayerInputManager : MonoBehaviour {

    // Playercontrols is the generated script, being used manually. 
    // We dont use this here, as we are using the higher level abstration:
    // PlayerInput. Instantiating it ourselves would give us better
    // control however.
    private PlayerControls playerControls;
    
    // Player input is the interface from unity to access the PlayerControls object
    // This gives us access to the events for the actions, like OnLook
    private PlayerInput _playerInput;
    
    [Header("Character Input Values")]
    [HideInInspector]
    public Vector2 move;
    [HideInInspector]
    public Vector2 look;
    [HideInInspector]
    public bool jump;
    [HideInInspector]
    public bool sprint;
    [HideInInspector]
    public bool activate;

    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
    }
    
    public void OnLook(InputAction.CallbackContext context) {
        look = context.ReadValue<Vector2>();
    }

    public void OnMovement(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context) {
        jump = true;
    }
    
    public void OnSprint(InputAction.CallbackContext context) {
        sprint = true;
    }
    
    // This class should inherit from an interface so the FPS controller can be sure of the interface
    public string currentControlScheme
    {
        get {
            return _playerInput.currentControlScheme;
        }
    }

}

