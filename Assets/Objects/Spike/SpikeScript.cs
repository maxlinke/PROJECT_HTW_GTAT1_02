using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D collision){
		IDamageable damageable = collision.collider.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.Damage(999, Vector2.zero);
		}
	}
}
