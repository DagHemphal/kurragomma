using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    public GameObject hider;
    public float max_dist_to_hiden = 5.5f;
    public float seekerFOV = 40f;

    RaycastHit hit;

    public bool IsHiderNear() 
    {
    	float dist = Vector3.Distance(hider.transform.position, transform.position);
    	if (dist < max_dist_to_hiden) {
    		Debug.Log("distance");
    		return true;

    	}
    	return false;
    }
    public bool IsHidenInFOV() 
    {
    	Vector3 targetdir = hider.transform.position - transform.position;
    	Vector3 forward = transform.forward;
    	float angle = Vector3.Angle(targetdir, forward);
    	if (angle < seekerFOV)
    	{
    		Debug.Log("FOV");
    		return true;
    	}
    	else return false;
    }

    public bool IshiderSeen() 
    {
    	Vector3 direction = (hider.transform.position - transform.position).normalized;
    	if(Physics.Raycast(transform.position, direction, out hit, max_dist_to_hiden) && hit.collider.gameObject == hider)
    	{
    		Debug.Log("seee you!");
    		return true;
    	}
    	else
    		return false;
    }

    void Update () {
    	if (IsHiderNear() && IsHidenInFOV())
    		IshiderSeen();
    		
    }
}
