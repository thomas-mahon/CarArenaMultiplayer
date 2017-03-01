using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
    [SerializeField] float maxAccelerationTorque;
    [SerializeField] float topSpeed;
    [SerializeField] WheelCollider [] wheelColliders;


    Rigidbody rigidbodyUseThis;
    float verticalInput;
    float horizontalInput;

	// Use this for initialization
	void Start () {
        rigidbodyUseThis = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	    //TODO get input from the player
	}

    void FixedUpdate() {
        Move();
        Turn();
    }

    private void Turn() {
        
    }

    private void Move() {
        
    }
}
