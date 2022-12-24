using UnityEngine;
using UnityEngine.InputSystem;

namespace FPSController {

    public class InputManager : MonoBehaviour {

        // Playercontrols is the generated script, being used manually
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

        private void Awake() {
            playerControls = new PlayerControls();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable() {
            playerControls.Enable();
        }

        private void OnDisable() {
            playerControls.Disable();
        }
        
        public void OnLook(InputValue value) {
            look = value.Get<Vector2>();
        }
        
        public void OnMovement(InputValue value) {
            move = value.Get<Vector2>();
        }
        
        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }
        
        public void OnActivate(InputValue value)
        {
            // Do something
        }
        
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
