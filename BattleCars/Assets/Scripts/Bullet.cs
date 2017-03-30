using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    [SerializeField]
    float forceCoefficient = 150;

    // Use this for initialization
    void Awake() {
        GetComponent<Rigidbody>().AddForce(transform.forward * forceCoefficient);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().Damage(1);
        }
        Destroy(gameObject);
    }
}
