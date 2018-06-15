using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchableSpike : MonoBehaviour {

	[SerializeField] Rigidbody2D rb;
	[SerializeField] float speed;

	bool launched;

	void Start () {
		launched = false;
	}
	
	void Update () {
		
	}

	void FixedUpdate(){
		if(launched){
			rb.MovePosition(transform.position + (transform.up * speed * Time.fixedDeltaTime));
		}
	}

	public void Launch(){
		launched = true;
	}

	void OnCollisionEnter2D(Collision2D collision){
		CheckAndDamage(collision.gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider){
		CheckAndDamage(collider.gameObject);
	}

	void CheckAndDamage(GameObject obj){
		IDamageable damageable = obj.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.Damage(999, Vector2.zero);
		}
	}
}
