using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider))]
public class WeaponPickup : MonoBehaviour {
    [SerializeField]
    WeaponType weaponType;

    private bool hasWeapon = true;

    void OnTriggerEnter(Collider coll) {
        if (coll.tag == "Player" && hasWeapon)
        {
            coll.GetComponentInParent<CarController>().AddWeapon(weaponType);
            hasWeapon = false;
        }
    }
}
