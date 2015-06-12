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

	private bool launched = false;
	private float alphaSet = 0f;


	void Awake () {
		GameObject jet = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 1/Jet");
		jet.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet1 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 2/Jet 1");
		jet1.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet2 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 3/Jet 2");
		jet2.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet3 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 4/Jet 3");
		jet3.GetComponent<ParticleSystem>().enableEmission = false;
	}
	
	// Update is called once per frame
	void Update () {
		// update the timer
		remainingTime = initialTimerValue - timeElapsed;
		if (remainingTime > 0.0) {
			countdownTimer.text = FormatTime (remainingTime);
		} else {
			countdownTimer.text = "00:00";
			// we can trigger the launch here

			GameObject foreground = GameObject.Find ("Foreground");
			foreground.transform.Translate (Vector3.down);
			GameObject bottomPanel = GameObject.Find ("BottomPanel");
			bottomPanel.transform.Translate (Vector3.down);

			GameObject black = GameObject.Find ("black");

			if (launched == false) {
				launched = true;

				Canvas canvasObject = (Canvas)FindObjectOfType (typeof(Canvas));
				{
					canvasObject.enabled = false;
				}
				GameObject jet = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 1/Jet");
				jet.GetComponent<ParticleSystem> ().enableEmission = true;
				GameObject jet1 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 2/Jet 1");
				jet1.GetComponent<ParticleSystem> ().enableEmission = true;
				GameObject jet2 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 3/Jet 2");
				jet2.GetComponent<ParticleSystem> ().enableEmission = true;
				GameObject jet3 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 4/Jet 3");
				jet3.GetComponent<ParticleSystem> ().enableEmission = true;

				var pos = black.transform.position;
				pos = new Vector3(0, 0, 0);
				black.transform.position = pos;

			}
			var a =	black.GetComponent<SpriteRenderer>().color;
			a = new Color(1f, 1f, 1f, alphaSet);
			black.GetComponent<SpriteRenderer>().color = a;

			if (launched == true) {
				alphaSet += .001f;
			}
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
