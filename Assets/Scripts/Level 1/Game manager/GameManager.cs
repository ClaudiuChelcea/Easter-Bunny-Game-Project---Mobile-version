using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
	public enum EggsLife
	{
		egg1 = 15,
		egg2 = 20,
		egg3 = 25
	};
	
	// Get objects
	public PlayerMovement player;
	public TextMeshProUGUI energyCD;
	public AudioClip get_audio;
	private AudioSource gameMusic, winSound;
	public GameObject pause_menu;
	public TextMeshProUGUI score;
	public TextMeshProUGUI egg1Text;
	public TextMeshProUGUI egg2Text;
	public TextMeshProUGUI egg3Text;
	public TextMeshProUGUI totalEggsText;
	public GameObject EGG1, EGG2, EGG3;
	public GameObject lose_menu, win_menu;
	public TextMeshProUGUI carrotsAnswer, carrotsAnswer2, carrots_needed_to_finish;
	public AudioClip win_sound, lose_sound;

	// Variables
	int egg1Seconds = (int) EggsLife.egg1;
	int egg2Seconds = (int) EggsLife.egg2;
	int egg3Seconds = (int) EggsLife.egg3;
	private int count_eggs_received = 3;
	private int nr_carrots_per_level = 3;
	bool won = false;
	bool lost = false;

	// Variables
	bool gameIsPaused = false;

	// Timer
	public int time_spent_in_level = 0;
	IEnumerator time()
	{
		while (true)
		{
			timeCount();
			yield return new WaitForSeconds(1);
		}
	}
	
	void timeCount()
	{
		time_spent_in_level += 1;
	}

	// Pause game
	public void PauseGameButton()
	{
		if (gameIsPaused == false)
			Pause();
		else
			resumeLevel();
	}

	// Check win
	void win()
	{
		if (won)
			return;

		if (player.playerScore == nr_carrots_per_level && player.is_at_the_exit == true)
		{
			won = true;
			winSound.PlayOneShot(win_sound);
			Time.timeScale = 0;
			gameIsPaused = true;
			gameMusic.Stop();
			win_menu.active = true;
			carrotsAnswer2.text = "Got " + player.playerScore.ToString() + " carrots!";
			if (count_eggs_received < 3)
			{
				totalEggsText.color = Color.yellow;
			}
			else
			{
				totalEggsText.color = Color.green;
			}
			totalEggsText.text = "Got rating " + count_eggs_received.ToString() + "!";

			// Write output to file
			if (File.Exists("Level " + SceneManager.GetActiveScene().buildIndex + " stats.txt"))
			{
				int prev_rating = int.Parse(File.ReadAllText("Level " + SceneManager.GetActiveScene().buildIndex + " stats.txt"));
				if (count_eggs_received > prev_rating)
					File.WriteAllText("Level " + SceneManager.GetActiveScene().buildIndex + " stats.txt", count_eggs_received.ToString());
			}
			else
			{
				File.WriteAllText("Level " + SceneManager.GetActiveScene().buildIndex + " stats.txt", count_eggs_received.ToString());
			}
		}
	}

	// Awake
	private void Awake()
	{
		energyCD.alpha = 0;
		score.text = player.playerScore.ToString();
		egg1Text.text = ((int) EggsLife.egg1).ToString() + "s";
		egg2Text.text = ((int)EggsLife.egg2).ToString() + "s";
		egg3Text.text = ((int)EggsLife.egg3).ToString() + "s";
		Time.timeScale = 1;
		carrots_needed_to_finish.text = "0/3";
		carrots_needed_to_finish.color = Color.red;
	}

	// Start
	private void Start()
	{
		var audio_sources = GetComponents<AudioSource>();
		gameMusic = audio_sources[0];
		gameMusic.PlayOneShot(get_audio);
		pause_menu.active = false;
		StartCoroutine(time());
		lose_menu.active = false;
		win_menu.active = false;
		winSound = audio_sources[1];
		won = false;
		lost = false;
	}

	// Update
	private void Update()
	{
		CD_energy_bar();
		check_pause();
		update_score();
		modify_eggs();
		win();
		how_many_carrots_to_win();

		if(player.isDead)
			loseLevel();
	}

	// Display how many carrots are needed to win
	void how_many_carrots_to_win()
	{
		if(player.playerScore == 0)
		{
			carrots_needed_to_finish.text = "0/3";
			carrots_needed_to_finish.color = Color.red;
		} else if(player.playerScore == 1)
		{
			carrots_needed_to_finish.text = "1/3";
			carrots_needed_to_finish.color = Color.yellow;
		} else if(player.playerScore == 2)
		{
			carrots_needed_to_finish.text = "2/3";
			carrots_needed_to_finish.color = Color.yellow;
		} else
		{
			carrots_needed_to_finish.text = "3/3";
			carrots_needed_to_finish.color = Color.green;
		}
	}

	// Modify text colors based on seconds left
	private void colors_per_time(int seconds_left, int low, int mid, TextMeshProUGUI text)
	{
		if(seconds_left > mid)
		{
			text.color = Color.green;
		} else if(seconds_left <= mid && seconds_left > low)
		{
			text.color = Color.yellow;
		} else
		{
			text.color = Color.red;
		}
	}

	// Eggs. If all eggs die, lose the game!
	private void modify_eggs()
	{
		if(egg1Seconds > 0)
		{
			egg1Seconds = (int) EggsLife.egg1 - time_spent_in_level;
			colors_per_time(egg1Seconds, 5, 10, egg1Text);
			egg1Text.text = egg1Seconds.ToString() + "s";
			count_eggs_received = 3;
		} else if (egg2Seconds > 0)
		{
			egg1Text.text = 0.ToString();
			egg2Seconds = (int) EggsLife.egg2 - time_spent_in_level + (int) EggsLife.egg1;
			egg2Text.text = egg2Seconds.ToString() + "s";
			colors_per_time(egg2Seconds, 7, 13, egg2Text);
			Destroy(EGG1);
			count_eggs_received = 2;
		} else if(egg3Seconds > 0)
		{
			Destroy(EGG2);
			egg1Text.text = 0.ToString() + "s";
			egg2Text.text = 0.ToString() + "s"; 
			egg3Seconds = (int) EggsLife.egg3 - time_spent_in_level + (int) EggsLife.egg2 + (int) EggsLife.egg1;
			egg3Text.text = egg3Seconds.ToString() + "s";
			colors_per_time(egg3Seconds, 10, 18, egg3Text);
			count_eggs_received = 1;
		} else
		{
			Destroy(EGG3);
			count_eggs_received = 0;
			loseLevel();
		}
	}

	// Lose game
	private void loseLevel()
	{
		if (lost)
			return;

		lost = true;
		Time.timeScale = 0;
		gameIsPaused = true;
		gameMusic.Stop();
		gameMusic.PlayOneShot(lose_sound);
		lose_menu.active = true;
		carrotsAnswer.text = "Got " + player.playerScore.ToString() + " carrots!";
	}

	// CD for energy bar
	private void CD_energy_bar()
	{
		// Cooldown for energy regen, display only if regening
		if (player.energySlider.value != 1)
		{
			energyCD.alpha = 1;
			float cd = player.timeUntilStartRegeneratingEnergy - player.startRegeneratingEnergyTime;

			if (cd >= 0f)
			{
				energyCD.text = "Regenerating...";
			}
			else
			{
				energyCD.text = "Regen in: " + (-cd).ToString("F2");
			}

			if (cd > 0f)
			{
				energyCD.color = Color.green;
			}
			else if (cd >= 0.5f && cd <= 1.5f)
			{
				energyCD.color = Color.yellow;
			}
			else
			{
				energyCD.color = Color.red;
			}
		}
		else
		{
			energyCD.alpha = 0;
		}
	}

	// Score
	private void update_score()
	{
		score.text = player.playerScore.ToString();
	}

	// Pause
	private void check_pause()
	{
		if (Input.GetKeyDown(KeyCode.Escape) == true && gameIsPaused == true)
		{
			resumeLevel();
		}
		else if (Input.GetKeyDown(KeyCode.Escape) == true && gameIsPaused == false)
		{
			Pause();
		}
	}

	private void Pause()
	{
		Time.timeScale = 0;
		gameIsPaused = true;
		gameMusic.Pause();
		pause_menu.active = true;
	}

	// Resume button
	public void resumeLevel()
	{
		Time.timeScale = 1;
		gameIsPaused = false;
		gameMusic.UnPause();
		pause_menu.active = false;
	}

	// Restart button
	public void restartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		resumeLevel();
	}

	// Main menu
	public void goToMainMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}
}
