using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlatform : MonoBehaviour {

	[SerializeField] Rigidbody2D rb;
	[SerializeField] Vector2 moveVector;
	[SerializeField] float moveTime;

	Vector2 startPos;

	void Start () {
		startPos = transform.position;
	}
	
	void Update () {
		
	}

	void FixedUpdate(){
		float scaledTime = Time.time / moveTime;
		float progress = scaledTime - Mathf.Floor(scaledTime);
		rb.MovePosition(Vector2.Lerp(startPos, startPos + moveVector, progress));
	}
}
