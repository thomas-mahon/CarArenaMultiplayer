using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
            coll.gameObject.GetComponentInParent<CarController>().SendMessage("PowerUpTorque");
    }
}
