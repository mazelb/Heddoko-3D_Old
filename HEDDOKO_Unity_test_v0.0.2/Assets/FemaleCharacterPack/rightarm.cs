using UnityEngine;
using System.Collections;

public class rightarm : MonoBehaviour {
	public float moveSpeed = 10f;
	public float turnSpeed = 50f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow))
			transform.Rotate(Vector3.forward, -turnSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.DownArrow))
			transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);
	}
}
