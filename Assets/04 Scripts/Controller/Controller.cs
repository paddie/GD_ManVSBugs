using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {
	
	/** Mask for the raycast placement */
	public LayerMask selectionLayer;
	public Material selectionMaterial;
	public Material arrowMaterial;
	
	private List<Transform> selected = new List<Transform>();
	private Transform start;
	private Transform end;
	
	public void Start () {
	}
	
	public void OnGUI () {
		Ray ray;
		RaycastHit hit;
		if (Input.GetMouseButtonDown (0)) {
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, selectionLayer)) {
				// Select(hit.transform);
				if (selected.Count>0) {
					ClearSelection();	
				}
				Select(hit.transform);
			} else {
				ClearSelection();
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, selectionLayer)) {
				Select(hit.transform);
			} else {
				if (selected.Count>0) {
					selected[selected.Count -1].GetComponent<Selectable>().DeleteTargetTrooper();
					//selected[selected.Count-1].GetComponent<Route>().ClearRoute();
					//selected[selected.Count-1].GetComponent<Route>().target = null;
					ClearSelection();
				}
			}
		}
	}

	void Update () {
		
	}
	
	void SetStart(Transform t) {
		ClearSelection();
		start = t;
		selected.Add (t);
		t.GetComponent<Selectable>().Select(selectionMaterial);
	}
	
	void SetEnd(Transform t) {
		if (start != null && end == null && !start.Equals(end)) {
			end = t;
			selected.Add (t);
			t.GetComponent<Selectable>().Select(selectionMaterial);
			start.GetComponent<Seeker>().StartPath(start.position,end.position);
			start = t;
			end = null;
		}
	}
	
	void Select(Transform t) {
		selected.Add (t);
		t.GetComponent<Selectable>().Select(selectionMaterial);
		int count = selected.Count;
		Transform lastSelected;
		if (count>1) {
			lastSelected = selected[count-2];
		if (lastSelected.position!=t.position)
			if (t.GetComponent<Route>().target != lastSelected) {
				lastSelected.GetComponent<Selectable>().SetTargetTrooper (lastSelected, t);
				//lastSelected.GetComponent<Seeker>().StartPath(lastSelected.position,t.position);
				//lastSelected.GetComponent<Route>().target = t;
			}
		} else if (count > 2) {
			Transform origin = selected[count-3];
			ClearSelection();
			Select (origin);
		}
	}
	
	void ClearSelection() {
		end = null;
		start = null;
		foreach (Transform t in selected) {
			t.GetComponent<Selectable>().UnSelect();
		}
		selected.Clear ();
	}
}