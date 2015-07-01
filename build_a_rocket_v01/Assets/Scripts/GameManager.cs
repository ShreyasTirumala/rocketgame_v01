using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


//this is a test comment
//this is also a test comment

public class GameManager : MonoBehaviour {

	public Text countdownTimer;
	public float initialTimerValue = 120f; // time in seconds
	public float secondInitialValue = 30f;

	public int totalTrialsNumber = 5;

	public Text weight;
	public Text airResistance;
	public Text power;
	public Text fuel;
	public Text t1;
	public Text t2;
	public Text t3;
	public Text t4;
	public Text t5;
	
	// sounds being used
	public AudioClip countdownBeep;
	public AudioClip liftoffSound;

	// if the game has started
	public bool gameStarted = false;

	private float timeElapsed = 0f;
	private float timeElapsed2 = 0f;
	private float remainingTime = 0f;
	private int remainingTimeSec = 0;
	private float remainingTime2 = 0f;
	private int remainingTimeSec2 = 0;
	private TimeSpan t_timer_start;

	private bool launched = false;
	private float alphaSet = 0f;

	private int distance = 0;
	private int maxDistance;

	private float fov;

	public bool canRestart = false;

	private int d1;
	private int d2;
	private int d3;
	private int d4;
	private int d5;

	private int d1_old = -1;
	private int d2_old = -1;
	private int d3_old = -1;
	private int d4_old = -1;
	private int d5_old = -1;

	private bool doOnce;

	private Vector3 timerSavePosition;

	// audio source for sound effects
	private AudioSource audioSource1;
	private AudioSource audioSource2;

	// animator for the results panel
	private Animator resultsAnimator;

	// value to indicate whether the updated results have been shown
	private bool shownResults = true;

	// the object used to send all the messages to Thalamus
	// ETHAN
	// private ThalamusUnity thalamusUnity;

	void Awake () {
		var saved = GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ();
		d1 = saved.d1;
		d2 = saved.d2;
		d3 = saved.d3;
		d4 = saved.d4;
		d5 = saved.d5;

		gameStarted = saved.gameStarted;

		if (saved.gameStarted) {
			HideUIElements ();
		} else {
			GameObject.Find ("Canvas/GameOverText").GetComponent<Text> ().enabled = false;
		}

		GameObject jet = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 1/Jet");
		jet.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet1 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 2/Jet 1");
		jet1.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet2 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 3/Jet 2");
		jet2.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jet3 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 4/Jet 3");
		jet3.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jetLeft = GameObject.Find("RocketSprites/SelectedOutlines/FinSelectedOutlines/left_fin_selected_outline/Jet");
		jetLeft.GetComponent<ParticleSystem>().enableEmission = false;
		GameObject jetRight = GameObject.Find("RocketSprites/SelectedOutlines/FinSelectedOutlines/right_fin_selected_outline/Jet");
		jetRight.GetComponent<ParticleSystem>().enableEmission = false;


		// show the distance stats
		GameObject distanceDisplay = GameObject.Find("Canvas/Distance");
		distanceDisplay.GetComponent<Text> ().enabled = false;

		GameObject miles = GameObject.Find ("Canvas/Meters");
		miles.GetComponent<Text> ().enabled = false;


		//GameObject.Find ("Results").SetActive (false);

		fov =  Camera.main.orthographicSize;
		doOnce = false;
		
		// initialize the thalamusUnity object
		// ETHAN
		// thalamusUnity = new ThalamusUnity();

		// initialize the audio sources
		var audioSources = GetComponents<AudioSource> ();
		audioSource1 = audioSources [0];
		//audioSource2 = audioSources [1];
	}
	
	void Start ()
	{
		resultsAnimator = GameObject.Find ("Results").GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (gameStarted) {
			// update the timer
			remainingTime = initialTimerValue - timeElapsed;
			if (remainingTime > 0.0) {
				if (GetSeconds (remainingTime) != remainingTimeSec) {
					// get remaining seconds
					remainingTimeSec = GetSeconds (remainingTime);
					// set the timer text
					SetCountdownTimerText (FormatTime2 (remainingTimeSec));
					// play the beeping sound if it's 5,4,3,2,1 seconds left
					if (remainingTimeSec == 1 || remainingTimeSec == 2 || remainingTimeSec == 3 || 
						remainingTimeSec == 4 || remainingTimeSec == 5) {
						audioSource1.PlayOneShot (countdownBeep, 1F);
					}
				}
			} else {
				// we can trigger the launch here


				GameObject foreground = GameObject.Find ("Foreground");
				foreground.transform.Translate (Vector3.down);
				GameObject bottomPanel = GameObject.Find ("BottomPanel");
				bottomPanel.transform.Translate (Vector3.down);
				GameObject leftPanel = GameObject.Find ("LeftPiecePanel");
				leftPanel.transform.Translate (Vector3.down);
				GameObject rightPanel = GameObject.Find ("RightPiecePanel");
				rightPanel.transform.Translate (Vector3.down);

				var script = gameObject.GetComponent<TouchInputHandler> ();

				script.endSequence ();

				GameObject black = GameObject.Find ("black");

				if (launched == false) {
					launched = true;

					// show the distance stats
					GameObject distanceDisplay = GameObject.Find ("Canvas/Distance");
					distanceDisplay.GetComponent<Text> ().enabled = true;
					
					GameObject miles = GameObject.Find ("Canvas/Meters");
					miles.GetComponent<Text> ().enabled = true;

					GameObject timer = GameObject.Find ("Canvas/Timer");
					timerSavePosition = timer.transform.position;
					timer.GetComponent<Text> ().enabled = false;
					//timer.transform.position = new Vector3(-1000, -1000, 0);


					GameObject w = GameObject.Find ("Canvas/Weight");
					w.transform.position = new Vector3 (-1000, -1000, 0);
					GameObject ar = GameObject.Find ("Canvas/Resistance");
					ar.transform.position = new Vector3 (-1000, -1000, 0);
					GameObject f = GameObject.Find ("Canvas/Fuel");
					f.transform.position = new Vector3 (-1000, -1000, 0);
					GameObject p = GameObject.Find ("Canvas/Power");
					p.transform.position = new Vector3 (-1000, -1000, 0);


					foreach (GameObject piece in GameObject.Find("GameManager").GetComponent<TouchInputHandler>().rocketPieces) {
						if (piece.name.Contains ("fin_Engine") || piece.name.Contains ("fin_Propeller")) {
							// if it's on the left side
							if (piece.transform.position.x < 0) {
								GameObject jetLeft = GameObject.Find ("RocketSprites/SelectedOutlines/FinSelectedOutlines/left_fin_selected_outline/Jet");
								jetLeft.GetComponent<ParticleSystem> ().enableEmission = true;
							} 
							// or if it's on the right side 
							else {
								GameObject jetRight = GameObject.Find ("RocketSprites/SelectedOutlines/FinSelectedOutlines/right_fin_selected_outline/Jet");
								jetRight.GetComponent<ParticleSystem> ().enableEmission = true;
							}
						}
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
					pos = new Vector3 (0, 0, 0);
					black.transform.position = pos;
					
					
					// set the clip of audioSource2 to the liftoff sound
					audioSource1.clip = liftoffSound;
					
					// start the liftoff sound
					audioSource1.Play ();
				}

				var a = black.GetComponent<SpriteRenderer> ().color;
				a = new Color (1f, 1f, 1f, alphaSet);
				black.GetComponent<SpriteRenderer> ().color = a;
				
				//important!! change the position of the distance object here
				GameObject d = GameObject.Find ("Canvas/Distance");
				//d.transform.position = new Vector3(621, 334, 0);
				d.GetComponent<Text> ().text = distance.ToString ();

				maxDistance = script.calculateDistance ();
				if (distance < maxDistance) {
					distance += 1;
					if (launched == true) {
						alphaSet += .001f;
						
					}

					if (GetSeconds (remainingTime2) != remainingTimeSec2) {
						remainingTimeSec2 = GetSeconds (remainingTime2);
						SetCountdownTimerText ("00:00");
					}
				} else {
					distance = maxDistance;

					// stop the liftoff sound
					audioSource1.Stop ();

					// set the distance value for this trial
					if (doOnce == false) {
						if (d1 == -1) {
							d1 = maxDistance;
						} else if (d2 == -1) {
							d2 = maxDistance;
						} else if (d3 == -1) {
							d3 = maxDistance;
						} else if (d4 == -1) {
							d4 = maxDistance;
						} else {
							d5 = maxDistance;
						}
						doOnce = true;
					}

					var results = GameObject.Find ("Results");
					results.GetComponent<Animator> ().SetTrigger ("go");

					GameObject jet4 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 1/Jet");
					jet4.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet5 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 2/Jet 1");
					jet5.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet6 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 3/Jet 2");
					jet6.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet7 = GameObject.Find ("RocketSprites/SelectedOutlines/BoosterSelectedOutlines/engine_selected_outline 4/Jet 3");
					jet7.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet8 = GameObject.Find ("RocketSprites/SelectedOutlines/FinSelectedOutlines/left_fin_selected_outline/Jet");
					jet8.GetComponent<ParticleSystem> ().enableEmission = false;
					GameObject jet9 = GameObject.Find ("RocketSprites/SelectedOutlines/FinSelectedOutlines/right_fin_selected_outline/Jet");
					jet9.GetComponent<ParticleSystem> ().enableEmission = false;

					if (d1 != -1) 
						GameObject.Find ("Canvas/Trial1").transform.position = results.transform.position - new Vector3 (-37, -16, 0);
					if (d2 != -1) 
						GameObject.Find ("Canvas/Trial2").transform.position = results.transform.position - new Vector3 (-37, -7, 0);
					if (d3 != -1) 
						GameObject.Find ("Canvas/Trial3").transform.position = results.transform.position - new Vector3 (-37, 2, 0);
					if (d4 != -1) 
						GameObject.Find ("Canvas/Trial4").transform.position = results.transform.position - new Vector3 (-37, 11, 0);
					if (d5 != -1) 
						GameObject.Find ("Canvas/Trial5").transform.position = results.transform.position - new Vector3 (-37, 20, 0);

					// Only update the text displays and saved variables (and send a Thalamus message) when the results values change
					if (d1 != d1_old || d2 != d2_old || d3 != d3_old || d4 != d4_old || d5 != d5_old) {
						if (d1 != d1_old) {
							t1.text = "Trial 1: " + d1.ToString ();
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d1 = d1;
							d1_old = d1;
						}
						if (d2 != d2_old) {
							t2.text = "Trial 2: " + d2.ToString ();
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d2 = d2;
							d2_old = d2;
						}
						if (d3 != d3_old) {
							t3.text = "Trial 3: " + d3.ToString ();
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d3 = d3;
							d3_old = d3;
						}
						if (d4 != d4_old) {
							t4.text = "Trial 4: " + d4.ToString ();
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d4 = d4;
							d4_old = d4;
						}
						if (d5 != d5_old) {
							t5.text = "Trial 5: " + d5.ToString ();
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().d5 = d5;
							d5_old = d5;
						}

						GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().gameStarted = gameStarted;

						// ETHAN
						// send the results to Thalamus
						//thalamusUnity.Publisher.SentFromUnityToThalamus ("results*" + d1.ToString() + "*" + d2.ToString() + "*" + d3.ToString() + "*" + d4.ToString() + "*" + d5.ToString());

						//Debug.Log("results*" + d1.ToString() + "*" + d2.ToString() + "*" + d3.ToString() + "*" + d4.ToString() + "*" + d5.ToString());
					}


					// Put the game over panel once the results panel has come out
					AnimatorStateInfo currentResultsPanelState = resultsAnimator.GetCurrentAnimatorStateInfo (0);
					int trialNum = GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().trialNumber;

					if (trialNum >= totalTrialsNumber &&
					    currentResultsPanelState.IsName ("Base Layer.stay"))
					{
//						GameObject.Find ("Results").GetComponent<MeshRenderer> ().enabled = false;
//						GameObject.Find ("Canvas/Trial1").GetComponent<Text> ().enabled = false;
//						GameObject.Find ("Canvas/Trial2").GetComponent<Text> ().enabled = false;
//						GameObject.Find ("Canvas/Trial3").GetComponent<Text> ().enabled = false;
//						GameObject.Find ("Canvas/Trial4").GetComponent<Text> ().enabled = false;
//						GameObject.Find ("Canvas/Trial5").GetComponent<Text> ().enabled = false;
						GameObject.Find ("Canvas/Panel").GetComponent<Image> ().enabled = true;
						GameObject.Find ("Canvas/GameOverText").GetComponent<Text> ().enabled = true;
					}

					GameObject timer = GameObject.Find ("Canvas/Timer");
					timer.transform.position = timerSavePosition;
					remainingTime2 = secondInitialValue - timeElapsed2;
					if (remainingTime2 > 0.0) {
						if (GetSeconds (remainingTime2) != remainingTimeSec2) {
							remainingTimeSec2 = GetSeconds (remainingTime2);
							SetCountdownTimerText (FormatTime2 (remainingTimeSec2));
						}
					} else {
						if (GetSeconds (remainingTime2) != remainingTimeSec2) {
							remainingTimeSec2 = GetSeconds (remainingTime2);
							SetCountdownTimerText ("00:00");

							// hide the distance stats
							GameObject distanceDisplay = GameObject.Find ("Canvas/Distance");
							distanceDisplay.GetComponent<Text> ().enabled = false;
							
							GameObject miles = GameObject.Find ("Canvas/Meters");
							miles.GetComponent<Text> ().enabled = false;
						}

						// increase the trail number & enable/disable restart
						if (trialNum < totalTrialsNumber)
						{
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().trialNumber++;
							Debug.Log ("Trial number: " + GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().trialNumber.ToString ());

							canRestart = true;
						}
					
					}
					timeElapsed2 += Time.deltaTime;

				}


				Camera.main.orthographicSize = fov;
				//fov += .05f;

			}

			// increment the time elapsed
			timeElapsed += Time.deltaTime;

		}
	}

	public void StartGame() {
		gameStarted = true;

		HideUIElements ();

		// relational
		if (GameObject.Find ("Canvas/Toggle").GetComponent<Toggle> ().isOn) {
			// ETHAN
			// send the timer value to Thalamus
			//thalamusUnity.Publisher.SentFromUnityToThalamus ("relational");
			
			//Debug.Log ("relational");
		} 
		// test
		else if (GameObject.Find ("Canvas/Toggle (1)").GetComponent<Toggle> ().isOn) {
			// ETHAN
			// send the timer value to Thalamus
			//thalamusUnity.Publisher.SentFromUnityToThalamus ("task");
			
			//Debug.Log ("task");
		} 
		// control
		else if (GameObject.Find ("Canvas/Toggle (2)").GetComponent<Toggle> ().isOn) {
			// ETHAN
			// send the timer value to Thalamus
			//thalamusUnity.Publisher.SentFromUnityToThalamus ("control");
			
			//Debug.Log ("control");
		}
	}

	void HideUIElements() {
		GameObject.Find ("Canvas/Panel").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Button").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Button/Text").GetComponent<Text> ().enabled = false;
		GameObject.Find ("Canvas/Toggle/Background").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Toggle/Background/Checkmark").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Toggle/Label").GetComponent<Text> ().enabled = false;
		GameObject.Find ("Canvas/Toggle (1)/Background").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Toggle (1)/Background/Checkmark").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Toggle (1)/Label").GetComponent<Text> ().enabled = false;
		GameObject.Find ("Canvas/Toggle (2)/Background").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Toggle (2)/Background/Checkmark").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Canvas/Toggle (2)/Label").GetComponent<Text> ().enabled = false;
		GameObject.Find ("Canvas/GameOverText").GetComponent<Text> ().enabled = false;
	}

	void SetCountdownTimerText(string timerText)
	{
		countdownTimer.text = timerText;

		// ETHAN
		// send the timer value to Thalamus
		 //thalamusUnity.Publisher.SentFromUnityToThalamus ("timer*" + timerText);

		 //Debug.Log ("timer*" + timerText);
	}

	// restarts the game for each trial
	void RestartGame() {
		// reset the timer
		timeElapsed = 0;
	}

	int GetSeconds(float value) {
		TimeSpan t = TimeSpan.FromSeconds (value);
		return (t.Minutes * 60 + t.Seconds);
	}

	string FormatTime(float value) {
		TimeSpan t = TimeSpan.FromSeconds (value);
		return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
	}

	string FormatTime2(int value) 
	{
		int min, sec;
		min = value / 60;
		sec = value % 60;
		return string.Format("{0:D2}:{1:D2}", min, sec);
	}
}

// testing merging code
