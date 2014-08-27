using UnityEngine;
using System.Collections;

public class rightforearm : MonoBehaviour {
	public float moveSpeed = 10f;
	public float turnSpeed = 50f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.A))
			transform.Rotate(Vector3.forward, -turnSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.S))
			transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);
	
	}
}
