using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class AbilityGrapple : Ability
{
    [Header("SpringValues")]
    public float springForce = 60f;
    public float springDamper = 15f;
    public float massScale = 4.5f;
    [Range(0, 1)]
    public float minDistFromJoint = 0.6f;
    [Range(0, 1)]
    public float maxDistanceFromJoint = 0.8f;
    [Header("GrappleValues")]
    public LayerMask whatIsGrappleable;
    public LayerMask whatIsShouldBlockCasting;
    public Transform gunTip, cam, player;
    public float grappleDistance = 50f;
    public LineRenderer lr;
    private Vector3 grapplePoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    

    public override void Initialize()
    {
        base.Initialize();
        aName = "Grapple";
    }

    public override void TriggerAbility()
    {
        
        if (Time.time > aLastStartTime + aBaseCoolDown) // We can trigger this ability
        {
            base.TriggerAbility();
            aLastStartTime = Time.time;
            StartGrapple();
        }
    }

    public override void StopAbility()
    {
        if (aExectuting)
        {
            aExectuting = false;
            StopGrapple();
        }
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            TriggerAbility();
        }
        if (aExectuting)
            DrawRope();
        if (Input.GetMouseButtonUp(0))
            StopAbility();
    }


    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    void StopGrapple()
    {
        aRB.useGravity = true;
        EventManager.TriggerEvent(new GameEvent("GrappleEnd", IPlayerStates.AirMove));
        lr.positionCount = 0;
        Destroy(joint);
    }

    void StartGrapple()
    {
        RaycastHit hit;
        RaycastHit interferenceHit;
        float radius = 1f;
        //Switched to SphereCast
        //Physics.Raycast(cam.position, cam.forward, out hit, grappleDistance, whatIsGrappleable)
        if (Physics.SphereCast(transform.position, radius, cam.forward, out interferenceHit, grappleDistance, whatIsShouldBlockCasting))
        {
            Debug.Log("Interference hit, cancelling grapple");
            return;
        }
            
        if (Physics.SphereCast(cam.position, radius, cam.forward, out hit, grappleDistance, whatIsGrappleable))
        {

            aExectuting = true;
            aRB.useGravity = false;
            EventManager.TriggerEvent(new GameEvent("GrappleStart", IPlayerStates.Grappling));

            grapplePoint = hit.point;
            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * maxDistanceFromJoint;
            joint.minDistance = distanceFromPoint * minDistFromJoint;

            //Adjust these values to fit your game.
            joint.spring = springForce;
            joint.damper = springDamper;
            joint.massScale = massScale;


            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }
}
