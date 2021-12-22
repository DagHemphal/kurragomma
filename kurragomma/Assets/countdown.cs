using UnityEngine;
using TMPro;

public class countdown : MonoBehaviour
{
    public TextMeshProUGUI Countdown;
    public GameObject seeker;
    public GameObject hider;
    
    float counter = 10f;

    // Start is called before the first frame update
    void Start()
    {
		seeker.GetComponent<Playermovement>().enabled = false;
		hider.GetComponent<Playermovement>().enabled = true;
        Countdown.text = counter.ToString("0");
 	}

    // Update is called once per frame
    void Update()
    {
       if(counter > 0)
        {
            counter -= Time.deltaTime;
            Countdown.text = counter.ToString("0");

        }
        else { //TO-DO should run once not every frame.

        	Countdown.text = "";
        	hider.GetComponent<Playermovement>().enabled = false;
    		seeker.GetComponent<Playermovement>().enabled = true;
        }
    }
}
