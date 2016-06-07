using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


//this is a test comment
//this is also a test comment

public class GameManager : MonoBehaviour {

	public bool sendThalamusMsgs = false;

	public Text countdownTimer;
	public float initialTimerValue = 120f; // time in seconds
	public float secondInitialValue = 30f;

	// Values dependent on the number of trials
	public int totalTrialsNumber = 7;
	private List<int> distanceVals = new List<int> () {-1, -1, -1, -1, -1, -1, -1};
	private List<int> oldDistanceVals = new List<int> () {-1, -1, -1, -1, -1, -1, -1};


	// Everything on the canvas that we'll be manipulating (lots of things) 
	public GameObject weight;
	public GameObject airResistance;
	public GameObject power;
	public GameObject fuel;

	[SerializeField] Text gameOverText;
	[SerializeField] Image overlayPanel;
	[SerializeField] GameObject resultsPanel;
	[SerializeField] List<Text> trialResultsTexts;

	[SerializeField] Text distanceText;
	[SerializeField] Text milesText;

	[SerializeField] GameObject startButton;
	[SerializeField] GameObject toggleR;
	[SerializeField] GameObject toggleT;
	[SerializeField] GameObject toggleC;
	

	// sounds being used
	public AudioClip countdownBeep;
	public AudioClip liftoffSound;

	// if the game state bools
	public bool gameStarted = false;
	public bool paused = false;

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

	private bool doOnce;

	private Vector3 timerSavePosition;

	// audio source for sound effects
	private AudioSource audioSource1;
	private AudioSource audioSource2;

	// animator for the results panel
	private Animator resultsAnimator;

	// the object used to send all the messages to Thalamus
	private ThalamusUnity thalamusUnity;

	void Awake () {
		var saved = GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ();

		for (int i = 0; i < oldDistanceVals.Count; i++) {
			distanceVals[i] = saved.distanceVals[i];
		}

		gameStarted = saved.gameStarted;
		// gameStarted = true; // DEMO CHANGE

		if (gameStarted) {
			HideUIElements ();
		} else {
			gameOverText.enabled = false;
		}

		// show the question mark
		GameObject questionMark = GameObject.Find("QuestionArea");
		questionMark.GetComponent<SpriteRenderer> ().enabled = true;

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
		distanceText.enabled = false;
		milesText.enabled = false;

		fov =  Camera.main.orthographicSize;
		doOnce = false;
		
		// initialize the thalamusUnity object
		// ETHAN
		if (sendThalamusMsgs) {
			thalamusUnity = new ThalamusUnity ();
		}

		// initialize the audio sources
		var audioSources = GetComponents<AudioSource> ();
		audioSource1 = audioSources [0];
		//audioSource2 = audioSources [1];
	}
	
	void Start ()
	{
		resultsAnimator = resultsPanel.GetComponent<Animator> ();
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
					SetCountdownTimerText (remainingTimeSec);
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
					distanceText.enabled = true;
					
					// hide the question mark
					GameObject questionMark = GameObject.Find ("QuestionArea");
					questionMark.GetComponent<SpriteRenderer> ().enabled = false;

					// show the miles
					milesText.enabled = true;

					// hide the timer
					GameObject timer = GameObject.Find ("Canvas/Timer");
					timerSavePosition = timer.transform.position;
					timer.GetComponent<Text> ().enabled = false;
					//timer.transform.position = new Vector3(-1000, -1000, 0);


					weight.transform.position = new Vector3 (-1000, -1000, 0);
					airResistance.transform.position = new Vector3 (-1000, -1000, 0);
					fuel.transform.position = new Vector3 (-1000, -1000, 0);
					power.transform.position = new Vector3 (-1000, -1000, 0);


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

				// update the distance text to show how far the rocket has gone
				distanceText.text = distance.ToString ();

				maxDistance = script.calculateDistance ();
				if (distance < maxDistance) {
					distance += 1;
					if (launched == true) {
						alphaSet += .001f;
						
					}

					if (GetSeconds (remainingTime2) != remainingTimeSec2) {
						remainingTimeSec2 = GetSeconds (remainingTime2);
						SetCountdownTimerText (0);
					}
				} else {
					distance = maxDistance;

					// stop the liftoff sound
					audioSource1.Stop ();

					// set the distance value for this trial
					if (doOnce == false) {
						for (int i = 0; i < distanceVals.Count; i++)
						{
							if (distanceVals[i] == -1)
							{
								distanceVals[i] = maxDistance;
								break;
							}
						}
						doOnce = true;
					}
				
					resultsPanel.GetComponent<Animator> ().SetTrigger ("go");

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

					int y_val;
					for (int i = 0; i < distanceVals.Count; i++)
					{
						if (distanceVals[i] != -1)
						{
							y_val = -16 + 7 * i;
							GameObject.Find ("Canvas/Trial" + (i+1).ToString()).transform.position = resultsPanel.transform.position - new Vector3 (-36, y_val, 0);
						}
					}


					int trialChanged = -1; // note trials in this case will start at 0
					for (int i = 0; i < distanceVals.Count; i++)
					{
						if (distanceVals[i] != oldDistanceVals[i])
						{
							trialChanged = i;
							break;
						}
					}
					
					// Only update the text displays and saved variables (and send a Thalamus message) when the results values change
					if (trialChanged != -1)
					{
						trialResultsTexts[trialChanged].text = "Trial " + (trialChanged + 1).ToString() + ":   " + distanceVals[trialChanged].ToString();
						GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().distanceVals[trialChanged] = distanceVals[trialChanged];
						oldDistanceVals[trialChanged] = distanceVals[trialChanged];
						GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().gameStarted = gameStarted;
						
						// ETHAN
						// send the results to Thalamus
						string resultsString = "results*";
						for (int i = 0; i < distanceVals.Count; i++)
						{
							resultsString = resultsString + distanceVals[i].ToString();
							if (i != (distanceVals.Count - 1))
							{
								resultsString = resultsString + "*";
							}
						}

						if (sendThalamusMsgs) {
							thalamusUnity.Publisher.SentFromUnityToThalamus (resultsString);
						}

						// Debug.Log (resultsString);
					}


					// Put the game over panel once the results panel has come out
					AnimatorStateInfo currentResultsPanelState = resultsAnimator.GetCurrentAnimatorStateInfo (0);
					int trialNum = GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().trialNumber;

					if (currentResultsPanelState.IsName ("Base Layer.stay"))
					{
						paused = true;

						if (trialNum >= totalTrialsNumber)
						{
	//						GameObject.Find ("Results").GetComponent<MeshRenderer> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial1").GetComponent<Text> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial2").GetComponent<Text> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial3").GetComponent<Text> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial4").GetComponent<Text> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial5").GetComponent<Text> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial6").GetComponent<Text> ().enabled = false;
	//						GameObject.Find ("Canvas/Trial7").GetComponent<Text> ().enabled = false;
							overlayPanel.enabled = true;
							gameOverText.enabled = true;
						}
					}

					GameObject timer = GameObject.Find ("Canvas/Timer");
					timer.transform.position = timerSavePosition;
					remainingTime2 = secondInitialValue - timeElapsed2;
					if (remainingTime2 > 0.0) {
						if (GetSeconds (remainingTime2) != remainingTimeSec2) {
							remainingTimeSec2 = GetSeconds (remainingTime2);
							SetCountdownTimerText (remainingTimeSec2);
						}
					} else {
						if (GetSeconds (remainingTime2) != remainingTimeSec2) {
							remainingTimeSec2 = GetSeconds (remainingTime2);
							SetCountdownTimerText (0);

							// hide the distance stats
							distanceText.enabled = false;
							milesText.enabled = false;
						}

						// increase the trail number & enable/disable restart
						if (trialNum < totalTrialsNumber)
						{
							GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().trialNumber++;
							Debug.Log ("Trial number: " + GameObject.Find ("SavedVariables").GetComponent<SavedVariables> ().trialNumber.ToString ());

							canRestart = true;

							paused = false;
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
		if (toggleR.GetComponent<Toggle> ().isOn) {
			// send the timer value to Thalamus
			if (sendThalamusMsgs) {
				thalamusUnity.Publisher.SentFromUnityToThalamus ("relational");
			}
		} 
		// task
		else if (toggleT.GetComponent<Toggle> ().isOn) {
			// send the timer value to Thalamus
			if (sendThalamusMsgs) {
				thalamusUnity.Publisher.SentFromUnityToThalamus ("task");
			}
		} 
		// control
		else if (toggleC.GetComponent<Toggle> ().isOn) {
			// send the timer value to Thalamus
			if (sendThalamusMsgs) {
				thalamusUnity.Publisher.SentFromUnityToThalamus ("control");
			}
		}
	}

	void HideUIElements() {
		overlayPanel.enabled = false;
		startButton.SetActive (false);
		toggleR.SetActive (false);
		toggleT.SetActive (false);
		toggleC.SetActive (false);
		gameOverText.enabled = false;
	}

	void SetCountdownTimerText(int timerSec)
	{
		string timerText = FormatTime2 (timerSec);
		countdownTimer.text = timerText;

		if (timerSec % 5 == 0) {
			// ETHAN
			// send the timer value to Thalamus
			if (sendThalamusMsgs) {
				thalamusUnity.Publisher.SentFromUnityToThalamus ("timer*" + timerText);
			}
			
			//Debug.Log ("timer*" + timerText);
		}
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
