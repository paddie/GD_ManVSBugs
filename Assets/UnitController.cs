using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
	}
}
