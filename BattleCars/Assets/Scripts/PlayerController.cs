using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
    [SerializeField] float maxTorque;
    [SerializeField] float topSpeed;
    [SerializeField] WheelCollider [] wheelColliders;
    [SerializeField] int playerNumber = 1;
    [SerializeField] float maxSteeringAngle = 45;
    Rigidbody rigidbodyUseThis;
    float acceleratorInput;
    float steeringInput;
    float brakingInput;
    string acceleratorInputAxisName;
    string steeringInputAxisName;
    string brakingInputAxisName;
    float currentSpeed;
    float steeringAngle;
    float currentTorque;
    private float brakeTorque;

    // Use this for initialization
    void Start () {
        rigidbodyUseThis = GetComponent<Rigidbody>();
        acceleratorInputAxisName = "Gas" + playerNumber;
        steeringInputAxisName = "Steering" + playerNumber;
        brakingInputAxisName = "Brakes" + playerNumber;

        currentTorque = maxTorque;
	}
	
	// Update is called once per frame
	void Update () {
        //TODO get input from the player
        acceleratorInput = Input.GetAxis(acceleratorInputAxisName);
        steeringInput = Input.GetAxis(steeringInputAxisName);
	}

    void FixedUpdate() {
        Move();
    }
    

    private void Move() {
        steeringInput = Mathf.Clamp(steeringInput, -1, 1);
        acceleratorInput = Mathf.Clamp(acceleratorInput, 0, 1);
        brakingInput = Mathf.Clamp(brakingInput, 0, 1);

        steeringAngle = steeringInput * maxSteeringAngle;
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        Drive();
    }

    private void Drive() {
        float torqueForward = acceleratorInput * (currentTorque / 4.0f);
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = torqueForward;
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rigidbodyUseThis.velocity) < 50f)
                wheelColliders[i].brakeTorque = brakeTorque * brakingInput;
        }
        if (currentSpeed >= topSpeed)
            rigidbodyUseThis.velocity = topSpeed * rigidbodyUseThis.velocity.normalized;

    }
}
