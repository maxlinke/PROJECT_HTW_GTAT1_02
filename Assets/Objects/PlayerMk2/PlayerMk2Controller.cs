using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMk2Controller : MonoBehaviour, IDamageable {

	[SerializeField] Rigidbody2D rb;
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer sr;
	[SerializeField] Text killCountText;
	[SerializeField] GameObject explosionPrefab;
	[SerializeField] float levelResetOnDeathDelay;

	[Header("Key Binds")]
	[SerializeField] KeyCode key_left;
	[SerializeField] KeyCode key_right;
	[SerializeField] KeyCode key_jump;
	[SerializeField] KeyCode key_slow;

	[Header("Jump Params")]
	[SerializeField] int numberOfPossibleConsecutiveJumps;
	[SerializeField] float jumpHeight;

	[Header("Grounded Movement")]
	[SerializeField] float maxGroundSpeed;
	[SerializeField] float maxGroundAccel;
	[SerializeField] float maxGroundDecel;
	[SerializeField] float autoGroundDecel;

	[Header("Airborne Movement")]
	[SerializeField] float maxAirSpeed;
	[SerializeField] float maxAirAccel;
	[SerializeField] float maxAirDecel;
	[SerializeField] float autoAirDecel;

	[Header("General Movement Params")]
	[SerializeField] float slowSpeedModifier;

	List<ContactPoint2D> collisionContacts;

	ContactPoint2D groundCP;
	Vector2 groundPoint;
	Vector2 groundNormal;
	float groundAngle;
	bool grounded;
	bool onStaticGround;
	bool onKinematicGround;
	bool onDynamicGround;

	int jumps;
	bool canJump;
	bool wantToJump;
	bool justJumped;
	bool wasGrounded;

	bool dead;

	void Start () {
		collisionContacts = new List<ContactPoint2D>();
		dead = false;
	}
	
	void Update () {
		if(Input.GetKeyDown(key_jump)){
			wantToJump = true;
		}
		if(Input.GetKeyDown(KeyCode.R)){
			ReloadLevel();
		}
	}

	void FixedUpdate () {
		DetermineGroundedness();
		DetermineAbilityToJump();

		Vector2 inputDir = GetInputDirection();
		float speed = rb.velocity.magnitude;
		Vector2 horiVel = new Vector2(rb.velocity.x, 0f);
		float horiSpeed = horiVel.magnitude;

		Vector2 accelVec;

		if(!dead){
			if(inputDir != Vector2.zero){

				if(inputDir.x > 0) sr.flipX = false;
				else sr.flipX = true;

				float inputDotVel = Vector2.Dot(inputDir, horiVel);
				if(inputDotVel > 0f){	//"forwards"
					if(grounded){
						float maxSpeed = maxGroundSpeed;
						float maxAccel = maxGroundAccel;
						if(Input.GetKey(key_slow)){
							maxSpeed *= slowSpeedModifier;
						}
						accelVec = GetClampedAcceleration(horiVel, inputDir * maxSpeed, maxAccel);
					}else{
						float maxSpeed = maxAirSpeed;
						float maxAccel = maxAirAccel;
						if(Input.GetKey(key_slow)){
							maxSpeed *= slowSpeedModifier;
						}
						accelVec = GetClampedAcceleration(horiVel, inputDir * maxSpeed, maxAccel);
					}
				}else{	//"backwards"
					if(grounded){
						accelVec = inputDir * maxGroundDecel;
					}else{
						accelVec = inputDir * maxAirDecel;
					}
				}
			}else{
				if(grounded){
					accelVec = -horiVel.normalized * autoGroundDecel;
				}else{
					accelVec = -horiVel.normalized * autoAirDecel;
				}
				Vector2 hypoNewVec = horiVel + (accelVec * Time.fixedDeltaTime);
				if(Vector2.Dot(hypoNewVec, horiVel) < 0){
					accelVec = -horiVel / Time.fixedDeltaTime;
				}
			}

		}else{	//dead
			
			if(grounded){
				accelVec = -horiVel.normalized * autoGroundDecel;
				if(Mathf.Abs((accelVec * Time.fixedDeltaTime).x) > horiSpeed){
					accelVec = -horiVel / Time.fixedDeltaTime;
				}
			}else{
				accelVec = Vector2.zero;
			}
		}


		rb.AddForce(accelVec * rb.mass, ForceMode2D.Force);

		if(!dead){
			if(canJump && wantToJump){
				rb.velocity = new Vector2(rb.velocity.x, CalculateJumpVelocity());
				jumps++;
				justJumped = true;
			}else{
				justJumped = false;
			}

			Vector3 nextVelocity = rb.velocity + (accelVec * Time.fixedDeltaTime);
			float nextSpeed = Mathf.Abs(nextVelocity.x);
			float nextVerticalVelocity = nextVelocity.y;
			animator.SetFloat("speed", nextSpeed);
			animator.SetFloat("verticalVelocity", nextVerticalVelocity);
			if(!grounded && wasGrounded) animator.SetTrigger("justFell");
			if(grounded && !wasGrounded) animator.SetTrigger("justLanded");
			if(justJumped) animator.SetTrigger("justJumped");
		}

		PrepareNextFixedUpdate();
	}

	Vector2 GetClampedAcceleration(Vector2 startVelocity, Vector2 targetVelocity, float maxAcceleration){
		Vector2 deltaV = targetVelocity - startVelocity;
		Vector2 accel = deltaV.normalized * maxAcceleration;
		if(accel.magnitude / Time.fixedDeltaTime > deltaV.magnitude){
			return deltaV / Time.fixedDeltaTime;
		}else{
			return accel;
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		collisionContacts.AddRange(collision.contacts);
	}

	void OnCollisionStay2D (Collision2D collision) {
		collisionContacts.AddRange(collision.contacts);
	}

	void OnTriggerEnter2D (Collider2D otherCollider) {
		ICollectible collectible = otherCollider.GetComponent<ICollectible>();
		if(collectible != null){
			collectible.Collect();
			killCountText.text += "I";
		}
	}

	public void Damage(float amount, Vector3 direction){
		if(!dead){
			GameObject obj = Instantiate(explosionPrefab) as GameObject;
			obj.transform.position = transform.position;
			obj.transform.rotation = Quaternion.identity;
			dead = true;
			animator.SetTrigger("justDied");
			StartCoroutine(DeathCoroutine());
		}
	}

	IEnumerator DeathCoroutine(){
		yield return new WaitForSeconds(levelResetOnDeathDelay);
		ReloadLevel();
	}

	void ReloadLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	Vector2 GetInputDirection(){
		Vector2 output = Vector2.zero;
		if(Input.GetKey(key_left)) output += Vector2.left;
		if(Input.GetKey(key_right)) output += Vector2.right;
		return output;
	}

	void PrepareNextFixedUpdate(){
		wantToJump = false;
		wasGrounded = grounded;
		collisionContacts.Clear();
	}

	void DetermineAbilityToJump(){
		if(grounded) jumps = 0;
		if(!grounded && wasGrounded && !justJumped) jumps = 1;
		canJump = (jumps < numberOfPossibleConsecutiveJumps);
	}

	void DetermineGroundedness () {
		groundCP = GetGroundPoint(collisionContacts);
		if(groundCP.collider != null && !justJumped){
			groundPoint = groundCP.point;
			groundNormal = groundCP.normal;
			groundAngle = Vector2.Angle(Vector2.up, groundNormal);
			grounded = true;
			onStaticGround = (groundCP.collider.attachedRigidbody == null);
			onKinematicGround = !onStaticGround && (groundCP.collider.attachedRigidbody.isKinematic);
			onDynamicGround = !onStaticGround && !onKinematicGround;
		}else{
			groundPoint = Vector2.zero;
			groundNormal = Vector2.zero;
			groundAngle = 0f;
			grounded = false;
			onStaticGround = false;
			onKinematicGround = false;
			onDynamicGround = false;
		}
	}

	ContactPoint2D GetGroundPoint (List<ContactPoint2D> contacts) {
		ContactPoint2D flattestContactPoint = new ContactPoint2D();
		float bestDot = -1;
		foreach(ContactPoint2D cp in contacts){
			float newDot = Vector2.Dot(cp.normal, Vector2.up);
			if(newDot > bestDot && newDot > 0f){
				bestDot = newDot;
				flattestContactPoint = cp;
			}
		}
		return flattestContactPoint;
	}

	float CalculateJumpVelocity(){
		return Mathf.Sqrt(2f * Physics2D.gravity.magnitude * jumpHeight);
	}

	Vector2 Horizontalized(Vector2 input){
		return new Vector2(input.x, 0f);
	}

}
