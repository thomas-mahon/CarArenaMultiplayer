﻿using UnityEngine;
using System.Collections;
using System;

public class CarController : MonoBehaviour {
    #region SerializedFields

    [SerializeField]
    float maxSteeringAngle = 40f;
    [SerializeField]
    AnimationCurve torqueCurveModifier =
        new AnimationCurve(new Keyframe(0, 1), new Keyframe(200, 0.25f));
    [SerializeField]
    float maxMotorTorque = 500f;
    [SerializeField]
    float brakeTorque = 50f;
    [SerializeField]
    float downForce = 150f;
    [Range(0, 1)] [SerializeField]
    float steeringAssistance;
    [SerializeField]
    float centerOfMassOffset = -0.2f;
    [Range(0, 1)] [SerializeField]
    float tractionControlAdjustmentAmount = 1f;
    [SerializeField]
    float slipLimit = 0.3f;
    [SerializeField]
    float maxSidewaysSlip = 0.3f;
    [SerializeField]
    float maxForwardSlip = 0.3f;
    [SerializeField]
    float frictionModifier = 0.1f;
    [SerializeField]
    WheelCollider[] wheelsUsedForSteering;
    [SerializeField]
    WheelCollider[] wheelsUsedForDriving;
    [SerializeField]
    WheelCollider[] wheelsAll;
    [SerializeField]
    GameObject[] wheelMeshes;
    #endregion

    #region Private variables

    float steeringInput;
    float driveInput;
    float brakeInput;
    Rigidbody rigidBody;
    float currentRotation;
    float currentTorque;
    float tireSidewaysStiffnessDefault;

    #endregion

    #region Private properties
    private float ForwardVelocity{get { return rigidBody.transform.InverseTransformDirection(rigidBody.velocity).z;}}
    private bool IsMovingForward{get { return ForwardVelocity > 0;}}
    #endregion

    void Start() {

        try { rigidBody = GetComponent<Rigidbody>(); }
        catch (Exception) { throw new Exception("SimpleCarController must be attached to a GameObject with a Rigidbody component."); }
        rigidBody.centerOfMass = new Vector3(transform.position.x, centerOfMassOffset, transform.position.z);
        tireSidewaysStiffnessDefault = wheelsAll[0].sidewaysFriction.stiffness;
        //for (int i = 0; i < wheelsAll.Length; i++)
        //{
        //    wheelsAll[i].suspensionDistance = rideHeight;
        //}
    }

    #region UpdateFunctions

    void Update() {
        GetInput();
    }

    void FixedUpdate() {
        Turn();
        SteeringHelper();
        UpdateMotorTorque();
        TractionControl();
        UpdateDownforce();
        UpdateBrakeTorque();
        FixWheelMeshesToColliders();
        Drive();
    }

    private void Turn() {
        for (int i = 0; i < wheelsUsedForSteering.Length; i++)
            wheelsUsedForSteering[i].steerAngle = steeringInput * maxSteeringAngle;
    }
    //Standard Assets CarController HelpSteer();
    private void SteeringHelper() {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            wheelsAll[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(currentRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - currentRotation) * steeringAssistance;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rigidBody.velocity = velRotation * rigidBody.velocity;
        }
        currentRotation = transform.eulerAngles.y;
    }

    private void UpdateWheelFriction(WheelHit wheelHit) {
        WheelFrictionCurve sidewaysFrictionCurve = wheelsAll[0].sidewaysFriction;
        WheelFrictionCurve forwardFrictionCurve = wheelsAll[3].forwardFriction;

        if (wheelHit.sidewaysSlip > maxSidewaysSlip || wheelHit.forwardSlip > maxForwardSlip)
            sidewaysFrictionCurve.stiffness += frictionModifier;
        else
            sidewaysFrictionCurve.stiffness = tireSidewaysStiffnessDefault;


        for (int i = 0; i < wheelsAll.Length; i++)
            wheelsAll[i].sidewaysFriction = sidewaysFrictionCurve;

    }
    
    private void UpdateMotorTorque() {

        float curveMod = torqueCurveModifier.Evaluate(rigidBody.velocity.magnitude);
        currentTorque = driveInput * maxMotorTorque * curveMod;
        
    }
    
    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    private void TractionControl() {
        WheelHit wheelHit;
        for (int i = 0; i < 4; i++)
        {
            wheelsAll[i].GetGroundHit(out wheelHit);

            AdjustTorque(wheelHit.forwardSlip);

            UpdateWheelFriction(wheelHit);
        }
    }
    //Standard Assets CarController AdjustTorque()
    private void AdjustTorque(float forwardSlip) {
        if (forwardSlip >= slipLimit && currentTorque >= 0)
        {
            currentTorque -= 10 * tractionControlAdjustmentAmount;
        }
        else
        {
            currentTorque += 10 * tractionControlAdjustmentAmount;
            if (currentTorque > maxMotorTorque)
            {
                currentTorque = maxMotorTorque;
            }
        }
    }
    //Standard Assets CarController UpdateDownforce()
    private void UpdateDownforce() {
        for (int i = 0; i < wheelsAll.Length; i++)
            wheelsAll[i].attachedRigidbody.AddForce(-transform.up * downForce * wheelsAll[i].attachedRigidbody.velocity.magnitude);
    }
    private void UpdateBrakeTorque() {
        //Get forwardVelocity of current motion in LOCAL space instead of GLOBAL space
        float brakeTorqueToApply = 0f;
        bool isMovingForwardAndApplyingBrakes = IsMovingForward == (brakeInput < 0);

        if (isMovingForwardAndApplyingBrakes)
            brakeTorqueToApply = brakeTorque;
        for (int i = 0; i < wheelsAll.Length; i++)
        {
            wheelsAll[i].brakeTorque = brakeTorqueToApply;
        }
    }
    private void FixWheelMeshesToColliders() {
        //fix the wheel meshes to the wheel colliders
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            Quaternion rotation;
            Vector3 position;
            wheelsAll[i].GetWorldPose(out position, out rotation);
            wheelMeshes[i].transform.position = position;
            wheelMeshes[i].transform.rotation = rotation;
        }
    }
    private void Drive() {;
        for (int i = 0; i < wheelsUsedForDriving.Length; i++)
        {
            wheelsUsedForDriving[i].motorTorque = IsMovingForward ? currentTorque : brakeInput * maxMotorTorque;
            //wheelsUsedForDriving[i].brakeTorque = brakeInput < 0 ? brakeInput * brakeTorque : 0.0f;
        }
    }
    #endregion

    void GetInput() {
        steeringInput = Input.GetAxis("Horizontal");
        driveInput = Input.GetAxis("Gas1");
        brakeInput = Input.GetAxis("Brakes1");
    }
}