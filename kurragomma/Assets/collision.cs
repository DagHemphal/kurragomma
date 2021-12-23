using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
    	Debug.Log("Found You!");
    }
}
