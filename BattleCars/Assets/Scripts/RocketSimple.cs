using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class RocketSimple : MonoBehaviour, IWeapon {
    [SerializeField]
    float destructTimer;
    [SerializeField]
    int damage;
    [SerializeField]
    string shootingInput;
    [SerializeField]
    Transform rocketSpawnPoint;
    [SerializeField]
    GameObject rocketPrefab;
    

    public int Damage{ get{ return damage;}}
    public float LifeSpan{ get{ return destructTimer;}}
    public bool IsActive { get { return isActiveAndEnabled;} }

    GameManager gameManager;

    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(GetComponent<IWeapon>().DestroySelf(destructTimer));
        shootingInput = "Shoot" + GetComponentInParent<CarController>().PlayerNumber;
    }

    IEnumerator IWeapon.DestroySelf(float LifeSpan) {
        yield return new WaitForSeconds(destructTimer);
        Destroy(gameObject);
    }

    void Update() {
        if (Input.GetButtonDown(shootingInput))
            Shoot();
    }

    private void Shoot() {
        gameManager.PlayerShotRocket(GetComponentInParent<CarController>().PlayerNumber);
        GameObject rocket = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation) as GameObject;
        rocket.transform.parent = transform;
    }
}
