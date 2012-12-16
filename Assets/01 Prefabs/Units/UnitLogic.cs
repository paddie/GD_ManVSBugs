using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitLogic : MonoBehaviour {

	public float observeTimer = 0.1f;
	public Material enemySightedMat;
	public int id = 0;
	
	
	// raceSpecifics
	
	
	// unitStats
	// needs more comments
	public float ScanRadius = 50;
	public float ScanFrequency = 0.7f;
	public float unitAccuracy = 70.0f;
	public float baseHealth = 10f;
	public float baseDamage = 5f;
	public string enemyTag;
	public float firingFrequency = 0.7f;
	public bool hasArrived = false;
	private bool isDead = false;
	private Transform currentEnemy = null;
	
	
	public LayerMask selectionLayer;

	
	public void Start() {
		
		if (gameObject.tag == "Trooper") {
			enemyTag = "Bug";
		} else {
			enemyTag = "Trooper";
		}
		InvokeRepeating("ScanForEnemies", 0.0f, ScanFrequency);
	}
	
	private Transform LocateClosestEnemy() {
		
		Transform closestEnemy = null;
		Collider[] objectsInRange = Physics.OverlapSphere(transform.position, ScanRadius);
		float minDistance = 100.0f;
		foreach (Collider col in objectsInRange) {
			Transform enemy = col.transform;
			// only check items that respoond to enemyTag
	        if ( enemy.tag == enemyTag ) {
				RaycastHit hit;
				Vector3 direction = enemy.position - transform.position+Vector3.up;
				if ( Physics.Raycast (transform.position, direction, out hit,this.ScanRadius, this.selectionLayer) ) { 
					//Debug.DrawRay(transform.position, enemy.position - transform.position, Color.red);
					//Debug.LogWarning("enemy hidden by terrain");
					continue;
				} else {
					float distance = Vector3.Distance( col.transform.position, transform.position);
		        	if ( distance < minDistance ) {
		        		closestEnemy = enemy;
		        		minDistance = distance;
		        	}
		        }		
			}
    	}
		
		return closestEnemy;
	}
	
	public void ScanForEnemies() {	
		// if we engaged an enemy in the previous scan
		// check if the enemy is still within range to save a scan
		if ( this.currentEnemy != null ) {
			RaycastHit hit;
			Vector3 direction = this.currentEnemy.position - transform.position;
			if ( Vector3.Distance(this.currentEnemy.position, transform.position) > this.ScanRadius ||
				Physics.Raycast (transform.position+Vector3.up, direction, out hit,this.ScanRadius, this.selectionLayer) ) {
				
				this.currentEnemy = null;
			} else {
				EngageEnemy(this.currentEnemy);
				return;
			}
		}
		
		this.currentEnemy = LocateClosestEnemy();
		if (this.currentEnemy == null) {
			GetComponent<AIPath>().canMove = true;
			return;
		}
		EngageEnemy(this.currentEnemy);
	}
	
	public void EngageEnemy(Transform enemy) {
		
		
		GetComponent<AIPath>().canMove = false;
		
		float prob = UnityEngine.Random.Range(0.0f,100.0f);
		if ( prob <= unitAccuracy ) {
			try {
				StartAttacking (enemy.position);
				Invoke ("StopAttacking",0.05f);
				enemy.GetComponent<UnitLogic>().DoDamage(baseDamage);
			} catch (MissingReferenceException e) {
				return;
			} catch (NullReferenceException e) {
				return;
			}
		}
	}
	
	private void StartAttacking(Vector3 to) {
		transform.GetComponent<AnimationController>().StartAttacking(to);
	}
	
	private void StopAttacking() {
		transform.GetComponent<AnimationController>().StopAttacking();
	}
	
	private void StopMoving() {
		GetComponent<AIPath>().enabled = false;
	}
	
	private void GetMoving() {
		GetComponent<AIPath>().enabled = true;
	}
	
	public void Dock(Transform node) {
		Invoke ("StopMoving",0.5f);
		GetComponent<AIPath>().target = node;
		this.hasArrived = true;
		StopMoving();
		//GetComponent<AIPath>().canMove = false;
		//GetComponent<AIPath>().canSearch = false;
	}
	
	private readonly object targetLock = new object();
	private readonly object isDeadLock = new object();
	
	public void SetNewTarget(Transform node) {
		//lock ( this.targetLock ) {
		GetComponent<AIPath>().target = node;
		this.hasArrived = false;
		GetMoving();
		//}
		
		//GetComponent<AIPath>().canMove = true;
		GetComponent<AIPath>().canSearch = true;
	}
	
	public void DoDamage(float damage) {
		
		if ( this.isDead ) return;
		
		bool tearDown = false;
		lock ( isDeadLock ) {
			this.baseHealth -= damage;	
			if ( this.baseHealth <= 0 && !this.isDead) {
				this.isDead = true;
				tearDown = true;
			}
		}
		
		if ( !tearDown ) {
			return;
		}
		
		// teardown!
		
		// signal death to stat object
		GameObject.Find("GameLogic").GetComponent<GameState>().UnitDied(this.tag);
		
		// if we've got a space reserved on an object, signal death to node
		Transform target = gameObject.GetComponent<AIPath>().target;
		if ( target != null ) {
			if ( this.hasArrived ) {
				target.GetComponent<Selectable>().DockedUnitDied(transform);
			} else {
				target.GetComponent<Selectable>().ApproachingUnitDied(this.tag);
			}			
		}
		
		Destroy(gameObject);

	}
}