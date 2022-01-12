using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SeekerAgent : Agent
{
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public GameObject hider;
    public GameObject wall;
    public float max_dist_to_target = 4f;
    public float seekerFOV = 40f;

    RaycastHit hit;

    public bool IsTargetNear(GameObject target) 
    {
        float dist = Vector3.Distance(target.transform.position, transform.position);
        if (dist < max_dist_to_target)
            return true;
        else
            return false;
    }
    public bool IsTargetInFOV(GameObject target) 
    {
        Vector3 targetdir = target.transform.position - transform.position;
        Vector3 forward = transform.forward;
        float angle = Vector3.Angle(targetdir, forward);
        if (angle < seekerFOV)          
            return true;
        else 
            return false;
    }

    public bool IsTargetSeen(GameObject target) 
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        if(Physics.Raycast(transform.position, direction, out hit, max_dist_to_target) && hit.collider.gameObject == target)  
            return true;
        else
            return false;
    }

    public bool TargetFound(GameObject target) {
        return (IsTargetNear(target) && IsTargetInFOV(target) && IsTargetSeen(target));
    }

    //Körs varjegång agenten hittar hider
    public override void OnEpisodeBegin()
    {

        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 0, 0.5f, 0);
        }

        

    }

    //indata för miljön
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(TargetFound(hider));
        sensor.AddObservation(TargetFound(wall));
    }

    private float turnSmoothVelocity;
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        float horizontal = actionBuffers.ContinuousActions[0];
        float vertical = actionBuffers.ContinuousActions[1];
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            rBody.AddForce(direction * forceMultiplier);
        }

        // Rewards
        // Reached hider
        if (TargetFound(hider)) {
            SetReward(1.0f);

            //Flytta hider till start position
            hider.transform.localPosition = new Vector3(3f, 0.5f, 4f);
            transform.localPosition = new Vector3(0f, 0.5f, 0f);
            EndEpisode();
        }
        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            SetReward(-0.2f);
            EndEpisode();
        }
        else
            SetReward(-0.1f);

    }

    //test för styrning med keyboard
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
