using UnityEngine;
using System.Collections;

/** AI controller specifically made for the spider robot.
 * The spider robot (or mine-bot) which is got from the Unity Example Project
 * can have this script attached to be able to pathfind around with animations working properly.\n
 * This script should be attached to a parent GameObject however since the original bot has Z+ as up.
 * This component requires Z+ to be forward and Y+ to be up.\n
 * 
 * It overrides the AIPath class, see that class's documentation for more information on most variables.\n
 * Animation is handled by this component. The Animation component refered to in #anim should have animations named "awake" and "forward".
 * The forward animation will have it's speed modified by the velocity and scaled by #animationSpeed to adjust it to look good.
 * The awake animation will only be sampled at the end frame and will not play.\n
 * When the end of path is reached, if the #endOfPathEffect is not null, it will be instantiated at the current position. However a check will be
 * done so that it won't spawn effects too close to the previous spawn-point.
 * \shadowimage{mine-bot.png}
 */
[RequireComponent(typeof(Seeker))]
public class SoldierAnimation : AIPath {
	
	/** Animation component.
	 * Should hold animations "awake" and "forward"
	 */
	public Animation anim;
	
	/** Minimum velocity for moving */
	public float sleepVelocity = 0.4F;
	
	/** Speed relative to velocity with which to play animations */
	public float animationSpeed = 0.2F;
	
	/** Effect which will be instantiated when end of path is reached.
	 * \see OnTargetReached */
	public GameObject endOfPathEffect;
	
	public float acceptableEndDistance = 3.0f;
	
	/** Point for the last spawn of #endOfPathEffect */
	protected Vector3 lastTarget;
	protected bool moving = false;
	
	public new void Start () {
		lastTarget = GetFeetPosition();
		float random = Random.value;
		string idleAnimation = "soldierIdle";
		if (random < 0.33) {
			idleAnimation = "soldierIdle";
		} else if (random < 0.66) {
			idleAnimation = "soldierIdleRelaxed";
		} else {
			idleAnimation = "soldierCrouch";
		}
		
		//Prioritize the walking animation
		anim["soldierSprint"].layer = 10;
		
		//Play all animations
		anim.Play (idleAnimation);
		anim.Play ("soldierSprint");
		
		//Setup awake animations properties
		anim[idleAnimation].wrapMode = WrapMode.Clamp;
		anim[idleAnimation].speed = 0;
		anim[idleAnimation].normalizedTime = 1F;
		
		//Call Start in base script (AIPath)
		base.Start ();
	}
	
	/**
	 * Called when the end of path has been reached.
	 * An effect (#endOfPathEffect) is spawned when this function is called
	 * However, since paths are recalculated quite often, we only spawn the effect
	 * when the current position is some distance away from the previous spawn-point
	*/
	public override void OnTargetReached () {
		if (endOfPathEffect != null && Vector3.Distance (GetFeetPosition(), lastTarget) > 1) {
			GameObject.Instantiate (endOfPathEffect,tr.position,tr.rotation);
			lastTarget = GetFeetPosition();
		}		
	}
	
	public override Vector3 GetFeetPosition ()
	{
		return tr.position;
	}
	
	protected new void FixedUpdate () {
		//Get velocity in world-space
		Vector3 velocity;
		if (canMove) {
		
			//Calculate desired velocity
			Vector3 dir = CalculateVelocity (GetFeetPosition());
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			if (targetDirection != Vector3.zero) {
				RotateTowards (targetDirection);
			}
			
			if (dir.sqrMagnitude > sleepVelocity*sleepVelocity) {
				//If the velocity is large enough, move
			} else {
				//Otherwise, just stand still (this ensures gravity is applied)
				dir = Vector3.zero;
			}
			
			if (navController != null)
				navController.SimpleMove (GetFeetPosition(), dir);
			else if (controller != null)
				controller.SimpleMove (dir);
			else
				Debug.LogWarning ("No NavmeshController or CharacterController attached to GameObject");
			
			velocity = controller.velocity;
		} else {
			velocity = Vector3.zero;
		}
		
		
		//Animation
		
		//Calculate the velocity relative to this transform's orientation
		Vector3 relVelocity = tr.InverseTransformDirection (velocity);
		
		if (velocity.sqrMagnitude <= sleepVelocity*sleepVelocity) {
			//Fade out walking animation
			anim.Blend ("soldierSprint",0,0.2F);
			if (moving) {

				moving = false;
			}
		} else {
			moving = true;
			//Fade in walking animation
			anim.Blend ("soldierSprint",1,0.2F);
			
			//Modify animation speed to match velocity
			AnimationState state = anim["soldierSprint"];
			
			float speed = relVelocity.z;
			state.speed = speed*animationSpeed;
		}
	}
}
