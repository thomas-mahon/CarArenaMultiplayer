using UnityEngine;
using System.Collections;

public class RocketShot : MonoBehaviour {
    [SerializeField]
    float forceCoefficient = 150;

    GameManager gameManager;
    // Use this for initialization
    void Awake () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetComponent<Rigidbody>().AddForce(transform.forward * forceCoefficient);
        StartCoroutine(DestroySelf());
	}

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().Damage(25);
            gameManager.PlayerHitMacGun(GetComponentInParent<CarController>().PlayerNumber);
            if (!collision.gameObject.GetComponent<PlayerHealth>().CheckIsAlive())
                gameManager.PlayerKill(GetComponentInParent<CarController>().PlayerNumber);
        }
            Destroy(gameObject);
    }

    IEnumerator DestroySelf() {
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }

}
