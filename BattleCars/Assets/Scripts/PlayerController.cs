using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
    [SerializeField] float maxTorque;
    [SerializeField] float brakeTorque;
    [SerializeField] float topSpeed;
    [SerializeField] float downforce;
    [SerializeField] int playerNumber = 1;
    [SerializeField] float maxSteeringAngle = 45;
    [SerializeField] WheelCollider [] wheelColliders;
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
        UpdateDownforce();

    }

    private void UpdateDownforce() {
        for (int i = 0; i < wheelColliders.Length; i++)
            wheelColliders[i].attachedRigidbody.AddForce(-transform.up * downforce * wheelColliders[i].attachedRigidbody.velocity.magnitude);
    }

    private void Move() {
        steeringInput = Mathf.Clamp(steeringInput, -1, 1);
        acceleratorInput = Mathf.Clamp(acceleratorInput, 0, 1);
        brakingInput = Mathf.Clamp(brakingInput, -1, 0);

        steeringAngle = steeringInput * maxSteeringAngle;
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        Drive();
    }

    private void Drive() {
        float torqueForward = acceleratorInput * currentTorque;
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = torqueForward;
            if (Math.Sign(rigidbodyUseThis.transform.InverseTransformDirection(rigidbodyUseThis.velocity).z) == Math.Sign(brakingInput))
                wheelColliders[i].brakeTorque = brakeTorque * brakingInput;
            else if (brakingInput > 0)
            {
                wheelColliders[i].brakeTorque = brakingInput * brakeTorque;
                wheelColliders[i].motorTorque = 0.0f;
            }
        }
        if (currentSpeed >= topSpeed)
            rigidbodyUseThis.velocity = topSpeed * rigidbodyUseThis.velocity.normalized;

    }
}
