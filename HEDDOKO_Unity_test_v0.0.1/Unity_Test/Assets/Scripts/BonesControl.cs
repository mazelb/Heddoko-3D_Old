using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))] 

public class BonesControl : MonoBehaviour 
{
	public Transform rightUpperArmTransform = null;
	public Transform rightForeArmTransform = null;
	public Transform leftUpperArmTransform = null;
	public Transform leftForeArmTransform = null;

	public float rightUpperArmFac = 0.5f;
	public float rightForeArmFac = 0.5f;
	public float leftUpperArmFac = 0.5f;
	public float leftForeArmFac = 0.5f;

	private Quaternion rightUpperArmTarget;
	private Quaternion rightForeArmTarget;
	private Quaternion leftUpperArmTarget;
	private Quaternion leftForeArmTarget;


	//test 
	private Vector3 currentDirection = Vector3.forward;

	//protected Animator animator;
	
	//public bool ikActive = false;

	// Use this for initialization
	void Start () 
	{
		//animator = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	void LateUpdate()
	{
		currentDirection.x = Mathf.Clamp(currentDirection.x + Input.GetAxis("Mouse X"), -1, 1);
		currentDirection.y = Mathf.Clamp(currentDirection.y + Input.GetAxis("Mouse Y"), -1, 1);

		rightUpperArmTarget = Quaternion.LookRotation(currentDirection);

		rightUpperArmTransform.localRotation = Quaternion.Slerp(rightUpperArmTransform.localRotation, rightUpperArmTarget, rightUpperArmFac);
		//elbow.localRotation = Quaternion.Slerp(elbow.localRotation, rotation, elbowControl);
		//hand.localRotation = Quaternion.Slerp(hand.localRotation, rotation, handControl);
	}

	//a callback for calculating IK
	/*void OnAnimatorIK()
	{
		if(animator) 
		{
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) 
			{
				
				//weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);
				
				//set the position and the rotation of the right hand where the external object is
				if(rightHandObj != null) 
				{
					animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
				}                   
				
			}
			
			//if the IK is not active, set the position and rotation of the hand back to the original position
			else 
			{          
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);             
			}
		}
	}*/   
}
