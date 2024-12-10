using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
	[SerializeField]
	WaveSpawner spawner;

	[SerializeField]
	Animator waveAnimator;

	[SerializeField]
	Text waveCountdownText;

	[SerializeField]
	Text waveCountText;

	[Header("Wave Count UI")]
	[SerializeField]
	TextMeshProUGUI waveCount;

	[Header("Wave Timer")]
	[SerializeField]
	public TextMeshProUGUI timerText;  // UI Text to display the timer
	public float survivalTime = 0f;
	public bool isCounting = true;

	private WaveSpawner.SpawnState previousState;

	[Header("Wave Player's Stats")]
	public int currentWave;

	// Use this for initialization
	void Start()
	{
		survivalTime = 0f;  // Initialize the timer
		isCounting = true;

		if (spawner == null)
		{
			Debug.LogError("No spawner referenced!");
			this.enabled = false;
		}
		if (waveAnimator == null)
		{
			Debug.LogError("No waveAnimator referenced!");
			this.enabled = false;
		}
		if (waveCountdownText == null)
		{
			Debug.LogError("No waveCountdownText referenced!");
			this.enabled = false;
		}
		if (waveCountText == null)
		{
			Debug.LogError("No waveCountText referenced!");
			this.enabled = false;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (isCounting)
		{
			survivalTime += Time.deltaTime;  // Increase time by the delta time
			DisplayTime(survivalTime);  // Update the UI text
		}
		switch (spawner.State)
		{
			case WaveSpawner.SpawnState.COUNTING:
				UpdateCountingUI();
				break;
			case WaveSpawner.SpawnState.SPAWNING:
				UpdateSpawningUI();
				break;
		}

		previousState = spawner.State;
	}

	public void StopTimer()
	{
		isCounting = false;  // Stop counting
	}

	void DisplayTime(float timeToDisplay)
	{
		int minutes = Mathf.FloorToInt(timeToDisplay / 60);
		int seconds = Mathf.FloorToInt(timeToDisplay % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  // Display in MM:SS format
	}

	void UpdateCountingUI()
	{
		if (previousState != WaveSpawner.SpawnState.COUNTING)
		{
			waveAnimator.SetBool("WaveInComing", false);
			waveAnimator.SetBool("WaveCountDown", true);
			//Debug.Log("COUNTING");

		}
		// Sử dụng thuộc tính công khai WaveCountdown thay vì truy cập trực tiếp waveCountdown
		// waveCountdownText.text = Mathf.CeilToInt(spawner.WaveCountdown).ToString();
		waveCountdownText.text = ((int)spawner.WaveCountdown).ToString();
	}

	void UpdateSpawningUI()
	{
		if (previousState != WaveSpawner.SpawnState.SPAWNING)
		{
			waveAnimator.SetBool("WaveCountDown", false);
			waveAnimator.SetBool("WaveInComing", true);

			waveCountText.text = spawner.NextWave.ToString();
			currentWave = spawner.NextWave;
			waveCount.SetText(spawner.NextWave.ToString());
			//Debug.Log("SPAWNING");
		}
	}
}
