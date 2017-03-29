using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    GameObject selectedObject;
    bool buttonSelected = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
	}

    void OnDisable() {

        buttonSelected = false;
    }
}
