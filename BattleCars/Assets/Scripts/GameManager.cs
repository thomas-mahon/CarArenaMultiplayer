using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum WeaponType {
        Rocket, Laser, Minigun, TargettedRocket
    };
public class GameManager : MonoBehaviour {
    
    public List<Transform> PlayersInRound = new List<Transform>();
    [SerializeField]
    int maxKills;
    [SerializeField]
    float timeLimit;
    [SerializeField]
    Slider[] playerHealthSliders;
    [SerializeField]
    GameObject wrapUpPanel;

    float[] playerScores;
    int[] playerDeaths;
    int[] playerKills;
    int[] playerMacGunHits;
    int[] playerMacGunShots;
    int[] playerRocketHits;
    int[] playerRocketShots;
    float[] playerAccuracy;
    void Start() {
        playerScores = new float[PlayersInRound.ToArray().Length];
        playerDeaths = new int[PlayersInRound.ToArray().Length];
        playerKills = new int[PlayersInRound.ToArray().Length];
        playerMacGunHits = new int[PlayersInRound.ToArray().Length];
        playerMacGunShots = new int[PlayersInRound.ToArray().Length];
        playerAccuracy = new float[PlayersInRound.ToArray().Length];
        playerRocketHits = new int[PlayersInRound.ToArray().Length];
        playerRocketShots = new int[PlayersInRound.ToArray().Length];
        StartCoroutine(TimedGame());
        for (int i = 0; i < PlayersInRound.ToArray().Length; i++)
        {
            playerScores[i] = 0;
            playerDeaths[i] = 0;
            playerKills[i] = 0;
            playerMacGunHits[i] = 0;
            playerMacGunShots[i] = 0;
            playerAccuracy[i] = 0;
            playerRocketHits[i] = 0;
            playerRocketShots[i] = 0;
        }
    }
    
    //TODO: Implement Observer pattern for slider value management
    public int[] GetPlayerKills() {
        return playerKills;
    }
    public int[] GetPlayerDeaths() {
        return playerDeaths;
    }
    public float[] GetPlayerAccuracy() {
        return playerAccuracy;
    }
    public float[] GetPlayerScores() {
        return playerScores;
    }

    // Update is called once per frame
    void Update () {
        foreach (Transform playerTransform in PlayersInRound)
        {
            playerHealthSliders[playerTransform.gameObject.GetComponent<CarController>().PlayerNumber -1].value = playerTransform.gameObject.GetComponent<PlayerHealth>().CurrentHealth;
        }
        

	}

    IEnumerator TimedGame() {
        yield return new WaitForSeconds(timeLimit);
        EndGame();
    }

    void EndGame() {
        foreach (Transform t in PlayersInRound)
        {
            t.GetComponent<CarController>().enabled = false;
        }
        for (int i = 0; i < playerHealthSliders.Length; i++)
        {
            playerHealthSliders[i].enabled = false;
        }
        CalculateFinalScores();
        wrapUpPanel.SetActive(true);
        wrapUpPanel.GetComponent<PostGameWrapUp>().FinalizeScores();
    }

    public void PlayerDied(int playerNumber) {
        playerDeaths[playerNumber - 1]++;
    }
    public void PlayerShotMacGun(int playerNumber) {
        playerMacGunShots[playerNumber - 1]++;
    }
    public void PlayerHitMacGun(int playerNumber) {
        playerMacGunHits[playerNumber - 1]++;
    }
    public void PlayerKill(int playerNumber) {
        playerKills[playerNumber - 1]++;
        if (playerKills[playerNumber - 1] >= maxKills)
            EndGame();
    }
    public void PlayerShotRocket(int playerNumber) {
        playerRocketShots[playerNumber - 1]++;
    }
    public void PlayerHitRocket(int playerNumber) {
        playerRocketHits[playerNumber - 1]++;
    }

    public void CalculateFinalScores() {
        for (int i = 0; i < PlayersInRound.ToArray().Length; i++)
        {
            if(playerMacGunShots[i] != 0 || playerRocketShots[i] != 0)
                playerAccuracy[i] = (playerRocketHits[i] + playerMacGunHits[i]) / (playerMacGunShots[i] + playerRocketShots[i]);

            playerScores[i] = ((500 * playerKills[i]) - (200 * playerDeaths[i])) * playerAccuracy[i];
        }
    }
}
