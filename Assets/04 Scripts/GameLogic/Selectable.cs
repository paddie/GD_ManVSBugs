using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	//public Renderer renderer;
	
	void Start() {
		if ( transform.tag == "DropZone") {
			this.unitType = GetComponent<DropZone>().Unit.tag;
			this.CurrentlyHeldBy = GetComponent<DropZone>().Unit.tag;
		}
		
		InvokeRepeating("releaseTheTroops",1.0f,2.0f);
		InvokeRepeating("releaseTheBugs",1.0f,2.0f);	
	}
//	public selectedMaterial : Material;
//	var targetMaterial : Material;

	//public int Capacity = 10;
	public int TrooperCapacity = 10;
	public int BugCapacity = 10;
	public int bugSquadSize = 2;
	
	public int ApproachingTroopers = 0;
	public int ApproachingBugs = 0;
	public int CurrentTrooperCount = 0;
	public int CurrentBugCount = 0;
	
	public string unitType;
	public string CurrentlyHeldBy;

	
	
	public Transform TrooperTarget = null;
	private List<Transform> TrooperUnits = new List<Transform>();

	public Transform BugTarget = null;
	private List<Transform> BugUnits = new List<Transform>();

	// set to true if the node is selected by a user
	public bool isSelected = false;

	// node is blocked
//	public bool isBlocked = false;
	
	private readonly object TrooperUnitsLock = new object();
	private readonly object BugUnitsLock = new object();


	// for displaying the target-node-number relative to selected node
	//	var isTarget : boolean = false;
	//	var targetNumber : int = -1;
	
	//defaultMaterial = renderer.material;
	
	/*
	*****************************************************
	** MOUSE CONTROLS
	*****************************************************
	*/
	//function OnMouseOver() {
	//    if (Input.GetMouseButtonDown(1)) {
	//    	if (this.isSelected) {
	//    		if ( this.isBlocked ) {
	//    			Debug.LogError("now unblocked!!");
	//    			this.isBlocked = false;	
	//    		} else {
	//    			Debug.LogError("now blocked!");
	//    			this.isBlocked = true;
	//    		}
	//    	}
	//    	//Debug.LogError("Right Click!");
	//    	if ( Input.GetKey( KeyCode.LeftShift ) ) {
	//    		Debug.LogError("Setting Bug Target!");	
	//			GameObject.Find("Game").GetComponent(GameLogic).SetTargetBug(gameObject);
	//			//Select();
	//		} else {
	//			Debug.LogError("Setting Trooper Target!");	
	//			GameObject.Find("Game").GetComponent(GameLogic).SetTargetTrooper(gameObject);
	//			//Select();
	//		}
	//    }
	//}
	
	//function OnMouseDown () {
	//	if (Input.GetMouseButton(0)) {
	//		//Debug.LogError("Left Click!");
	//		GameObject.Find("Game").GetComponent(GameLogic).SetSelection(gameObject);
	//		Select();
	//	}
	//}
	
	//function Select() {
	//	isSelected = true;
	//	transform.Find("SelectPlate").GetComponent(MeshRenderer).enabled = true;
	//	//transform.Find("TargetPlate").GetComponent(MeshRenderer).enabled = false;
	//	// if node has a target, show that target!
	//	if (this.TrooperTarget != null) {
	//		//TrooperTarget.GetComponent(Selection).DeSelect();
	//		TrooperTarget.GetComponent(Selection).TargetRenderOn(1);
	//	}
	//	//renderer.material = selectedMaterial;
	//	//selected = true;
	//}
	
	// 1) remove SelectPlate
	// 2) remove TargetPlate
	//function DeSelect() {
	//	isSelected = false;
	//	// disable selectPlate
	//	//transform.Find("SelectPlate").GetComponent(MeshRenderer).enabled = false;
	//	//transform.Find("TargetPlate").GetComponent(MeshRenderer).enabled = false;
	//	if (this.TrooperTarget != null) {
	//		//TrooperTarget.GetComponent(Selection).DeSelect();
	//		TrooperTarget.GetComponent(Selection).TargetRenderOff();
	//	}
	//	//TrooperTarget.GetComponent().RenderAsTarget();
	//	//renderer.material = defaultMaterial;
	//	//selected = false;
	//}

/*
*****************************************************
** FLOW CONTROL
*****************************************************
*/
	public void Select(Material m) {
		transform.Find("Selector").GetComponent<MeshRenderer>().material = m;
		transform.Find("Selector").GetComponent<MeshRenderer>().enabled = true;
		// transform.GetComponent<Route>().ShowMesh();
		isSelected = true;
		//transform.Find("SelectPlate").GetComponent(MeshRenderer).enabled = true;
		//transform.Find("TargetPlate").GetComponent(MeshRenderer).enabled = false;
		// if node has a target, show that target!
		//if (this.TrooperTarget != null) {
			//TrooperTarget.GetComponent(Selection).DeSelect();
			//TrooperTarget.GetComponent(Selection).TargetRenderOn(1);
		//}
		//renderer.material = selectedMaterial;
		//selected = true;
	}
	
	public void UnSelect() {
		transform.Find("Selector").GetComponent<MeshRenderer>().enabled = false;
		// transform.GetComponent<Route>().HideMesh();
		isSelected = false;
	}
	
	public void DeleteTargetTrooper() {
		transform.GetComponent<Route>().ClearRoute();
		transform.GetComponent<Route>().target = null;
		this.TrooperTarget = null;
	}
	
	public void SetTargetTrooper(Transform from, Transform to) {
		transform.GetComponent<Seeker>().StartPath(from.position,to.position);
		transform.GetComponent<Route>().target = to;
		//transform.GetComponent<AIPath>().target = to;
		// Disable old targetRender
	//	if (this.TrooperTarget != null) {
	//		TrooperTarget.GetComponent<Selectable>().TargetRenderOff();
	//	}
		this.TrooperTarget = to;
	//	this.TrooperTarget.GetComponent<Selectable>().TargetRenderOn(1);
		
		// release the hounds!
		releaseTheTroops();
	}
	
	// CurrentTrooperCount is increased thorugh PermissionToBoard()
	public void EnqeueUnit( Transform unit ) {
		
		if (unit.tag == "Trooper" ) {
			lock ( this.TrooperUnitsLock ) {
				unit.GetComponent<UnitLogic>().Dock(transform);
				//this.ApproachingTroopers -= 1;
				this.CurrentTrooperCount += 1;
				this.TrooperUnits.Add(unit);
			}	
		} else {
			lock ( this.BugUnitsLock ) {
				unit.GetComponent<UnitLogic>().Dock(transform);
				//this.ApproachingTroopers -= 1;
				this.CurrentBugCount += 1;
				this.BugUnits.Add(unit);
			}	
		}
		// - check if the selectable is a dropzone..
		// - and only make this check if the units entering the node is different from
		//   the race currently holding the node
		//if ( transform.tag == "DropZone" ) checkForTakeOver(unit.tag);
	}
	
//	private void checkForStatusAfterDeath(string tag) {
//		Debug.Log("unit" + tag+ " died, checking if we need to relase node.." + "(" +this.CurrentBugCount+"," + this.CurrentTrooperCount +")");
//		if ( this.CurrentTrooperCount == 0 && this.CurrentBugCount == 0 ) {
//			Debug.Log("unit" + tag+ " died and released the dropzone");
//			GetComponent<DropZone>().Neutral();
//		}
//	}
	
	public string GetDominantUnit() {
		
		//if ( this.CurrentBugCount > 0 )  
		
		if ( GetComponent<DropZone>().Unit.tag == "Trooper" ) {
			if ( this.CurrentBugCount <= 0 ) {
				return "Trooper";
			}
			
			return "Bug";
		} else {
			if ( this.CurrentTrooperCount <= 0 ) {
				return "Bug";
			} 
			
			return "Trooper";
		}
	}
	
	
	public List<Transform> BugTargets;
//	private void checkForTakeOver( string unit ) {
//		
//		if ( this.CurrentlyHeldBy != unit ) {
//			// if currently blocked by enemy units
//			if ( unit == "Bug" ) {
//				if ( this.CurrentTrooperCount <= 0 && this.CurrentBugCount > 0 ) {
//					Debug.LogWarning("Bugs just overtook a DropZone!");
//					//GetComponent<DropZone>().TakeOver("Bug");
//					this.CurrentlyHeldBy = "Bug";
//				}
//			} else {
//				if ( this.CurrentTrooperCount > 0 && this.CurrentBugCount <= 0 ) {
//					Debug.LogWarning("Troopers just overtook a DropZone!");
//					//GetComponent<DropZone>().TakeOver("Trooper");
//					this.CurrentlyHeldBy = "Trooper";
//				}
//			}
//		}
//	}
		
	public void releaseTheBugs() {
		
		if ( this.BugTargets == null || this.BugUnits.Count == 0 || this.BugTargets.Count < 1 ) {
			return;
		}
			
		if ( transform.tag == "DropZone" && this.CurrentBugCount < 2 )  {
			return;
		}
		
		lock(this.BugUnitsLock ) {
			int target = UnityEngine.Random.Range(0, this.BugTargets.Count);
			
			
//			Debug.LogWarning("chose between " + this.BugTargets.Count + ", chose " + (target+1) );
			
			Transform currentBugTarget = this.BugTargets[target];

			
			int permissable = currentBugTarget.GetComponent<Selectable>().PermissionToBoard(this.BugUnits.Count, "Bug");
			// no room for units at the next node
			if ( permissable == 0 ) { return; }
			
			//Transform unit = null;
			for ( int i = 0; i < permissable ; i++ ) {
				try {
					this.BugUnits[0].GetComponent<UnitLogic>().SetNewTarget(currentBugTarget);
					this.BugUnits.RemoveAt(0);
				} catch (Exception e) {
					
				}
				this.CurrentBugCount -= 1;
			}
		}
	}
	
	private void releaseTheTroops() {
		// only release units if the node has been unblocked
		if ( this.TrooperTarget == null ||
			 this.TrooperUnits.Count == 0 ) return;
		
		//Debug.LogError("TrooperUnits length: " + this.TrooperUnits.length);
		// 1) reserve space on the node
		lock(this.TrooperUnitsLock ) {
			int permissable = this.TrooperTarget.GetComponent<Selectable>().PermissionToBoard(this.TrooperUnits.Count, "Trooper");
			// no room for units at the next node
			//Transform unit = null;
			for ( int i = 0; i < permissable ; i++ ) {
				try {
					this.TrooperUnits[0].GetComponent<UnitLogic>().SetNewTarget(this.TrooperTarget);
					this.TrooperUnits.RemoveAt(0);
				} catch (Exception e) {
					
				}
				this.CurrentTrooperCount -= 1;
			}
		}
		//this.checkForTakeOver("");

	}
	
	// handles: enemies entering the node
	// - either passing through or spawning into
	public void OnTriggerEnter( Collider collider ) {
		// simple wrapper
		//Debug.LogError("Collission!");
		//Debug.LogError("Collission!");
		unitEntered(collider.transform);
	}
	
	// called by dropzone.js when a unit is spawned
	// and when a node's collision is triggered
	public void unitEntered(Transform unit) {
		//Debug.Log(unit.name+ "entered zone");
		// if unit is freshly spawned or has this node as a target (meaning it's not passing through)
		if ( unit.GetComponent<AIPath>().target == null || unit.GetComponent<AIPath>().target == transform ) {
			// trooper logic
			if (unit.tag == "Trooper" ) {
				
				this.ApproachingTroopers -= 1;
				//Debug.LogError("Approaching unit has arrived!: " + this.ApproachingTroopers);
				//Debug.LogError("Trooper entered!");
				if ( this.TrooperTarget != null &&
					 this.TrooperTarget.GetComponent<Selectable>().PermissionToBoard(1, unit.tag) == 1) {
					//Debug.LogError("Zone is unblocked and has a target!")
					// might be a race condition
					
					unit.GetComponent<UnitLogic>().SetNewTarget(this.TrooperTarget);
					return;
				}
				//unit.GetComponent( Route ).target = transform;
				//this.CurrentTrooperCount += 1;
				this.EnqeueUnit(unit);
			// bug logic
			} else {
				this.ApproachingBugs -= 1;
				//Debug.LogError("Bug entered zone!");
				if ( transform.tag == "DropZone" ) {
					this.EnqeueUnit(unit);					
					return;
				}
				
				//if ( this.BugTarget != null ) Debug.LogError("zone has a target!");
				
				if ( this.BugTarget != null &&
					 this.BugTarget.GetComponent<Selectable>().PermissionToBoard(1, unit.tag) == 1) {
					//Debug.LogError("Bug has a target!");
					// might be a race condition
					
					unit.GetComponent<UnitLogic>().SetNewTarget(this.BugTarget);
					return;
				}
				//unit.GetComponent( Route ).target = transform;
				//this.CurrentTrooperCount += 1;
				this.EnqeueUnit(unit);
			}
		}
	}
	
	/*****************************
	 ** AI 
	 ******************************/
	
	// set bug target
	public void SetTargetBug(Transform target) {
		this.BugTarget = target;
		// there is a race-condition here..
		// - just in case a number of units are added to the array as we set the target..
		//BugTarget.GetComponent(Selection).TargetRenderOn();
//		while (BugUnits.Count > 0 ) {
//			Transform unit = BugUnits.Dequeue();
//			this.CurrentBugCount--;
//			unit.transform.GetComponent<Route>().target = this.BugTarget;
//		}
	}
	
	/*
	*****************************************************
	** UNIT HELPER FUNCTIONS
	*****************************************************
	*/
	
	// returns 'true' if node is below capacity
	public bool AtMaxCapacity() {
		if ( this.TrooperCapacity - this.CurrentTrooperCount <= 0 ) {
			return true;
		} else {
			return false;
		}
	}
	
	// called by units with the node as their target as they die
	public void ApproachingUnitDied(string type) {
		//lock ( this.TrooperUnitsLock ) {
		if (type == "Trooper") {
			this.ApproachingTroopers -= 1;	
		} else {
			this.ApproachingBugs -= 1;
		}
			
			//Debug.LogError("Approaching trooper(" + id + ") Died! Count = " + this.ApproachingTroopers);
		//}
	}
	
	public void DockedUnitDied(Transform unit) {
		if ( unit.tag == "Trooper") {
			lock ( this.TrooperUnitsLock ) {
				
				for ( int i = 0; i < this.TrooperUnits.Count; i++ ) {
					if ( this.TrooperUnits[i] == unit) {
						//Debug.LogError("Docked trooper(" + unit.GetComponent<UnitLogic>().id + ") Died! Before = " + this.TrooperUnits.Count);
						this.TrooperUnits.RemoveAt(i);
						//Debug.LogError("Docked trooper(" + unit.GetComponent<UnitLogic>().id + ") Died! Count = " + this.TrooperUnits.Count);
						break;
					}
				}
				
				this.CurrentTrooperCount -= 1;
				//this.checkForStatusAfterDeath(unit.tag);
			}
		} else {
			lock ( this.BugUnitsLock ) {
				
				for ( int i = 0; i < this.BugUnits.Count; i++ ) {
					if ( this.BugUnits[i] == unit) {
						//Debug.LogError("Docked trooper(" + unit.GetComponent<UnitLogic>().id + ") Died! Before = " + this.TrooperUnits.Count);
						this.BugUnits.RemoveAt(i);
						//Debug.LogError("Docked trooper(" + unit.GetComponent<UnitLogic>().id + ") Died! Count = " + this.TrooperUnits.Count);
						break;
					}
				}
				//this.checkForStatusAfterDeath(unit.tag);
				this.CurrentBugCount -= 1;
			}
		}
	}
	
//	public void DeployUnit(Transform unit) {
//		//lock ( this.TrooperUnitsLock ) {
//			this.CurrentTrooperCount -= 1;
//			for ( int i =0; i < this.TrooperUnits.Count ; i++ ) {
//				if ( unit == this.TrooperUnits[i] ) {
//					this.TrooperUnits.RemoveAt(i);
//					//this.CurrentTrooperCount -= this.TrooperCount;
//				}
//			}
//		//}
//	}
	
	// if the node has capacity, reserve a space (increase troopercount) and return 'true'
	// - oelse return 'false'
	public int PermissionToBoard(int count, String tag) {
		
		if (tag == "Trooper") {
			
			lock(this.TrooperUnitsLock) {
				
				int permissableUnits = this.TrooperCapacity - (this.CurrentTrooperCount + this.ApproachingTroopers);
				
				if ( permissableUnits == 0  ) { return 0; }
				
				if ( permissableUnits >= count ) {
					//Debug.LogError(count + " units approaching.");
					this.ApproachingTroopers += count;
					return count;
				} else {
					//Debug.LogError(permissableUnits + " units approaching.");
					this.ApproachingTroopers += permissableUnits;
					return permissableUnits;
				}
			} // unlock
		} else {
			lock(this.BugUnitsLock) {
				
				int permissableUnits = this.BugCapacity - (this.CurrentBugCount + this.ApproachingBugs);
				
				if ( permissableUnits == 0  ) { return 0; }
				
				if ( permissableUnits >= count ) {
					//Debug.LogError(count + " units approaching.");
					this.ApproachingBugs += count;
					return count;
				} else {
					//Debug.LogError(permissableUnits + " units approaching.");
					this.ApproachingBugs += permissableUnits;
					return permissableUnits;
				}
			} // unlock
		}
	}
	
	/*
	*****************************************************
	** HEADS UP DISPLAY
	*****************************************************
	*/
	
	// handles marking nodes as targets or "next nodes" from a given node
	// - nodes with a targetNode != null are sequentially called to mark and label the redirect path
	//public void TargetRenderOn(int val) {
	//	if (isTarget == true || isSelected == true) return;
	//	isTarget = true;
	//	targetNumber = val;
	//	transform.Find("TargetPlate").GetComponent(MeshRenderer).enabled = true;
	//	if ( TrooperTarget == null ) return;
	//	val = val + 1;
	//	TrooperTarget.GetComponent(Selection).TargetRenderOn(val);
	//}
	
	//public void TargetRenderOff() {
	//	if (isTarget == false) return;
	//	isTarget = false;
	//	targetNumber = -1;
	//	transform.Find("TargetPlate").GetComponent(MeshRenderer).enabled = false;
	//	if ( TrooperTarget == null ) return;
	//	TrooperTarget.GetComponent(Selection).TargetRenderOff();
	//}
	
	//public void OnGUI() {
	//	if ( !isTarget ) return;
	//    var pos = transform.position;
	//	pos.y += 15;
	//	pos = Camera.main.WorldToScreenPoint(pos); 
	//	var rect = new Rect(pos.x - 10, Screen.height - pos.y - 15, 100, 22);
	//	GUI.color = Color.black;
	//    GUI.Label(rect, targetNumber.ToString() );
	//}
	//	
}

