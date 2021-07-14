using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DebugTools.DebugHelpers;
using System;

public class AbilityDash : Ability
{ 
    public LayerMask hitLayerMask;
    public float dashDistance = 10f;
    public float dashSpeed = 50f;
    Vector3 endPoint;
    Vector3 savedVelocity;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void TriggerAbility()
    {
        Debug.LogError("Triggered Dash ability");
        if(Time.time > aLastStartTime + aBaseCoolDown) // We can trigger this ability
        {
            aLastStartTime = Time.time;
            StartDash();
        }
    }

    public override void StopAbility()
    {
        aExectuting = false;
    }

    void StartDash()
    {
        //Raycast dash distance ahead and see if we hit anything
        Vector3 endCastPoint = transform.position + (aPlayerCameraController.m_CamTran.forward * dashDistance);
        Vector3 originPoint = aPlayerCameraController.m_CamTran.position;
        Vector3 p1 = aPlayerCameraController.m_CamTran.position + cC.center + Vector3.up * -cC.height * 0.5F;
        Vector3 p2 = p1 + Vector3.up * cC.height;

        RaycastHit hit;
        Physics.CapsuleCast(p1, p2, cC.radius, aPlayerCameraController.m_CamTran.forward, out hit, dashDistance);
        if (hit.collider)
        {
            Debug.LogError("We hit at point " + hit.point);
            endPoint = hit.point;
        }
        else
        {
            endPoint = transform.position + (aPlayerCameraController.m_CamTran.forward * dashDistance);
        }
        savedVelocity = cC.velocity;
        aExectuting = true;
    }

    void Update()
    {

        if (Input.GetKeyDown(aAbilityKey))
        {
            TriggerAbility();
        }
        if(aExectuting)
            Dash();
    }

    private void Dash()
    {
        float distance = Vector3.Distance(endPoint, transform.position);
        var offset = endPoint - transform.position;
        if (distance > 2f)
        {
            offset = offset.normalized * dashSpeed;
            cC.Move(offset * Time.deltaTime);
        }
        else
        {
            StopAbility();
        }
    }
}
