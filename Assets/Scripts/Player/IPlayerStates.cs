using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IPlayerStates
{
     AirMove,
     GroundMove,
     Grappling,
     Dashing
}


public enum ICurrentPhysics
{
    RigidBody,
    CharacterController
}