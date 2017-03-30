using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour {

    enum DamageStatus {
    none, light, medium, heavy, critical };

    public float CurrentHealth{get{return currentHealth;}}

    [SerializeField]
    GameObject[] damageIndicatorPrefabs;
    [SerializeField]
    Transform vfxSpawnPoint;
    
    [SerializeField]
    float maxHealth;

    GameObject showDamage;
    DamageStatus damageStatus;
    float currentHealth;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
        damageStatus = DamageStatus.none;
        showDamage = new GameObject();
	}

    public void Damage(float damage) {
        currentHealth -= damage;
        if (currentHealth >= 75)
            UpdateDamageStatus(DamageStatus.none);
        else if (currentHealth < 75 && currentHealth >= 50)
            UpdateDamageStatus(DamageStatus.light);
        else if (currentHealth < 50 && currentHealth >= 25)
            UpdateDamageStatus(DamageStatus.medium);
        else if (currentHealth < 25 && currentHealth >= 1)
            UpdateDamageStatus(DamageStatus.heavy);
        else if (currentHealth < 1)
            UpdateDamageStatus(DamageStatus.critical);
    }

    private void UpdateDamageStatus(DamageStatus damageStatusTemp) {
        

        switch (damageStatusTemp)
        {
            case DamageStatus.light:
                ShowDamageLevel(0);
                break;
            case DamageStatus.medium:
                ShowDamageLevel(1);
                break;
            case DamageStatus.heavy:
                ShowDamageLevel(2);
                break;
            case DamageStatus.critical:
                ShowDamageLevel(3);
                RespawnWithCarController();
                break;
            default:
                break;
        }
    }

    private void ShowDamageLevel(int i) {
        Destroy(showDamage);
        showDamage = new GameObject();
        showDamage = Instantiate(damageIndicatorPrefabs[i], vfxSpawnPoint) as GameObject;
        showDamage.transform.parent = vfxSpawnPoint;
        showDamage.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void RespawnWithCarController() {
        transform.gameObject.GetComponent<CarController>().SendMessage("Respawn");
        currentHealth = maxHealth;
        damageStatus = DamageStatus.none;
        Damage(0);
    }
}
