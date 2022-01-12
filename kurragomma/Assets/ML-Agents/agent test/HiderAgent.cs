using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class HiderAgent : Agent
{
    Rigidbody rBody;
    public float counter = 10f;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public GameObject seeker;
    public float max_dist_to_target = 4f;
    public float seekerFOV = 40f;

    RaycastHit hit;

    public bool IsSeekerNear(GameObject target) 
    {
        float dist = Vector3.Distance(target.transform.position, transform.position);
        if (dist < max_dist_to_target)
            return true;
        else
            return false;
    }
    public bool IsAgentInFOV(GameObject target) 
    {
        Vector3 targetdir = transform.position - target.transform.position;
        Vector3 forward = target.transform.forward;
        float angle = Vector3.Angle(targetdir, forward);
        if (angle < seekerFOV)          
            return true;
        else 
            return false;
    }

    public bool IsAgentSeen(GameObject target) 
    {
        Vector3 direction = (transform.position - target.transform.position).normalized;
        if(Physics.Raycast(target.transform.position, direction, out hit, max_dist_to_target) && hit.collider.gameObject == gameObject)  
            return true;
        else
            return false;
    }

    //Körs varjegång agenten hittar target.
    public override void OnEpisodeBegin()
    {
        //stop seeker agent
        seeker.GetComponent<SeekerAgent>().enabled = false;

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
        // Seeker and Agent positions
        sensor.AddObservation(seeker.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
    }

    private float turnSmoothVelocity;
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        
        if(counter > 0)
        {
            counter -= Time.deltaTime;
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
        }
        else {
            //start seeker agent
            seeker.GetComponent<SeekerAgent>().enabled = true;
            // Rewards
            // hider have been found
            if (IsSeekerNear(seeker) && IsAgentInFOV(seeker) && IsAgentSeen(seeker))
            {
                //flytta seeker till start position
                seeker.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                transform.localPosition = new Vector3(3f, 0.5f, 4f);
                counter = 5;
                SetReward(-1.0f);
                EndEpisode();
            }
            else {
                SetReward(0.1f);
            }
        }       
    }

    //test för styrning med keyboard
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}