using UnityEngine;

public class collision : MonoBehaviour
{
    public GameObject obj;
    public float max_dist_to_obj = 5.5f;
    public float seekerFOV = 40f;
    public won Won;
    public grab Grab;

    RaycastHit hit;

    public bool IsHiderNear() 
    {
    	float dist = Vector3.Distance(obj.transform.position, transform.position);
    	if (dist < max_dist_to_obj)
    		return true;
    	else
    		return false;
    }
    public bool IsHidenInFOV() 
    {
    	Vector3 targetdir = obj.transform.position - transform.position;
    	Vector3 forward = transform.forward;
    	float angle = Vector3.Angle(targetdir, forward);
    	if (angle < seekerFOV)    		
    		return true;
    	else 
    		return false;
    }

    public bool IshiderSeen() 
    {
    	Vector3 direction = (obj.transform.position - transform.position).normalized;
    	if(Physics.Raycast(transform.position, direction, out hit, max_dist_to_obj) && hit.collider.gameObject == obj)  
    		return true;
    	else
    		return false;
    }

    void Update () {
    	if (IsHiderNear() && IsHidenInFOV() && IshiderSeen()) {

    		if (obj.name == "triangle")
    			Grab.run();
    		else if(obj.name == "Hider")
    			Won.run();
    	}
    		
    }
}
