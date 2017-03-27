using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
    bool hasBeenUsed = false;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player") && !hasBeenUsed)
        {
            coll.gameObject.GetComponentInParent<CarController>().SendMessage("PowerUpTorque");
            hasBeenUsed = true;
        }
    }
}
