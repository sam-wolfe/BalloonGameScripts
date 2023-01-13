using System;
using System.Diagnostics;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PlayerInputManager : PlayerControlInputManager {
    
    // TODO test to see if we can remove monobehaviour on this. Maybe
    // TODO  if we just remove the need for the awake function by injecting
    // TODO  the playerinput somewhere.
    
    // TODO Or shit, maybe we don't need currentControlScheme?

    private PlayerInput _playerInput;
    
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
    
    // This class should inherit from an interface so the FPS
    // controller can be sure of the interface
    public string currentControlScheme
    {
        get {
            return _playerInput.currentControlScheme;
        }
    }

}

