using System.Collections;
using System.Collections.Generic;
using FPSController;
using UnityEngine;

public class BackAndForthButton : MonoBehaviour, ISelectable {

    public ShipPrototype ship;
    
    public void Select() {
        ship.BackAndForth = !ship.BackAndForth;
        Debug.Log("BackAndForthButton");
    }
}
