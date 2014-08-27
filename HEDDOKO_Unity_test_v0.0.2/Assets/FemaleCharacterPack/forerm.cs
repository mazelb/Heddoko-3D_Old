using UnityEngine;
using System.Collections;

public class forerm : MonoBehaviour {
	public float moveSpeed = 10f;
	public float turnSpeed = 50f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Q))
			transform.Rotate(Vector3.forward, -turnSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.W))
			transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);

	}
}
