using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilCherry : MonoBehaviour {

	[SerializeField] Rigidbody2D rb;
	[SerializeField] float moveSpeed;

	Vector3 direction;
	bool launched;

	void Start () {
		launched = false;
	}
	
	void Update () {
		
	}

	void FixedUpdate(){
		
	}

	void OnCollisionEnter2D(Collision2D collision){
		IDamageable damageable = collision.collider.GetComponent<IDamageable>();
		if(damageable != null){
			damageable.Damage(999, Vector3.zero);
		}
		Destroy(this.gameObject);
	}

	public void Launch(){
		if(!launched){
			launched = true;
			GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
			direction = (player.transform.position - this.transform.position).normalized;
			rb.velocity = direction * moveSpeed;
		}
	}

}
