using UnityEngine;

public class collisionhider : MonoBehaviour
{
    public GameObject obj;
    public float max_dist_to_obj = 5.5f;
    public float seekerFOV = 40f;

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

    void Update () {
    	if (IsObjNear() && IsObjInFOV() && IsObjSeen()) {
    		Debug.Log("Found YOU!");
    	}
    		
    }
}

