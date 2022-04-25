using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
public class MainMenuManager : MonoBehaviour
{
	public TextMeshProUGUI text1, text2, text3, text4, text5, text6, text7;

	public void ExitGame()
	{
		Application.Quit();
	}

	public void LoadLevel1()
	{
		SceneManager.LoadScene("Level 1");
	}

	public void LoadLevel2()
	{
		SceneManager.LoadScene("Level 2");
	}

	public void LoadLevel3()
	{
		SceneManager.LoadScene("Level 3");
	}

	public void LoadLevel4()
	{
		SceneManager.LoadScene("Level 4");
	}

	public void LoadLevel5()
	{
		SceneManager.LoadScene("Level 5");
	}

	public void LoadLevel6()
	{
		SceneManager.LoadScene("Level 6");
	}

	public void LoadLevel7()
	{
		SceneManager.LoadScene("Level 7");
	}

	// Update rating
	private void Start()
	{
		getRatingPerLevel(text1, 1);
		getRatingPerLevel(text2, 2);
		getRatingPerLevel(text3, 3);
		getRatingPerLevel(text4, 4);
		getRatingPerLevel(text5, 5);
		getRatingPerLevel(text6, 6);
		getRatingPerLevel(text7, 7);
	}

	private void getRatingPerLevel(TextMeshProUGUI text1, int index)
	{
		if (File.Exists("Level " + index.ToString() + " stats.txt"))
		{
			int value = int.Parse(File.ReadAllText("Level " + index.ToString() + " stats.txt"));
			text1.text = "Rating " + value.ToString() + "/3";
			if (value == 0)
				text1.color = Color.red;
			else if (value < 3)
				text1.color = Color.yellow;
			else
				text1.color = Color.green;
		}
		else
		{
			text1.text = "Rating " + "0/3";
			text1.color = Color.red;
		}
	}
}
