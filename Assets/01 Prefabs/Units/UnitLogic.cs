using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitLogic : MonoBehaviour {

	public float observeTimer = 0.1f;
	public Material enemySightedMat;
	
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
		    	Debug.LogError("Engaging enemy!");
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
			float prob = Random.Range(0.0f,100.0f);
			if ( prob <= unitAccuracy ) {
			//baseDamage can be altered as unit is promoted
			//Debug.LogError("Direct Hit! (" + prob + ")");
				enemy.GetComponent<UnitLogic>().DoDamage(baseDamage);
			} //else {
			//logic.DidDodge();
			//}
			
			//yield return new WaitForSeconds(firingFrequency);
//		}
	}
	
	public void DoDamage(float damage) {
		this.baseHealth -= damage;
		if ( this.baseHealth < 0.0 ) {
			// update global playerScore etc.
			//GameObject.Find("Game").GetComponent<GameLogic>().UnitDied(gameObject.name);
			Transform target = gameObject.GetComponent<AIPath>().target;
			Destroy(gameObject);
			if (target == null) return;
			target.GetComponent<Selectable>().ApproachingOrArrivedTrooperDied();
		}
	}
	
}