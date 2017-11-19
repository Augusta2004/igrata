using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class YummyGameUI : MonoBehaviour
{
	public Text levelText;
	public Text scoreText;
	public Text gameOverText;
	public Button close;
	public GameObject fader;
	
	public void DisplayScore(int score, int lives)
	{
		scoreText.text = score + "\n" + lives + " lives";
	}

	public void DisplayLevel(int level)
	{
		levelText.text = "lvl " + level;
	}
	
	public void HideLevelAndScore()
	{
		scoreText.text = "";
		levelText.text = "";
	}
	
	public void DisplayGameOver(int score)
	{
		fader.SetActive (true);
		gameOverText.gameObject.SetActive (true);
		gameOverText.text = "GAME OVER \n Score: " + score;
	}
	
	public void ShowCloseButton()
	{
		close.gameObject.SetActive (true);
	}
}
