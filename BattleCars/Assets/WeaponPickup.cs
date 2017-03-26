using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider))]
public class WeaponPickup : MonoBehaviour {
    
    [SerializeField]
    WeaponType weaponType;

    void OnTriggerEnter(Collider coll) {
        if (coll.tag == "Player")
        {
            coll.GetComponentInParent<CarController>().AddWeapon(weaponType);
        }
    }
}
