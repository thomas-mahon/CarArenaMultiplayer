using UnityEngine;
using System.Collections;

public class PauseMenuOpen : MonoBehaviour {
    [SerializeField]
    GameObject menuPanel;

	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Cancel"))
        {
            foreach (Transform playerTransform in gameObject.GetComponent<GameManager>().PlayersInRound)
            {
                playerTransform.gameObject.GetComponent<CarController>().enabled = false;
            }
            menuPanel.SetActive(true);
        }
	}

    void ResumeGame() {
        foreach (Transform playerTransform in gameObject.GetComponent<GameManager>().PlayersInRound)
        {
            playerTransform.gameObject.GetComponent<CarController>().enabled = true;
        }
        menuPanel.SetActive(false);
    }
}
