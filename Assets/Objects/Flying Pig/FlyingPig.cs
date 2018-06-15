using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPig : MonoBehaviour, ICollectible {

	[SerializeField] SpriteRenderer sr;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] GameObject bloodEffectPrefab;

	[SerializeField] float distanceWeight;
	[SerializeField] float moveWeight;

	Vector3 startPos;
	Vector3 currentDirection;
	Vector2 timeOffset;

	float perlinScale = 0.5f;

	void Start () {
		startPos = transform.position;
		float xOff = Random.Range(0f, 10000f);
		float yOff = Random.Range(0f, 10000f);
		timeOffset = new Vector2(xOff, yOff);
	}
	
	void Update () {
		
	}

	void FixedUpdate(){
		currentDirection = new Vector3(Mathf.PerlinNoise((Time.time + timeOffset.x) * perlinScale, 0f), Mathf.PerlinNoise(0f, (Time.time + timeOffset.y) * perlinScale), 0f).normalized;
		Vector3 returnVec = (startPos - transform.position) * distanceWeight;
		Vector3 exploreVec = currentDirection *  moveWeight;
		Vector3 vectorSum = returnVec + exploreVec;
		rb.AddForce(vectorSum);
		if(vectorSum.x > 0) sr.flipX = false;
		else sr.flipX = true;
	}

	public void Collect(){
		GameObject obj = Instantiate(bloodEffectPrefab) as GameObject;
		obj.transform.position = this.transform.position;
		obj.transform.rotation = Quaternion.identity;
		Destroy(this.gameObject);
	}

}
