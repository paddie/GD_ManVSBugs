using UnityEngine; 
using System.Collections;

public class AnimationController : MonoBehaviour {
	/** Animation component.
	 * Should hold animations "awake" and "forward"
	 */
	public Animation anim;
	
	/** Minimum velocity for moving */
	public float sleepVelocity = 0.4F;
	
	/** Speed relative to velocity with which to play animations */
	public float animationSpeed = 0.2F;
	public float attackSpeedModifier = 1.0f;
	
	public bool attacking = false;
	public bool drawTracer = true;
	
	public Material trace;
	
	private string firingAnimation;
	private string idleAnimation;
	private string walkingAnimation;
	
	private LineRenderer lr;
	
	public void Start() {
		lr = gameObject.AddComponent<LineRenderer>();
		lr.material = trace;
        lr.SetWidth(0.1F, 0.1F);
        lr.SetVertexCount(2);
		lr.enabled = false;
		
		if (gameObject.tag == "Trooper") {
		float random = Random.value;
		idleAnimation = "soldierIdle";
		if (random < 0.33) {
			idleAnimation = "soldierIdle";
			firingAnimation = "soldierIdle";
		} else if (random < 0.66) {
			idleAnimation = "soldierIdleRelaxed";
			firingAnimation = "soldierIdle";
		} else {
			idleAnimation = "soldierCrouch";
			firingAnimation = "soldierCrouch";
		}
		walkingAnimation = "soldierSprint";
		} else if (gameObject.tag == "Bug") {
			firingAnimation = "attack";
			idleAnimation = "idle";
			walkingAnimation = "walk";
		}
		anim[firingAnimation].wrapMode = WrapMode.Clamp;
		
		//Prioritize the walking animation
		anim[walkingAnimation].layer = 10;
		
		//Play all animations
		anim.Play (idleAnimation);
		anim.Play (walkingAnimation);
		
		//Setup awake animations properties
		anim[idleAnimation].wrapMode = WrapMode.Clamp;
		anim[idleAnimation].speed = 0;
		anim[idleAnimation].normalizedTime = 1F;
	}
	
	public void Update() {
		if (!attacking) {
		Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
		Vector3 relVelocity = transform.InverseTransformDirection (velocity);
		if (velocity.sqrMagnitude <= sleepVelocity*sleepVelocity) {
			//Fade out walking animation
			anim.Blend (walkingAnimation,0,0.2F);
		} else {
			//Fade in walking animation
			anim.Blend (walkingAnimation,1,0.2F);
			
			//Modify animation speed to match velocity
			AnimationState state = anim[walkingAnimation];
			float speed = relVelocity.z;
			state.speed = speed*animationSpeed;
		}
		}
	}
	
	public void StartAttacking(Vector3 to, AudioClip audio) {
		if (attacking) 
			return;
		AudioSource.PlayClipAtPoint(audio,transform.position);
		Quaternion targetRotation = Quaternion.LookRotation(to - transform.position);
		transform.rotation = targetRotation;
		anim.Play (firingAnimation);
		attacking = true;
		lr.SetPosition(0, transform.position+Vector3.up);
		lr.SetPosition(1, to+Vector3.up+ new Vector3(Random.value,Random.value,Random.value));
		if (drawTracer)
		lr.enabled = true;
		if (gameObject.tag == "Trooper") {
			BroadcastMessage("Fire");
		}
	}
	
	public void StopAttacking() {
		attacking = false;
		lr.enabled = false;
		if (gameObject.tag == "Trooper") {
			BroadcastMessage("StopFiring");
		}
	}
 }
