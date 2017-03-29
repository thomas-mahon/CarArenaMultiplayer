using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class RocketTargetted : MonoBehaviour, IWeapon {
    [SerializeField]
    int damage;
    [SerializeField]
    float maxRange;
    [SerializeField]
    float fieldOfViewRangeForPrimaryTargeting;
    [SerializeField]
    float lifeSpan;
    [SerializeField]
    string shootButton;
    [SerializeField]
    string targetSelectLeft;
    [SerializeField]
    string targetSelectRight;
    [SerializeField]
    GameObject targettingReticle;

    GameManager gameManager;
    private List<Transform> playerTransforms { get { return gameManager.PlayersInRound; } }
    Transform target = null;
    bool targetLocated = false;
    bool targetIsInRange { get { return ((target.position - transform.position).magnitude <= maxRange); } }

    public int Damage {get{ return damage;}}
    public float LifeSpan {get{ return lifeSpan;}}
    public bool IsActive { get { return isActiveAndEnabled;} }

    bool isTargettingLeft;
    bool isShooting;
    bool isTargettingRight;
    enum Direction {
    left, right };
    void Awake () {
        try { gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); }
        catch (Exception) { throw new Exception("Scene must contain an object called GameManager with the GameManager script attached"); }
        SetTargetToPrimary();
        if (target != null)
            SetReticleToTarget();
        StartCoroutine(GetComponent<IWeapon>().DestroySelf(lifeSpan));
	}

    private void SetReticleToTarget() {
        targettingReticle.transform.position = target.position;
        targettingReticle.GetComponentInChildren<Image>().enabled = true;
    }

    void Update () {
        if (targetLocated && targetIsInRange)
            SetReticleToTarget();
        else
            targettingReticle.GetComponentInChildren<Image>().enabled = false;

        if (!targetLocated)
            SetTargetToPrimary();
        else
            GetInput();
	}

    private void GetInput() {
        isTargettingLeft = Input.GetButtonDown(targetSelectLeft);
        isTargettingRight = Input.GetButtonDown(targetSelectRight);
        isShooting = Input.GetButtonDown(shootButton);
    }

    IEnumerator IWeapon.DestroySelf(float destructTimer) {
        yield return new WaitForSeconds(destructTimer);
        
        Destroy(gameObject);
    }

    private void FixedUpdate() {
        if (isTargettingLeft)
            NextTarget(Direction.left);
    }

    private void NextTarget(Direction direction) {
        Transform[] playerTransformArray = playerTransforms.ToArray();
        Transform closestTransformX = playerTransformArray[0];

        for (int i = 1; i < playerTransformArray.Length; i++)
        {
            if (playerTransformArray[i].position.x - transform.position.x < closestTransformX.position.x - transform.position.x)
                closestTransformX = playerTransformArray[i];
        }

        switch (direction)
        {
            case Direction.left:

                break;
            case Direction.right:

                break;
            default:
                break;
        }
    }

    private void SetTargetToPrimary() {
        RaycastHit hit;
        targetLocated = false;
        foreach (var playerTarget in playerTransforms)
        {
            Vector3 rayDirection = playerTarget.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out hit, maxRange) && Vector3.Angle(rayDirection, transform.forward) <= fieldOfViewRangeForPrimaryTargeting)
            {
                if ((hit.transform.tag == "Player") && (hit.transform != transform.parent))
                {
                    target = hit.transform;
                    targetLocated = true;
                    continue;
                }
            }

        }
        if (!targetLocated)
        {
            foreach (var playerTarget in playerTransforms)
            {
                Vector3 rayDirection = playerTarget.position - transform.position;
                if (Physics.Raycast(transform.position, rayDirection, out hit, maxRange))
                {
                    if (hit.transform.tag == "Player" && (hit.transform != transform.parent))
                    {
                        target = hit.transform;
                        targetLocated = true;
                        continue;
                    }
                }
            }
        }

    }
    
}
