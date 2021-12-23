using UnityEngine;

public class grab : MonoBehaviour
{
    public GameObject obj;
    public float max_dist_to_obj = 3f;
    public float seekerFOV = 40f;
    public bool picked = false;


    private Vector3 dist;


    RaycastHit hit;

    public bool IsObjNear() 
    {
    	float dist = Vector3.Distance(obj.transform.position, transform.position);
    	if (dist < max_dist_to_obj)
    		return true;
    	else
    		return false;
    }
    public bool IsObjInFOV() 
    {
    	Vector3 targetdir = obj.transform.position - transform.position;
    	Vector3 forward = transform.forward;
    	float angle = Vector3.Angle(targetdir, forward);
    	if (angle < seekerFOV)    		
    		return true;
    	else 
    		return false;
    }

    public bool IsObjSeen() 
    {
    	Vector3 direction = (obj.transform.position - transform.position).normalized;
    	if(Physics.Raycast(transform.position, direction, out hit, max_dist_to_obj) && hit.collider.gameObject == obj)  
    		return true;
    	else
    		return false;
    }

    void Update () 
    {
    	if (IsObjNear() && IsObjInFOV() && IsObjSeen()) 
    	{
    		if (Input.GetButtonDown("Fire1")) 
    		{
    			dist = obj.transform.position - transform.position;
	    		if (picked) {
	    			GetComponent<Playermovement>().grabing = false;
	    			picked = false;
	    		}
	    		else {
	    			GetComponent<Playermovement>().grabing = true;
	    			picked = true;
	    		}
    		}
    	}
    	if (picked) 
    	{
    		Debug.Log("picked!");
    		float mag = dist.magnitude;
    		if (mag < 1.5f)
    			mag = 1.5f;
	    	obj.transform.position = (mag*transform.forward) + transform.position;
    	}
    		
    }
    	
}
