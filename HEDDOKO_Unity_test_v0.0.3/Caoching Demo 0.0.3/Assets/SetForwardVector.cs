using UnityEngine;
using System.Collections;

public class SetForwardVector : MonoBehaviour
{
    public Transform Target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.forward = Target.forward;
	}
}
