using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudeIndicator : MonoBehaviour
{
    
    [SerializeField]
    // Test for gizmo, remove
    private ShipController ship;
    
    private Vector3 targetAltitude;
    
    // void Start() {
    //     initPostion();
    // }

    // Update is called once per frame
    void Update() {
        transform.position = ship.targetAltitude;
    }

    // private void initPostion() {
    //     var col = ship.GetComponent<BoxCollider>();
    //
    //     Debug.Log(col.size.x);
    //     Debug.Log(col.size.z);
    //
    //     targetAltitude = new Vector3(
    //         transform.position.x - col.size.x / 2, 0, transform.position.z - col.size.z / 2
    //     );
    // }
}
