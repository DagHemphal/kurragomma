using UnityEngine;

public class grab : MonoBehaviour
{
    public GameObject obj;
    public float max_dist_to_obj = 3f;
    public float seekerFOV = 40f;
    public bool picked = false;


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

    Rigidbody trirb;
    public float forcepush = 1f;
    private CharacterController controller;

    void Start() 
    {
    	trirb = obj.GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }


    void Update () 
    {
    	if (IsObjNear() && IsObjInFOV() && IsObjSeen()) 
    	{
    		if (Input.GetButtonDown("Fire1")) 
    		{
	    		if (picked) {
	    			//GetComponent<Playermovement>().grabing = false;
	    			picked = false;
	    		}
	    		else {
	    			//GetComponent<Playermovement>().grabing = true;
	    			picked = true;
	    		}
    		}
    	}
    	if (picked) 
    	{  

            Vector3 horizontalVelocity = controller.velocity;
    		trirb.AddForce(horizontalVelocity);
    	}
        //Debug.Log(controller.velocity);
    		
    }
    	
}
