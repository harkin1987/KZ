using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerMovement;
using Tools;
using System;

public class MovementController : MonoBehaviour, EventListener<GameEvent>
{
    private CharacterController cc;
    private Rigidbody rb;
    private PlayerControllerCC pcCC;
    private PlayerControllerRB pcRB;
    private CapsuleCollider cap;
    private Vector3 savedVelocity;
    ICurrentPhysics currentPhysics;


    void Start()
    {
        currentPhysics = ICurrentPhysics.CharacterController;
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        pcCC = GetComponent<PlayerControllerCC>();
        pcRB = GetComponent<PlayerControllerRB>();
        cap = GetComponent<CapsuleCollider>();
        SetCharacterController(true);
    }


    public void SetCharacterController(bool active)
    {
        //Save the current Velocity
        rb.detectCollisions = !active;
        rb.isKinematic = active;
        //Debug.Log($"isKinematic is set to {rb.isKinematic} - Activate was: {active}");
        cc.enabled = active;
        pcRB.enabled = !active;
        pcCC.enabled = active;
        cap.enabled = !active;
    }

    void SaveCurrentVelocity()
    {
        if(currentPhysics == ICurrentPhysics.CharacterController) {
            savedVelocity = cc.velocity;
            //Debug.Log("Saved velocity of CC:" + cc.velocity);
        }
        else if(currentPhysics == ICurrentPhysics.RigidBody)
        {
            savedVelocity = rb.velocity;
            //Debug.Log("Saved velocity of RB: " + rb.velocity);
        }
    }

    void SetCurrentVelocity()
    {
        if (currentPhysics == ICurrentPhysics.CharacterController)
        {
            //cc.Move(savedVelocity * Time.deltaTime);
            pcCC.m_PlayerVelocity = savedVelocity;
        }
        else if (currentPhysics == ICurrentPhysics.RigidBody)
        {
            rb.velocity = savedVelocity;
        }
    }

    public void OnEvent(GameEvent eventType)
    {
        if(eventType.EventName == "GrappleStart")
        {
            SwitchMovementType(ICurrentPhysics.RigidBody);
        }
        else if (eventType.EventName == "GrappleEnd")
        {
            SwitchMovementType(ICurrentPhysics.CharacterController);
        }
    }

    private void SwitchMovementType(ICurrentPhysics MovementType)
    {
        //Debug.Log($"Switching movement Type to {MovementType} from {currentPhysics}");
        bool successfulSwitch = false;
        SaveCurrentVelocity();
        if (MovementType == ICurrentPhysics.CharacterController && currentPhysics != MovementType)
        {
            SetCharacterController(true);
            currentPhysics = ICurrentPhysics.CharacterController;
            successfulSwitch = true;
        }
        else if(MovementType == ICurrentPhysics.RigidBody && currentPhysics != MovementType)
        {
            SetCharacterController(false);
            currentPhysics = ICurrentPhysics.RigidBody;
            successfulSwitch = true;
        }

        if(successfulSwitch)
            SetCurrentVelocity();
    }

    void OnDisable()
    {
        this.EventStopListening<GameEvent>();
    }

    void OnEnable()
    {
        this.EventStartListening<GameEvent>();
    }
}
