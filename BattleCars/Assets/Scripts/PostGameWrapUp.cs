using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PostGameWrapUp : MonoBehaviour {
    [SerializeField]
    Text[] killsTexts;
    [SerializeField]
    Text[] deathsTexts;
    [SerializeField]
    Text[] accuracyTexts;
    [SerializeField]
    Text[] scoreTexts;

    GameManager gameManager;
    int[] playerKills;
    int[] playerDeaths;
    float[] playerAccuracy;
    float[] playerScore;

    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //playerKills = new int[gameManager.PlayersInRound.ToArray().Length];
        //playerDeaths = new int[gameManager.PlayersInRound.ToArray().Length];
        //playerAccuracy = new float[gameManager.PlayersInRound.ToArray().Length];
        //playerScore = new float[gameManager.PlayersInRound.ToArray().Length];
        playerKills = gameManager.GetPlayerKills();
        playerDeaths = gameManager.GetPlayerDeaths();
        playerAccuracy = gameManager.GetPlayerAccuracy();
        playerScore = gameManager.GetPlayerScores();

    }
    public void FinalizeScores() {
        for (int i = 0; i < gameManager.PlayersInRound.ToArray().Length; i++)
        {
            killsTexts[i].text = "" + playerKills[i];
            deathsTexts[i].text = "" + playerDeaths[i];
            accuracyTexts[i].text = "" + playerAccuracy[i];
            scoreTexts[i].text = "" + playerScore[i];
        }
    }
}
