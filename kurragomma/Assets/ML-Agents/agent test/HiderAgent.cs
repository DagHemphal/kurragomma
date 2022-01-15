using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class HiderAgent : Agent
{
    
    public float counter = 10f;
    public GameObject seeker;

    Rigidbody rBody;
    SeekerAgent seeker_script;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        seeker_script = seeker.GetComponent<SeekerAgent>();
    }

    //Körs varjegång agenten hittar target.
    public override void OnEpisodeBegin()
    {
         transform.localPosition = new Vector3(3f, 0.5f, 4f);
    }

    //indata för miljön
    public override void CollectObservations(VectorSensor sensor)
    {
        // Seeker och Agent positions
        sensor.AddObservation(seeker.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(counter);
    }

    private float turnSmoothVelocity;
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //ned räkning för hur lång tid hider har på sig att gömma sig.
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
            if (counter <= 0)
                seeker_script.paused = false;
        }
        else {            
            // Rewards
            SetReward(0.1f);
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