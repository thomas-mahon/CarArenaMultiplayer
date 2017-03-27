using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    public enum WeaponType {
        Rocket, Laser, Minigun, TargettedRocket
    };
public class GameManager : MonoBehaviour {
    public List<Transform> PlayersInRound = new List<Transform>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
