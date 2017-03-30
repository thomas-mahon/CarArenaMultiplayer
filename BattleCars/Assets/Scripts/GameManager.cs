using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum WeaponType {
        Rocket, Laser, Minigun, TargettedRocket
    };
public class GameManager : MonoBehaviour {



    public List<Transform> PlayersInRound = new List<Transform>();

    [SerializeField]
    Slider[] playerHealthSliders;
	

    //TODO: Implement Observer pattern for slider value management

	// Update is called once per frame
	void Update () {
        foreach (Transform playerTransform in PlayersInRound)
        {
            playerHealthSliders[playerTransform.gameObject.GetComponent<CarController>().PlayerNumber -1].value = playerTransform.gameObject.GetComponent<PlayerHealth>().CurrentHealth;
        }
	}
}
