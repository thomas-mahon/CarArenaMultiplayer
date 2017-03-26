using UnityEngine;
using System.Collections;

public enum WeaponType {
Rocket, Laser, Minigun
};


public interface IDoDamage{
    WeaponType Weapon{get; set;}
    int Damage{get;set;}
}
