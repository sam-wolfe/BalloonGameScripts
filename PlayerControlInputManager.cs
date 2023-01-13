using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace {

    // This is a hack to make interface like behaviour available in the inspector
    public abstract class PlayerControlInputManager : MonoBehaviour {

        public Vector2 move { get; protected set; }
        public Vector2 look { get; protected set; }
        public bool jump { get; protected set; }
        public bool sprint { get; protected set; }
        public bool activate { get; protected set; }

    }

}