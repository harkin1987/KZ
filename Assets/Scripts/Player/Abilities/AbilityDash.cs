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

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void TriggerAbility()
    {
        if(Time.time > aLastStartTime + aBaseCoolDown) // We can trigger this ability
        {
            base.TriggerAbility();
            aLastStartTime = Time.time;
            StartDash();
        }
    }

    public override void StopAbility()
    {
        aPlayerCharCont.EnableGravity();
        aPlayerCharCont.SwitchActiveLayer(PlayerMovement.PlayerControllerCC.layers.Default);
        aExectuting = false;
    }

    void StartDash()
    {
        //Raycast dash distance ahead and see if we hit anything
        Vector3 endCastPoint = transform.position + (aPlayerCam.m_CamTran.forward * dashDistance);
        Vector3 originPoint = aPlayerCam.m_CamTran.position;
        Vector3 p1 = transform.position + cC.center + Vector3.down * ((cC.height +  cC.skinWidth) * 0.5F);
        Vector3 p2 = transform.position + cC.center + Vector3.up * ((cC.height + cC.skinWidth) * 0.5F);

        RaycastHit hit;
        //public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance);
        Physics.CapsuleCast(p1, p2, (cC.radius + cC.skinWidth), aPlayerCam.m_CamTran.forward, out hit, dashDistance, hitLayerMask);
        if (hit.collider)
        {
            endPoint = hit.point;
        }
        else
        {
            Vector3 normCamForward = Vector3.Normalize(aPlayerCam.m_CamTran.forward);
            endPoint = transform.position + (normCamForward * dashDistance);
            RenderVolume(endPoint);
        }
        aPlayerCharCont.DisableGravity();
        aPlayerCharCont.SwitchActiveLayer(PlayerMovement.PlayerControllerCC.layers.Dashing);
        aExectuting = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(endPoint, 1f);
    }

 


    void Update()
    {
        if (Input.GetKeyDown(aAbilityKey))
        {
            TriggerAbility();
        }
        if (aExectuting)
            Dash();
    }

    private void Dash()
    {
        if (aExectuting)
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

    void RenderVolume(Vector3 location)
    {
        shape.position = location;
        shape.GetComponent<Renderer>().enabled = true; // show it
    }

    public Transform shape;

}
