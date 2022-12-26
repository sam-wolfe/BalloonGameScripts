using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace FPSController {

    public class InputManager : MonoBehaviour {

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
            // playerControls = new PlayerControls();   
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable() {
            // playerControls.Enable();
        }

        private void OnDisable() {
            // playerControls.Disable();
        }
        
        public void OnLook(InputValue value) {
            look = value.Get<Vector2>();
        }
        
        public void OnLook(InputAction.CallbackContext context) {
            look = context.ReadValue<Vector2>();
            // Debug.Log($"lookvalue: {look}");
        }
        
        public void OnMovement(InputValue value) {
            move = value.Get<Vector2>();
            // Debug.Log("send messages");
        }
        
        public void OnMovement(InputAction.CallbackContext context) {
            move = context.ReadValue<Vector2>();
            // Debug.Log("unity events");
        }
        
        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }
        
        public void OnJump(InputAction.CallbackContext context) {
            jump = true;
        }
        
        // public void OnActivate(InputAction.CallbackContext context)
        // {
        //     // activate = value.isPressed;
        //     switch (context.phase) {
        //         case InputActionPhase.Started:
        //             activate = true;
        //             break;
        //         case InputActionPhase.Canceled:
        //             activate = false;
        //             break;
        //         case InputActionPhase.Performed:
        //             activate = false;
        //             break;
        //     }
        // }
        
        public void OnSprint(InputValue value)
        {
            sprint = value.isPressed;
        }
        
        // This class should inherit from an interface so the FPS controller can be sure of the interface
        public string currentControlScheme
        {
            get {
                return _playerInput.currentControlScheme;
            }
        }

    }

}
