using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour {

		[Header("Components")]
	[SerializeField] Rigidbody2D rb;
	[SerializeField] PhysicsMaterial2D pm;

		[Header("Debug")]
	[SerializeField] Text leftDebugField;
	[SerializeField] Text rightDebugField;

		[Header("Movement Parameters")]
	[SerializeField] float jumpHeight;	//TODO double jump
	[SerializeField] float maxSlopeAngle;	//TODO use this
	[SerializeField] float groundMaxSpeed;	//TODO build a nicer level to test shit
	[SerializeField] float groundAcceleration;
	[SerializeField] float groundDeceleration;
	[SerializeField] float airAcceleration;
	[SerializeField] float airDeceleration;

	ContactPoint2D groundCP;
	Vector2 groundPoint;
	Vector2 groundNormal;
	float groundAngle;
	bool grounded;
	bool onStaticGround;
	bool onKinematicGround;
	bool onDynamicGround;
	bool justJumped;

	List<ContactPoint2D> collisionContacts;
	Vector2 lastVelocity;

	KeyCode key_move_left = KeyCode.A;
	KeyCode key_move_right = KeyCode.D;
	KeyCode key_jump = KeyCode.Space;

	bool canJump;
	bool wantToJump;
	int jumps;

	void Start(){
		collisionContacts = new List<ContactPoint2D>();
		leftDebugField.text = "";
		rightDebugField.text = "";
	}

	void Update(){
		Debug.DrawRay(groundPoint, groundNormal, Color.green, 0f, false);

		if(Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale = 1.0f;
		if(Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale = 0.5f;
		if(Input.GetKeyDown(KeyCode.Alpha3)) Time.timeScale = 0.25f;
		if(Input.GetKeyDown(KeyCode.Alpha4)) Time.timeScale = 0.125f;
		if(Input.GetKeyDown(KeyCode.Alpha5)) Time.timeScale = 0.0625f;
		if(Input.GetKeyDown(KeyCode.Alpha6)) Time.timeScale = 0.03125f;

		if(Input.GetKeyDown(key_jump)) wantToJump = true;
	}

	void FixedUpdate(){
		DetermineGroundedness();
		DetermineAbilityToJump();

		leftDebugField.text = "";
		rightDebugField.text = "";
		rightDebugField.text += "v = " + string.Format("{0:F3}", rb.velocity.magnitude) + "u/s";
		rightDebugField.text += "\n" + "a = " + string.Format("{0:F3}", ((rb.velocity - lastVelocity).magnitude / Time.fixedDeltaTime)) + "u/s^2";
		rightDebugField.text += "\n" + "grounded = " + grounded;

		Vector2 moveInput = GetMoveInput();
		Vector2 forceVec = GetMoveForceVec(moveInput);

		rb.AddForce(forceVec, ForceMode2D.Force);

		if(wantToJump && canJump){
			rb.velocity = new Vector2(rb.velocity.x, CalculateJumpVelocity());
			justJumped = true;
			jumps++;
		}else{
			justJumped = false;
		}

		PrepareNextFixedUpdate();
	}

	void OnCollisionEnter2D(Collision2D collision){
		collisionContacts.AddRange(collision.contacts);
	}

	void OnCollisionStay2D(Collision2D collision){
		collisionContacts.AddRange(collision.contacts);
	}

	void PrepareNextFixedUpdate(){
		wantToJump = false;
		grounded = false;
		lastVelocity = rb.velocity;
		collisionContacts.Clear();
	}

	void GetGroundPoint(List<ContactPoint2D> contacts, out ContactPoint2D flattestContactPoint){
		flattestContactPoint = new ContactPoint2D();
		float bestDot = -1;
		foreach(ContactPoint2D cp in contacts){
			Debug.DrawRay(cp.point, cp.normal * 0.2f, Color.white, 0f, false);
			float newDot = Vector2.Dot(cp.normal, Vector2.up);
			if(newDot > bestDot && newDot > 0f){
				bestDot = newDot;
				flattestContactPoint = cp;
			}
		}
	}

	void DetermineGroundedness(){
		GetGroundPoint(collisionContacts, out groundCP);
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

	void DetermineAbilityToJump(){
		if(grounded) jumps = 0;
		canJump = (jumps < 2);
	}

	Vector2 GetMoveInput(){
		Vector2 moveInput = Vector2.zero;
		if(Input.GetKey(key_move_left))	moveInput += Vector2.left;
		if(Input.GetKey(key_move_right)) moveInput += Vector2.right;
		return moveInput;
	}

	Vector2 GetMoveForceVec(Vector2 moveInput){
		bool gotMoveInput = (moveInput != Vector2.zero);
		float acceleration;
		if(gotMoveInput){
			leftDebugField.text += "got input";
			if(grounded){
				leftDebugField.text += "\n" + "grounded";
				acceleration = GetCappedAcceleration(groundAcceleration, groundMaxSpeed, rb.velocity, moveInput);
			}else{
				leftDebugField.text += "\n" + "airborne";
				acceleration = GetCappedAcceleration(airAcceleration, groundMaxSpeed, Horizontalized(rb.velocity), moveInput);
			}
		}else{
			leftDebugField.text += "no input";
			if(grounded){
				leftDebugField.text += "\n" + "grounded";
				moveInput = -rb.velocity.normalized;
				acceleration = GetCappedDeceleration(groundDeceleration, rb.velocity, moveInput);
			}else{
				leftDebugField.text += "\n" + "airborne";
				moveInput = -Horizontalized(rb.velocity);
				acceleration = GetCappedDeceleration(airDeceleration, Horizontalized(rb.velocity), moveInput);
			}
		}
		return moveInput * acceleration * rb.mass;
	}

	float GetCappedAcceleration(float normalAcceleration, float maxSpeed, Vector2 velocity, Vector2 moveInput){
		float acceleration;
		if((velocity + (moveInput * normalAcceleration * Time.fixedDeltaTime)).magnitude <= maxSpeed){
			leftDebugField.text += "\n" + "normal acceleration";
			acceleration = normalAcceleration;
		}else{
			leftDebugField.text += "\n" + "capped acceleration";
			acceleration = (maxSpeed - velocity.magnitude) / Time.fixedDeltaTime;
		}
		return acceleration;
	}

	float GetCappedDeceleration(float normalDeceleration, Vector2 velocity, Vector2 moveInput){
		float acceleration;
		Vector2 velocityAfterNormalDecel = (velocity + (moveInput * normalDeceleration * Time.fixedDeltaTime));
		float velocityDot = Vector2.Dot(velocity, velocityAfterNormalDecel);
		if(velocityDot >= 0f){
			acceleration = normalDeceleration;
		}else{
			acceleration = (velocity.magnitude / Time.fixedDeltaTime);
		}
		return acceleration;
	}

	Vector2 GetJumpVec(){
		float jumpVel = CalculateJumpVelocity();
		float jumpAccel = jumpVel / Time.fixedDeltaTime;
		float jumpForce = rb.mass * jumpAccel;
		return Vector2.up * jumpForce;
	}

	float CalculateJumpVelocity(){
		return Mathf.Sqrt(2f * Physics2D.gravity.magnitude * jumpHeight);
	}

	Vector2 Horizontalized(Vector2 input){
		return new Vector2(input.x, 0f);
	}

}
