using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour {
    [SerializeField]
    List<GameObject> pickupPrefabs = new List<GameObject>();
    [SerializeField]
    float dropFrequency;
    //[SerializeField]
    //int maxUpgradesInScene;
    [SerializeField]
    float minXCreationBox;
    [SerializeField]
    float maxXCreationBox;
    [SerializeField]
    float minZCreationBox;
    [SerializeField]
    float maxZCreationBox;

    float dropCTR;

    void Start() {
        dropCTR = 0;
    }
    // Update is called once per frame
    void Update () {
        dropCTR += Time.deltaTime;
        if (dropCTR >= dropFrequency)
        {
            int prefabSelectionIndex = Random.Range(0, pickupPrefabs.ToArray().Length);
            Vector3 instantiatePosition = new Vector3(Random.Range(minXCreationBox, maxXCreationBox), 0, Random.Range(minZCreationBox, maxZCreationBox));

            GameObject pickup = Instantiate(pickupPrefabs[prefabSelectionIndex], instantiatePosition, transform.rotation) as GameObject;
            dropCTR = 0;
        }
	}
}
