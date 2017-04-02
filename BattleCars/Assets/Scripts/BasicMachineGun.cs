using UnityEngine;
using System.Collections;

public class BasicMachineGun : MonoBehaviour {
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    float shotTime;

    GameManager gameManager;
    string shootButton;
    bool canShoot;

    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canShoot = true;
        shootButton = "Shoot" + GetComponentInParent<CarController>().PlayerNumber;

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
        gameManager.PlayerShotMacGun(GetComponentInParent<CarController>().PlayerNumber);
        GameObject shot = Instantiate(bullet, transform.position, transform.rotation) as GameObject;
        shot.transform.parent = transform;
    }
}
