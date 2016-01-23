using UnityEngine;
using System.Collections;
using Assets.Scripts.UI;

public class SetForwardVector : MonoBehaviour
{
    public Transform Target;
    public Transform VectorA;
    public Transform VectorB;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.forward = Target.forward; 
	}
}
