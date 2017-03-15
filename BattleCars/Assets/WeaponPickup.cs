using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    enum WeaponType { Rocket, Laser, Minigun};
    [SerializeField]
    WeaponType weaponType;
    [SerializeField]
    GameObject rocketWeapon;
    [SerializeField]
    GameObject laserWeapon;
    [SerializeField]
    GameObject minigunWeapon;

    private GameObject weapon;

    // Use this for initialization
    void Start () {
        switch (weaponType)
        {
            case WeaponType.Rocket:
                weapon = rocketWeapon;
                break;
            case WeaponType.Laser:
                weapon = laserWeapon;
                break;
            case WeaponType.Minigun:
                weapon = minigunWeapon;
                break;
            default:
                break;
        }
    }
	
}
