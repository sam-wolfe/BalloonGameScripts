using System.Collections;
using System.Collections.Generic;
using FPSController;
using UnityEngine;

public class Selector : MonoBehaviour {

    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float range = 10;
    
    void Update() {
        Activate();
    }
    
    void Activate() {

        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, range)) {
            // Debug.Log(hit.transform.gameObject.name);

            ISelectable target = hit.transform.GetComponent<ISelectable>();

            // ReSharper disable once Unity.NoNullPropagation
            target?.Select();

        }
        
    }
}
