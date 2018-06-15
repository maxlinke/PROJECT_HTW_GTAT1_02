using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMover : MonoBehaviour {

		[Header("Components")]
	[SerializeField] Rigidbody2D rb;

		[Header("Settings")]
	[SerializeField] float maxHorizontalLead;
	[SerializeField] float horizontalLeadScalingFactor;
	[SerializeField] float maxVerticalLead;
	[SerializeField] float verticalLeadScalingFactor;
	[Range(1, 10)] [SerializeField] int smoothing = 1;

	float z;
	Vector2[] velocities;
	int velocityIndex;

	void Start(){
		z = transform.localPosition.z;
		velocities = new Vector2[smoothing];
		velocityIndex = 0;
	}
	
	void Update(){
		AddToVelocityArray(rb.velocity);
		Vector2 meanVelocity = GetMeanVelocity();
		Vector2 offset = CalculateHorizontalOffset(meanVelocity) + CalculateVerticalOffset(meanVelocity);
		transform.localPosition = new Vector3(offset.x, offset.y, z);
	}

	float CalculateOffset(float speed, float maxOffset, float scalingFactor){
		return ((-1f) * ((1f / ((speed * scalingFactor) + 1f)) - 1f) * maxOffset);
	}

	Vector2 CalculateHorizontalOffset(Vector2 velocity){
		float offset = CalculateOffset(Mathf.Abs(velocity.x), maxHorizontalLead, horizontalLeadScalingFactor);
		return new Vector2(Mathf.Sign(velocity.x) * offset, 0f);
	}

	Vector2 CalculateVerticalOffset(Vector2 velocity){
		float offset = CalculateOffset(Mathf.Abs(velocity.y), maxVerticalLead, verticalLeadScalingFactor);
		return new Vector2(0f, Mathf.Sign(velocity.y) * offset);
	}

	void AddToVelocityArray(Vector2 velocity){
		velocities[velocityIndex] = velocity;
		velocityIndex = (velocityIndex + 1) % velocities.Length;
	}

	Vector2 GetMeanVelocity(){
		Vector2 sum = Vector2.zero;
		for(int i=0; i<velocities.Length; i++){
			sum += velocities[i];
		}
		return (sum / velocities.Length);
	}

}
