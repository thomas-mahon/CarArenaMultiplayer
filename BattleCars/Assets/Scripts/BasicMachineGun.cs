using UnityEngine;
using System.Collections;

public class BasicMachineGun : MonoBehaviour {
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    string shootButton;
    [SerializeField]
    float shotTime;

    bool canShoot;

    void Awake() {
        canShoot = true;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetAxis(shootButton) > 0 && canShoot)
        {
            StartCoroutine(Shoot());
            canShoot = false;
        }
	}

    IEnumerator Shoot() {
        yield return new WaitForSeconds(shotTime);
        canShoot = true;
        
        Instantiate(bullet, transform.position, transform.rotation);
    }
}
