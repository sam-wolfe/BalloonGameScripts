using FPSController;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour {

    [Tooltip("POV view of player")]
    [SerializeField] private Camera _playerCamera;
    
    // Player input is the interface from unity to access the PlayerControls object
    // This gives us access to the events for the actions, like OnLook
    [Tooltip("Input Interface")]
    [SerializeField] private PlayerInput _playerInput;

    private InputAction ActivateAction;
    
    [Tooltip("Distance Player can activate selected object")]
    [SerializeField] private float range = 10;

    private ISelectable target;

    private void Start() {
        // Since we are using the PlayerInput class, so we have to (not really but its simpler) 
        // use this string based way to get an action from it. We could avoid this by
        // instantiating the PlayerControls InputActions class ourselves.
        ActivateAction = _playerInput.actions.FindAction("Activate");
        ActivateAction.started += Activate;
        
        Debug.Log(_playerInput.currentActionMap.actions);


        // TODO || this is an alternative syntax, accessing the current action map 
        // TODO ||  directly. I need to find out the pros/cons of this over FindAction.
        // TODO ||  It may just be that FindAction can return null/throw an exception via arg
        // ActivateAction = _playerInput.currentActionMap["Activate"];
        // ActivateAction.started += Activate;
    }

    private void OnValidate() {
        // Awesome method, need to use more
    }

    void Update() {
        CheckTarget();
    }

    void Activate(InputAction.CallbackContext ctx) {
        // Activate selectable object when action performed.
        target?.Select();
        Debug.Log("Select action.");
    }
    
    void CheckTarget() {
        // Check every frame if we are looking at something selectable, if so then make
        // it the target. If not, clear the target to null.

        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, range)) {
            // Debug.Log(hit.transform.gameObject.name);

            target = hit.transform.GetComponent<ISelectable>();

            if (target != null) {
                // Debug.Log(hit.transform.gameObject.name);
            }
        }
        
    }
}
