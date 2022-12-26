using System.Collections;
using System.Collections.Generic;
using FPSController;
using UnityEngine;

public class UpButton : MonoBehaviour, ISelectable
{
    public ShipPrototype ship;

    public void Select() {
        ship.makeBoatUP();
        Debug.Log("UpButton");
    }
}
