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
	private bool hasArrived = false;
	private bool isDead = false;
	
	public void Start() {
		
		if (gameObject.tag == "Trooper") {
			enemyTag = "Bug";
		} else {
			enemyTag = "Trooper";
		}
		InvokeRepeating("ScanForEnemies", 0.0f, ScanFrequency);
	}
	
	public void ScanForEnemies() {	
		Collider[] objectsInRange = Physics.OverlapSphere(transform.position, ScanRadius);
	    
	    // identify closest enemy and shoot!
	    float maxDistance = 1000;
	    Transform closestEnemy = null;
	    foreach (Collider col in objectsInRange) {
	    	
			// only check items that respoond to enemyTag
	        if ( col.gameObject.tag == enemyTag ) {
	        	
	        	var distance = Vector3.Distance( col.transform.position, transform.position);
	        	if ( distance < maxDistance ) {
	        		closestEnemy = col.transform;
	        		maxDistance = distance;
	        	}
	        }
	    }
	    
	    if ( closestEnemy != null ) { 
	    	Debug.LogError(gameObject.tag + "Engaging enemy!");
	    	this.GetComponent<AIPath>().canMove = false;
	    	EngageEnemy(closestEnemy);
	    } else {
	    	this.GetComponent<AIPath>().canMove = true;
	    }
	    
	    //yield return new WaitForSeconds(ScanFrequency);
	}
	
	public void EngageEnemy(Transform enemy) {
		// make sure the scanner doesn't run while we're engaging
		//EnemyInRange = enemy;
		
		// only shoot at enemy while he is within scan radius
//		if ( enemy != null  && gameObject != null &&
//			Vector3.Distance(enemy.transform.position, transform.position) < ScanRadius ) 
//		{
			float prob = UnityEngine.Random.Range(0.0f,100.0f);
			if ( prob <= unitAccuracy ) {
				//baseDamage can be altered as unit is promoted
				//Debug.LogError("Direct Hit! (" + prob + ")");
				UnitLogic tr = null;
				try {
					enemy.GetComponent<UnitLogic>().DoDamage(baseDamage);
				} catch (MissingReferenceException e) {
					return;
				} catch (NullReferenceException e) {
					return;
				}
			}
			
			//yield return new WaitForSeconds(firingFrequency);
//		}
	}
	
//	public void Deploy() {
//		GetComponent<AIPath>().canMove = true;
//		GetComponent<AIPath>().canSearch = true;
//	}
	
	public void Dock() {
		this.hasArrived = true;
		GetComponent<AIPath>().canMove = false;
		GetComponent<AIPath>().canSearch = false;
	}
	
	private readonly object targetLock = new object();
	
	public void SetNewTarget(Transform node) {
		lock ( this.targetLock ) {
			GetComponent<AIPath>().target = node;
		}
		this.hasArrived = false;
		GetComponent<AIPath>().canMove = true;
		GetComponent<AIPath>().canSearch = true;
	}
	
	public void DoDamage(float damage) {
		this.baseHealth -= damage;
		if ( this.baseHealth < 0.0 ) {
			// update global playerScore etc.
			//GameObject.Find("Game").GetComponent<GameLogic>().UnitDied(gameObject.name);
			Transform target = gameObject.GetComponent<AIPath>().target;
			lock ( this.targetLock ) {
				if (target != null && this.enemyTag == "Bug" && !this.isDead) {
					if ( this.hasArrived && !this.isDead) {
						target.GetComponent<Selectable>().DockedTrooperDied(transform);
					} else {
						target.GetComponent<Selectable>().ApproachingTrooperDied(this.id);
					}
				
				} else {
					// notify of bug death?
				}
				this.isDead = true;
				Destroy(gameObject);
			}
		}
	}
}