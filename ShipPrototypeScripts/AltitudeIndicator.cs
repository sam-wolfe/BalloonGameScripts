using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudeIndicator : MonoBehaviour
{
    
    [SerializeField]
    // Test for gizmo, remove
    private ShipController ship;
    
    private Vector3 targetAltitude;

    // Update is called once per frame
    void Update() {
        transform.position = ship.targetAltitude;
    }

}
