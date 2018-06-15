using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBasedCameraMover : MonoBehaviour {

	[SerializeField] KeyCode key_left;
	[SerializeField] KeyCode key_right;

	[SerializeField] float maxLead;
	[SerializeField] float camSpeed;

	void Start () {
		
	}
	
	void Update () {
		Vector3 moveVec = Vector3.zero;
		if(Input.GetKey(key_left)){
			moveVec += Vector3.left;
		}
		if(Input.GetKey(key_right)){
			moveVec += Vector3.right;
		}
		Vector3 newPos;
		Vector3 hypoNewPos = transform.localPosition + (moveVec * camSpeed * Time.deltaTime);
		if(hypoNewPos.x < -maxLead) newPos = new Vector3(-maxLead, hypoNewPos.y, hypoNewPos.z);
		else if(hypoNewPos.x > maxLead) newPos = new Vector3(maxLead, hypoNewPos.y, hypoNewPos.z);
		else newPos = hypoNewPos;
		transform.localPosition = newPos;
	}
}
