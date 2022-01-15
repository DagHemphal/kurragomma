using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SeekerAgent : Agent
{

    public GameObject hider;
    public GameObject wall;
    public float counter_reset = 10f;
    public float max_dist_to_target = 4f;
    public float seekerFOV = 40f;
    public bool paused = true;

    RaycastHit hit;
    Rigidbody rBody;
    HiderAgent hider_script;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        hider_script = hider.GetComponent<HiderAgent>();
    }

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
        //set paus
        paused = true;
        //Flytta till start position
        transform.localPosition = new Vector3(0f, 0.5f, 0f);
    }

    //indata för miljön
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target och Agent positions
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(TargetFound(hider));
        sensor.AddObservation(TargetFound(wall));
        sensor.AddObservation(paused); //not really needed
        sensor.AddObservation(hider_script.counter);
    }

    private float turnSmoothVelocity;
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //väntar tills hider har gömt sig
        if (!paused) {
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
            
            // Reached hider
            if (TargetFound(hider)) {

                //hider agent reward and variable set
                hider_script.counter = counter_reset;
                hider_script.SetReward(-1.0f);
            
                //this agent reward set
                SetReward(1.0f);

                //end both episdoes
                hider_script.EndEpisode();
                EndEpisode();
            }
            else
                SetReward(-0.1f);
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
