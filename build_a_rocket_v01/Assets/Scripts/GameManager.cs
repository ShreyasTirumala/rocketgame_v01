using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


//this is a test comment
//this is also a test comment

public class GameManager : MonoBehaviour {

	public Text countdownTimer;
	public float initialTimerValue = 120f; // time in seconds

	public Text weight;
	public Text airResistance;
	public Text power;
	public Text fuel;
	public Text t1;
	public Text t2;
	public Text t3;
	public Text t4;
	public Text t5;

	private float timeElapsed = 0f;
	private float remainingTime = 0f;
	private TimeSpan t_timer_start;

	private bool launched = false;
	private float alphaSet = 0f;

	private int distance = 0;
	private int maxDistance;

	private float fov;



	private int d1;
	private int d2;
	private int d3;
	private int d4;
			private int d5;

	private bool doOnce;


	void Awake () {
		var saved = GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ();
		d1 = saved.d1;
		d2 = saved.d2;
		d3 = saved.d3;
		d4 = saved.d4;
		d5 = saved.d5;

		GameObject jet = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 1/Jet");
		jet.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet1 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 2/Jet 1");
		jet1.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet2 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 3/Jet 2");
		jet2.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet3 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 4/Jet 3");
		jet3.GetComponent<ParticleSystem>().enableEmission = false;

		//GameObject d = GameObject.Find("Canvas/Distance");
		//d.transform.position = new Vector3(-1000, -1000, 0);

		//GameObject.Find ("Results").SetActive (false);

		fov =  Camera.main.orthographicSize;
		doOnce = false;

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
			GameObject leftPanel = GameObject.Find ("LeftPiecePanel");
			leftPanel.transform.Translate (Vector3.down);
			GameObject rightPanel = GameObject.Find ("RightPiecePanel");
			rightPanel.transform.Translate (Vector3.down);
			var script = gameObject.GetComponent<TouchInputHandler>();

			script.endSequence();

			GameObject black = GameObject.Find ("black");

			if (launched == false) {
				launched = true;

				GameObject timer = GameObject.Find("Canvas/Timer");
				timer.transform.position = new Vector3(-1000, -1000, 0);
				GameObject w = GameObject.Find("Canvas/Weight");
				w.transform.position = new Vector3(-1000, -1000, 0);
				GameObject ar = GameObject.Find("Canvas/Resistance");
				ar.transform.position = new Vector3(-1000, -1000, 0);
				GameObject f = GameObject.Find("Canvas/Fuel");
				f.transform.position = new Vector3(-1000, -1000, 0);
				GameObject p = GameObject.Find("Canvas/Power");
				p.transform.position = new Vector3(-1000, -1000, 0);


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
			
			//important!! change the position of the distance object here
			GameObject d = GameObject.Find("Canvas/Distance");
			//d.transform.position = new Vector3(621, 334, 0);
			d.GetComponent<Text>().text = distance.ToString();

			maxDistance = script.calculateDistance();
			if (distance < maxDistance) {
				distance +=5;
				if (launched == true) {
					alphaSet += .0005f;
					
				}
			}else {
				distance = maxDistance;
				if (doOnce == false) {
				if (d1 == -1) {
					d1 = maxDistance;
				}else if (d2 == -1) {
					d2 = maxDistance;
				}else if (d3 == -1) {
					d3 = maxDistance;
				}else if (d4 == -1) {
					d4 = maxDistance;
				}else {
					d5 = maxDistance;
				}
					doOnce = true;
				}

				var results = GameObject.Find ("Results");
				results.GetComponent<Animator>().SetTrigger ("go");

					GameObject jet4 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 1/Jet");
					jet4.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet5 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 2/Jet 1");
					jet5.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet6 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 3/Jet 2");
					jet6.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet7 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 4/Jet 3");
					jet7.GetComponent<ParticleSystem> ().enableEmission = false;
				if (d1 != -1) 
					GameObject.Find("Canvas/Trial1").transform.position = results.transform.position - new Vector3 (15, -16, 0);
				if (d2 != -1) 
					GameObject.Find("Canvas/Trial2").transform.position = results.transform.position - new Vector3 (15, -7, 0);
				if (d3 != -1) 
					GameObject.Find("Canvas/Trial3").transform.position = results.transform.position - new Vector3 (15, 2, 0);
				if (d4 != -1) 
					GameObject.Find("Canvas/Trial4").transform.position = results.transform.position - new Vector3 (15, 11, 0);
				if (d5 != -1) 
					GameObject.Find("Canvas/Trial5").transform.position = results.transform.position - new Vector3 (15, 20, 0);

				t1.text = "Trial 1: " + d1.ToString();
				t2.text = "Trial 2: "+ d2.ToString();
				t3.text = "Trial 3: "+d3.ToString();
				t4.text = "Trial 4: "+d4.ToString();
				t5.text = "Trial 5: "+d5.ToString();


			}


			Camera.main.orthographicSize = fov;
			//fov += .05f;
			GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d1 = d1;
			GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d2 = d2;
			GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d3 = d3;
			GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d4 = d4;
			GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d5 = d5;

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
