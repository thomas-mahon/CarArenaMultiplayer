using UnityEngine;
using System.Collections;
using System;

public class RocketSimple : MonoBehaviour {
    [SerializeField]
    float destructTimer;


    void Awake() {
        StartCoroutine(DestroySelf(destructTimer));
    }

    private IEnumerator DestroySelf(float destructTimer) {
        yield return new WaitForSeconds(destructTimer);

        Destroy(gameObject);
    }

}
