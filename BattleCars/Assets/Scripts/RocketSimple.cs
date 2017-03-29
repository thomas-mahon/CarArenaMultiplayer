using UnityEngine;
using System.Collections;
using System;

public class RocketSimple : MonoBehaviour, IWeapon {
    [SerializeField]
    float destructTimer;
    [SerializeField]
    int damage;
    public int Damage{ get{ return damage;}}
    public float LifeSpan{ get{ return destructTimer;}}
    public bool IsActive { get { return isActiveAndEnabled;} }

    void Awake() {
        StartCoroutine(GetComponent<IWeapon>().DestroySelf(destructTimer));
    }

    IEnumerator IWeapon.DestroySelf(float LifeSpan) {
        yield return new WaitForSeconds(destructTimer);
        Destroy(gameObject);
    }

}
