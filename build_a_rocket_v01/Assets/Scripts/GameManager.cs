using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

//this is a test comment
//this is also a test comment

public class GameManager : MonoBehaviour {

	public Text countdownTimer;
	public float initialTimerValue = 120f; // time in seconds

	private float timeElapsed = 0f;
	private float remainingTime = 0f;
	private TimeSpan t_timer_start;

	public Text weight;
	public Text airResistance;
	public Text power;
	public Text fuel;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// update the timer
		remainingTime = initialTimerValue - timeElapsed;
		if (remainingTime > 0.0) {
			countdownTimer.text = FormatTime(remainingTime);
		} else {
			countdownTimer.text = "00:00";
			// we can trigger the launch here
		}
		// increment the time elapsed
		timeElapsed += Time.deltaTime;

	}

	// restarts the game for each trial
	void RestartGame() {
		// reset the timer
		timeElapsed = 0;
	}

	string FormatTime(float value) {
		TimeSpan t = TimeSpan.FromSeconds (value);
		return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
	}
}

// testing merging code
