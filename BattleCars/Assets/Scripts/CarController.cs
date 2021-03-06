﻿using UnityEngine;
using System.Collections;
using System;

public class CarController : MonoBehaviour {
    #region SerializedFields
    //[SerializeField]
    //ObserverEventHandler observerEventHandler;
    [SerializeField]
    float maxSteeringAngle = 40f;
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    float maxMotorTorque = 500f;
    [SerializeField]
    float brakeTorque = 50f;
    [SerializeField]
    float downForce = 150f;
    [Range(0, 1)] [SerializeField]
    float steeringAssistance;
    [SerializeField]
    Vector3 centerOfMassOffset;
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
    [SerializeField]
    float powerUpMaxTorque;
    [SerializeField]
    float powerUpTime;
    [SerializeField]
    float maxAccelerationHelperVelocity;
    [SerializeField]
    float accelerationHelperIncriment = -5f;
    [SerializeField]
    RocketSimple rocketSimplePrefab;
    [SerializeField]
    RocketTargetted rocketTargettedPrefab;
    [SerializeField]
    GameObject basicMinigun;
    [SerializeField]
    Vector3 rocketOffset;
    [SerializeField]
    private float deathTime = 3f;
    #endregion

    #region Private variables

    float steeringInput;
    float driveInput;
    float brakeInput;
    Rigidbody rigidBody;
    float currentRotation;
    float currentTorque;
    float tireSidewaysStiffnessDefault;
    AnimationCurve torqueCurveModifier;
    bool isTorquePowerupActive;
    float startingMass;
    GameObject activeWeapon;
    int childObjectCountWithoutWeapon;
    bool canDrive;
    GameManager gameManager;
    #endregion

    #region Private properties
    bool hasWeapon{ get { return childObjectCountWithoutWeapon < transform.childCount ? true : false; } }
    private float ForwardVelocity{ get { return rigidBody.transform.InverseTransformDirection(rigidBody.velocity).z; } }
    private bool IsMovingForward{ get { return ForwardVelocity > 0; } }
    //private bool IsRocketAttached { get { return GetComponentInChildren<SkinnedMeshRenderer>().enabled;}}
    #endregion

    public int PlayerNumber = 1;

    void Start() {
        try { gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); }
        catch (Exception) { throw new Exception("Scene must contain a GameManager"); }

        try { rigidBody = GetComponent<Rigidbody>(); }
        catch (Exception) { throw new Exception("CarController must be attached to a GameObject with a Rigidbody component."); }

        rigidBody.centerOfMass = centerOfMassOffset;
        tireSidewaysStiffnessDefault = wheelsAll[0].sidewaysFriction.stiffness;
        torqueCurveModifier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(maxSpeed, 0.25f));
        startingMass = rigidBody.mass;
        childObjectCountWithoutWeapon = transform.childCount;
        canDrive = true;
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
        if (canDrive)
            Drive();
        AccelerationHelper();
        if (!hasWeapon)
            rigidBody.mass = startingMass;
        CheckRollOver();
    }

    private void CheckRollOver() {
        Quaternion normalRotation = new Quaternion(0,0,0,0);
        if (transform.rotation.x > normalRotation.x + 135)
        {
            DieAndRespawnAtLocation();
        }
    }
    #region Driving Mechanics
    private void AccelerationHelper()
    {
        if (ForwardVelocity <= maxAccelerationHelperVelocity)
        {
            rigidBody.AddForce(rigidBody.transform.InverseTransformDirection(rigidBody.velocity) * accelerationHelperIncriment);
        }
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

        if (wheelHit.sidewaysSlip > maxSidewaysSlip || wheelHit.forwardSlip > maxForwardSlip)
            sidewaysFrictionCurve.stiffness += frictionModifier;
        else
            sidewaysFrictionCurve.stiffness = tireSidewaysStiffnessDefault;


        for (int i = 0; i < wheelsAll.Length; i++)
            wheelsAll[i].sidewaysFriction = sidewaysFrictionCurve;

    }
    
    private void UpdateMotorTorque() {

        float curveMod = torqueCurveModifier.Evaluate(rigidBody.velocity.magnitude);
        if (isTorquePowerupActive)
            currentTorque = driveInput * powerUpMaxTorque * curveMod;
        else
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

    void PowerUpTorque()
    {
        StartCoroutine(TorqueIncreasePowerUp());
    }
    #endregion

    void GetInput() {
        steeringInput = Input.GetAxis("Steering" + PlayerNumber);
        driveInput = Input.GetAxis("Gas" + PlayerNumber);
        brakeInput = Input.GetAxis("Brakes" + PlayerNumber);
    }

    private IEnumerator TorqueIncreasePowerUp()
    {
        isTorquePowerupActive = true;
        yield return new WaitForSeconds(powerUpTime);
        isTorquePowerupActive = false;
    }

    public void AddWeapon(WeaponType weapon) {
        if (!hasWeapon)
            switch (weapon)
            {
                case WeaponType.Rocket:
                        activeWeapon = Instantiate(rocketSimplePrefab, transform, false) as GameObject;
                    rigidBody.centerOfMass = new Vector3(rocketOffset.x, centerOfMassOffset.y, centerOfMassOffset.z);
                    rigidBody.mass += 2000f;
                    break;
                case WeaponType.Laser:
                    activeWeapon = basicMinigun;
                    break;
                case WeaponType.Minigun:
                    activeWeapon = basicMinigun;
                    break;
                case WeaponType.TargettedRocket:
                    activeWeapon = Instantiate(rocketTargettedPrefab, transform, false) as GameObject;
                    rigidBody.mass += 4000f;
                    break;
                default:
                    activeWeapon = basicMinigun;
                    break;
            }
    }
    #endregion

    void Respawn() {
        StartCoroutine(DieAndRespawnAtLocation());
    }

    IEnumerator DieAndRespawnAtLocation() {
        canDrive = false;
        yield return new WaitForSeconds(deathTime);

        gameManager.PlayerDied(PlayerNumber);
        canDrive = true;
        gameObject.transform.position = new Vector3(0, 10, 0);
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

}
