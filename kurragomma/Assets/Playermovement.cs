using UnityEngine;

public class Playermovement : MonoBehaviour
{
    // Start is called before the first frame update

    
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;
    public bool grabing = false;

    private float turnSmoothVelocity; 
    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
    	float horizontal = Input.GetAxisRaw("Horizontal");
    	float vertical = Input.GetAxisRaw("Vertical");
    	Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    	if (direction.magnitude >= 0.1f)
    	{
    		float targetAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
            if (grabing)
                targetAngle += 180;
    		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
    		transform.rotation = Quaternion.Euler(0f, angle, 0f);
    		controller.Move(direction * speed * Time.deltaTime);
    	}

        
        velocity.y += gravity * Time.deltaTime;
        //Debug.Log(controller.velocity);
        controller.Move(velocity * Time.deltaTime);



        
    }
}
