using System.Collections;
using System.Collections.Generic;
using FPSController;
using UnityEngine;

public class DownButton : MonoBehaviour, ISelectable {

    public ShipPrototype ship;

    public void Select() {
        ship.makeBoatDOWN();
        Debug.Log("DownButton");
    }

}