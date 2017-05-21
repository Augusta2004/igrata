using UnityEngine;
using System.Collections;

public class GameAudio : MonoBehaviour
{
	public GameObject backgroundSound;
	public GameObject killEnemySound;
	public GameObject levelUpSound;
	public GameObject gameOverSound;
	
	public void PlayGameSound(string sound)
	{
		GameObject soundObj = new GameObject ();
		switch (sound)
		{
		case "kill": 
			soundObj = killEnemySound; 
			break;
			
		case "level": 
			soundObj = levelUpSound; 
			break;
			
		case "over": 
			soundObj = gameOverSound; 
			break;
		}
		
		PlayGameSound (soundObj);
	}
	
	private void PlayGameSound(GameObject sound)
	{
		sound.SetActive (true);
		sound.GetComponent<AudioSource>().Play();
	}
	
	public void LowerBackgroundSound()
	{
		backgroundSound.GetComponent<AudioSource> ().volume = 0.6f;
	}
	
	public void HigherBackgroundSound()
	{
		backgroundSound.GetComponent<AudioSource> ().volume = 0.4f;
	}
}
