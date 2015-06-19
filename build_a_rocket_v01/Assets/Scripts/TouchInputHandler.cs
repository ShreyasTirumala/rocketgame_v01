using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TouchInputHandler : MonoBehaviour {

	// all of the prefabs that we can touch/drag
	public GameObject[] conePieces;
	public GameObject[] finPieces;
	public GameObject[] bodyPieces;
	public GameObject[] boosterPieces;
	public GameObject[] outlinePieces;
	public GameObject[] selectedOutlinePieces;
	
	public GameObject[] trashcans;

	//lists for prefabs, not sure if arrays are still necessary but will leave them for now
	public List<GameObject> bodyPieceList = new List<GameObject> ();
	public List<GameObject> finPieceList = new List<GameObject> ();
	public List<GameObject> boosterPieceList = new List<GameObject> ();
	public List<GameObject> conePieceList = new List<GameObject> ();
	public List<GameObject> rocketPieces = new List<GameObject> ();

	public GameObject savedBodyPiece = null;

	public int resistance = 0;
	public int power = 0;
	public int fuel = 0;
	public int weight = 0;
	
	// flags to be able to use with either mouse or touch or both
	public bool usingMouse = true;
	public bool usingTouch = true;
	
	public int pixelsPerUnit = 10;
	
	// states of the build phase 
	private int currentState;
	private int nextState;
	private bool switching = false;
	private int switchDelay = 0;
	private const int nothingSelected = 0;
	private const int coneSelected = 1;
	private const int finSelected = 2;
	private const int bodySelected = 3;
	private const int boosterSelected = 4;
	
	// variables indicating whether a piece is a rocketPiece, pieceOutline, pieceSelectedOutline, or trashcan
	private const int rocketPiece = 0;
	private const int outlinePiece = 1;
	private const int selectedOutlinePiece = 2;
	private const int trashcan = 3;

	// variables indicating outline peiece identity
	private const int coneOutlinePiece = 0;
	private const int finOutlinePiece = 1;
	private const int bodyOutlinePiece = 2;
	private const int boosterOutlinePiece = 3;
	
	// camera settings
	private float cameraHeight;
	private float cameraWidth;
	
	// animator and related variables
	private Animator leftPanelAnimator;
	private Animator rightPanelAnimator;
	private bool firstStateChangeOccured = false;

	private bool ending = false;

	public GameObject[] savedPiece = new GameObject[10];
	
	// Use this for initialization
	void Start () {
		// set the state to nothingSelected
		currentState = nothingSelected;

		// get the animators
		GameObject leftPiecePanel = GameObject.Find ("LeftPiecePanel");
		if (leftPiecePanel != null) {
			leftPanelAnimator = leftPiecePanel.GetComponent<Animator> ();
		}
		GameObject rightPiecePanel = GameObject.Find ("RightPiecePanel");
		if (rightPiecePanel != null) {
			rightPanelAnimator = rightPiecePanel.GetComponent<Animator>(); 
		}
		
		// obtain the width and height of the camera
		Camera cam = Camera.main;
		cameraHeight = 2f * cam.orthographicSize;
		cameraWidth = Mathf.Round (cameraHeight * cam.aspect);
		
		//Initialize our lists:
		foreach (GameObject piece in conePieces) {
			conePieceList.Add (piece);
		}
		foreach (GameObject piece in finPieces) {
			finPieceList.Add (piece);
		}
		foreach (GameObject piece in boosterPieces) {
			boosterPieceList.Add (piece);
		}
		foreach (GameObject piece in bodyPieces) {
			bodyPieceList.Add (piece);
		}

		// hide all of the pieces to start out
		hidePieceList (conePieceList);
		hidePieceList (finPieceList);
		hidePieceList (bodyPieceList);
		hidePieceList (boosterPieceList);
		
		// hide all of the selected outlines to start out
		hidePieces (selectedOutlinePieces);

	}
	
	// Update is called once per frame
	void Update () {

		GameObject selectedBodyPiece = null;
		//count down the delay
		if (switchDelay > 0) {
			switchDelay -= 1;
		} else {
			switchDelay = 0;
		} 
		// for the mouse inputs
		if (usingMouse) {
			// if the left mouse button is clicking on our object
			if(Input.GetMouseButton(0)) {
				//if you click on the results object, reload the level
				if (ending) { 
					//if ( Mathf.Abs(Input.mousePosition.x) < GameObject.Find ("Results").GetComponent<MeshRenderer>().bounds.size.x/2) {
					//	if ( Mathf.Abs(Input.mousePosition.y) < GameObject.Find ("Results").GetComponent<MeshRenderer>().bounds.size.y/2) {
					Debug.Log ("reload");       
					Application.LoadLevel(Application.loadedLevel);
							
					//	}
				//	}
				}

				if (switching == false ) {
					selectedBodyPiece = MouseOverPiece(Input.mousePosition, rocketPiece);
					//lockListException(conePieceList, selectedBodyPiece);
					//lockListException(finPieceList, selectedBodyPiece);
					//lockListException(boosterPieceList, selectedBodyPiece);
					//lockListException(bodyPieceList, selectedBodyPiece);
				} else {
					selectedBodyPiece = null;
				}
				if (selectedBodyPiece != savedBodyPiece && savedBodyPiece != null) {
					selectedBodyPiece = null;
				}
				//find the closest new lock position
				newLock(selectedBodyPiece);

				//save it globally so we can operate on it in the next step if we want
				savedBodyPiece = selectedBodyPiece;
				//only select outline under certain conditions
				GameObject selectedOutlinePiece;
				if (selectedBodyPiece == null && switching == false /*&& switchDelay == 0*/) {
					selectedOutlinePiece = MouseOverPiece(Input.mousePosition, outlinePiece);
				} else {
					selectedOutlinePiece = null;
				}
				
				if (selectedBodyPiece != null) {
					Vector3 mousePos = Input.mousePosition;
					
					if (selectedBodyPiece.GetComponent<ObjectInfo>().firstTouch == false) {
						selectedBodyPiece.GetComponent<ObjectInfo>().firstTouch = true;
					}
					
					Vector3 piecePosition = new Vector3((mousePos.x * cameraWidth / Screen.width) - (cameraWidth / 2), 
					                                    (mousePos.y * cameraHeight / Screen.height) - (cameraHeight / 2), 
					                                    0);
					
					selectedBodyPiece.transform.position = piecePosition;
					
				} else if (selectedOutlinePiece != null) {
					string pieceName = selectedOutlinePiece.name;
					nextState = -1;
					if (pieceName.Contains("top")) {
						nextState = coneSelected;
					} else if (pieceName.Contains("right") || pieceName.Contains("left")) {
						nextState = finSelected;
					} else if (pieceName.Contains("body")) {
						nextState = bodySelected;
					} else if (pieceName.Contains("engine")) {
						nextState = boosterSelected;
					}
					lockAll ();
					// set the next state if not the current state
					if (nextState >= 0 && nextState != currentState) {
						switching = true;
						if (firstStateChangeOccured == true) {
							// play the animations to hide the sidebars
							leftPanelAnimator.SetTrigger ("stateChangeTriggerLeft");
							rightPanelAnimator.SetTrigger ("stateChangeTriggerRight");
						} else {
							leftPanelAnimator.SetBool ("firstStateSelectedLeft", true);
							rightPanelAnimator.SetBool ("firstStateSelectedRight", true);
						}
					}
				}
				
			} else {
				if (!switching) {
					lockAll();
					//the step after we stop selecting a piece, add that piece to the rocketPieces list
					addToRocketPieces(savedBodyPiece);
					checkTrash(savedBodyPiece);
					savedBodyPiece = null;
				}
			}
		}

		// for use of touch input only
		else if (usingTouch) {
			// if the left mouse button is clicking on our object
			if(Input.touchCount > 0) {
				//if you click on the results object, reload the level
				if (ending) { 
					//if ( Mathf.Abs(Input.mousePosition.x) < GameObject.Find ("Results").GetComponent<MeshRenderer>().bounds.size.x/2) {
					//	if ( Mathf.Abs(Input.mousePosition.y) < GameObject.Find ("Results").GetComponent<MeshRenderer>().bounds.size.y/2) {
					Debug.Log ("reload");       
					Application.LoadLevel(Application.loadedLevel);
					
					//	}
					//	}
				}
				//get our touch positions
				Vector3[] touchPosition;
				touchPosition = new Vector3[Input.touchCount];
				
				int i = 0;
				foreach (Touch touch in Input.touches) {
					touchPosition[i] = new Vector3 (0,0,0);
					touchPosition[i] = touch.position;
					i++;
				}
				
				GameObject[] selectedPiece;
				selectedPiece = new GameObject[Input.touchCount];
				
				if (switching == false ) {
					i = 0;
					foreach (Vector3 touch in touchPosition) {
						selectedPiece[i] = MouseOverPiece(touch, rocketPiece);
						//find the closest new lock position
						newLock(selectedPiece[i]);
						

						
						/*if (selectedPiece[i] != savedPiece[i] && savedPiece[i] != null) {
							selectedPiece[i] = null;
						} */
						i++;
					}
				} else {
					i = 0;
					foreach (GameObject piece in selectedPiece) {
						selectedPiece[i] = null;
						i++;
					}
				}
				
				
				
				//only select outline under certain conditions
				GameObject selectedOutlinePiece;
				if (selectedPiece[0] == null && switching == false /*&& switchDelay == 0*/) {
					selectedOutlinePiece = MouseOverPiece(touchPosition[0], outlinePiece); //FIX THIS, IT ONLY RECORDS THE FIRST TOUCH
				} else {
					selectedOutlinePiece = null;
				}
				
				bool allnull = true;
				foreach (GameObject selected in selectedPiece) {
					if (selected != null) {
						allnull = false;
					}
				}
				if (!allnull) {
					i = 0;
					foreach (GameObject selected in selectedPiece) {
						if (selected != null) {
							Vector3 mousePos = touchPosition[i];
							
							if (selected.GetComponent<ObjectInfo>().firstTouch == false) {
								selected.GetComponent<ObjectInfo>().firstTouch = true;
							}
							
							Vector3 piecePosition = new Vector3((mousePos.x * cameraWidth / Screen.width) - (cameraWidth / 2), 
							                                    (mousePos.y * cameraHeight / Screen.height) - (cameraHeight / 2), 
							                                    0);
							
							selected.transform.position = piecePosition;
						}
						i++;
					}
					
				} else if (selectedOutlinePiece != null) {
					string pieceName = selectedOutlinePiece.name;
					nextState = -1;
					if (pieceName.Contains("top")) {
						nextState = coneSelected;
					} else if (pieceName.Contains("right") || pieceName.Contains("left")) {
						nextState = finSelected;
					} else if (pieceName.Contains("body")) {
						nextState = bodySelected;
					} else if (pieceName.Contains("engine")) {
						nextState = boosterSelected;
					}
					lockAll ();
					// set the next state if not the current state
					if (nextState >= 0 && nextState != currentState) {
						switching = true;
						if (firstStateChangeOccured == true) {
							// play the animations to hide the sidebars
							leftPanelAnimator.SetTrigger ("stateChangeTriggerLeft");
							rightPanelAnimator.SetTrigger ("stateChangeTriggerRight");
						} else {
							leftPanelAnimator.SetBool ("firstStateSelectedLeft", true);
							rightPanelAnimator.SetBool ("firstStateSelectedRight", true);
						}
					}
				}
				int k = 0;
				for (k = 0; k<10; k++) {
					bool isselected = false;
					foreach (GameObject selected in selectedPiece) {
						if (selected == savedPiece[k]) {
							isselected = true;
						}
					}
					if (!isselected) {
						checkTrash(savedPiece[k]);
					}
				}
				if (!switching) {
					
					int q;
					for (q = 0; q<10; q++) {
						bool savednotselected = true;
						foreach(GameObject selected in selectedPiece) {
							if (savedPiece[q] == selected) {
								savednotselected = false;
							}
						}
						if (savednotselected) {
							addToRocketPieces(savedPiece[q]);
							
						}
						//savedPiece[q] = null;
					
					int m = 0;
					foreach (GameObject piece in selectedPiece) {
						//save it globally so we can operate on it in the next step if we want
						if (savedPiece[m] != selectedPiece[m]) {
							//addToRocketPieces(savedPiece[m]);
						}
						savedPiece[m] = selectedPiece[m];
						m++;
					}
					lockListExceptionArray(selectedPiece, conePieceList);
					lockListExceptionArray(selectedPiece, boosterPieceList);
					lockListExceptionArray(selectedPiece, bodyPieceList);
					lockListExceptionArray(selectedPiece, finPieceList);
					
					}
				}
				
				
			} else {
				if (!switching) {
					lockAll();
					//the step after we stop selecting a piece, add that piece to the rocketPieces list
					int i = 0;
					for (i = 0; i<10; i++) {
						addToRocketPieces(savedPiece[i]);
						checkTrash(savedPiece[i]);
						savedPiece[i] = null;
					}
				}
			}

		}
		//searchToAdd ();
		
		//get the current state
		AnimatorStateInfo currentRightPanelBaseState = rightPanelAnimator.GetCurrentAnimatorStateInfo (0);
		AnimatorStateInfo currentLeftPanelBaseState = leftPanelAnimator.GetCurrentAnimatorStateInfo (0);
		
		if (switching && currentRightPanelBaseState.IsName ("Base Layer.RightPanelIn") && currentLeftPanelBaseState.IsName ("Base Layer.LeftPanelIn")) {
			if (firstStateChangeOccured == true) {
				// hide the old pieces + the old selected outline
				if (currentState == coneSelected) {
					hideOutlinePieces(coneOutlinePiece, true);
					showOutlinePieces(coneOutlinePiece, false);
					hidePieceList (conePieceList);
				} else if (currentState == finSelected) {
					hideOutlinePieces(finOutlinePiece, true);
					showOutlinePieces(finOutlinePiece, false);
					hidePieceList (finPieceList);
				} else if (currentState == bodySelected) {
					hideOutlinePieces(bodyOutlinePiece, true);
					showOutlinePieces(bodyOutlinePiece, false);
					hidePieceList (bodyPieceList);
				} else if (currentState == boosterSelected) {
					hideOutlinePieces(boosterOutlinePiece, true);
					showOutlinePieces(boosterOutlinePiece, false);
					hidePieceList (boosterPieceList);
				}
			} else {
				firstStateChangeOccured = true;
			}
			
			// show the new pieces
			if (nextState == coneSelected) {
				hideOutlinePieces(coneOutlinePiece, false);
				showOutlinePieces(coneOutlinePiece, true);
				showPieceList (conePieceList);
			} else if (nextState == finSelected) {
				hideOutlinePieces(finOutlinePiece, false);
				showOutlinePieces(finOutlinePiece, true);
				showPieceList (finPieceList);
			} else if (nextState == bodySelected) {
				hideOutlinePieces(bodyOutlinePiece, false);
				showOutlinePieces(bodyOutlinePiece, true);
				showPieceList (bodyPieceList);
			} else if (nextState == boosterSelected) {
				hideOutlinePieces(boosterOutlinePiece, false);
				showOutlinePieces(boosterOutlinePiece, true);
				showPieceList (boosterPieceList);
			}
			
			// set the global variable to indicate the currentState
			currentState = nextState;
			switching = false;
			//I fixed a bunch of bugs with this... its hacky but it works
			switchDelay = 60;
			firstTouchReset();
		}

		//calculation of values
		resistance = 0;
		power = 0;
		fuel = 0;
		weight = 0;
		//update all of the values of the rocket
		foreach (GameObject piece in rocketPieces) {
			weight += piece.GetComponent<ObjectInfo> ().weight;
			resistance += piece.GetComponent<ObjectInfo> ().airResistance;
			fuel += piece.GetComponent<ObjectInfo> ().fuel;
			power += piece.GetComponent<ObjectInfo> ().power;
		}
		GameObject controller = GameObject.Find ("GameManager");
		var controlScript = controller.GetComponent<GameManager> ();
		controlScript.weight.text = weight.ToString ();
		controlScript.airResistance.text = resistance.ToString();
		controlScript.fuel.text = fuel.ToString();
		controlScript.power.text = power.ToString();

	}

	void hideOutlinePieces (int pieceType, bool isHighlightedOutline) {
		
		GameObject[] pieces;
		if (isHighlightedOutline) {
			pieces = selectedOutlinePieces;
		} else {
			pieces = outlinePieces;
		}
		
		foreach (GameObject piece in pieces) {
			string pieceName = piece.name;
			if (pieceName.Contains("top") && pieceType == coneOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = false; 
			} else if (pieceName.Contains("fin") && pieceType == finOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = false; 
			} else if (pieceName.Contains("body") && pieceType == bodyOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = false; 
			} else if (pieceName.Contains("engine") && pieceType == boosterOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = false; 
			}
		}
	}


	
	//hide the pieces, but not if they are locked into a rocket
	void hidePieces (GameObject[] pieces) {
		if (pieces != boosterPieces && pieces != conePieces && pieces != bodyPieces && pieces != finPieces) {
			foreach (GameObject piece in pieces) {
				piece.GetComponent<SpriteRenderer> ().enabled = false; 
			} 
		}else {
			foreach (GameObject piece in pieces) {
				if (piece.GetComponent<ObjectInfo>().seeMe == false) {
					piece.GetComponent<SpriteRenderer> ().enabled = false; 
				}
			}
		}
	}
	//hide each piece in a pieceList
	void hidePieceList (List<GameObject> pieces) {
		foreach (GameObject piece in pieces) {
			if (piece.GetComponent<ObjectInfo>().seeMe == false) {
				piece.GetComponent<SpriteRenderer> ().enabled = false; 
			}
		}
	}
	//manually hide a piece whether or not it is locked into rocket
	void hidePiece (GameObject piece) {
		piece.GetComponent<SpriteRenderer>().enabled = false; 
	}
	
	void showOutlinePieces (int pieceType, bool isHighlightedOutline) {
		
		GameObject[] pieces;
		if (isHighlightedOutline) {
			pieces = selectedOutlinePieces;
		} else {
			pieces = outlinePieces;
		}
		
		foreach (GameObject piece in pieces) {
			string pieceName = piece.name;
			if (pieceName.Contains("top") && pieceType == coneOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = true; 
			} else if ((pieceName.Contains("right") || pieceName.Contains("left")) && pieceType == finOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = true; 
			} else if (pieceName.Contains("body") && pieceType == bodyOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = true; 
			} else if (pieceName.Contains("engine") && pieceType == boosterOutlinePiece) {
				piece.GetComponent<SpriteRenderer> ().enabled = true; 
			}
		}
	}

	void showPieces (GameObject[] pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<SpriteRenderer>().enabled = true; 
			
		}
	}
	void showPieceList(List<GameObject> pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<SpriteRenderer>().enabled = true; 
		}
	}
	
	void showPiece (GameObject piece) {
		piece.GetComponent<SpriteRenderer>().enabled = true; 
	}
	
	//should set all non outline objects back to their lock positions. call this before setting dragged object positions
	void lockSet(List<GameObject> pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<ObjectInfo> ().reLock (); 
		}
	}
	void lockListException (List<GameObject> pieces, GameObject exception) {
		foreach (GameObject piece in pieces) {
			if (piece != exception) {
				piece.GetComponent<ObjectInfo> ().reLock ();
			}
		}
	}
	void lockListExceptionArray(GameObject[] array, List<GameObject> list) {
		foreach (GameObject piece in list) {
			bool notequal = true;
			foreach (GameObject except in array) {
				if (piece == except) {
					notequal = false;
				}
			}
			if (notequal)
				piece.GetComponent<ObjectInfo> ().reLock ();
		}
	}



	void lockAll() {
		lockSet (boosterPieceList);
		lockSet (conePieceList);
		lockSet (bodyPieceList);
		lockSet (finPieceList);
	}
	void firstTouch (List<GameObject> pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<ObjectInfo> ().firstTouch = false;
		}
	}
	
	void firstTouchReset() {
		firstTouch (boosterPieceList);
		firstTouch (finPieceList);
		firstTouch (bodyPieceList);
		firstTouch (conePieceList);
	}
	
	void newLock(GameObject piece) {
		if (piece != null) {
			float distance;
			float minDistance = Vector3.Distance (piece.transform.position, piece.GetComponent<ObjectInfo> ().lockPosition) / 3;
			float saveMinDistance = minDistance;
			Vector3 newLock = piece.GetComponent<ObjectInfo> ().lockPosition;
			Vector3 oldLock = newLock;
			bool trashpossible = false;
			if (rocketPieces.Contains(piece)) {
				trashpossible = true;
			}
		
			foreach (GameObject outline in selectedOutlinePieces) {
				if (outline.GetComponent<SpriteRenderer> ().enabled == true   ) {
					Debug.Log (outline.GetComponent<OutlineInfo>().objectLocked);
					if (outline.GetComponent<OutlineInfo>().objectLocked == false) 
				{
					distance = Vector3.Distance (outline.transform.position, piece.transform.position);
					if (minDistance > distance && !collisionLoop (outline.transform.position)) {
						minDistance = distance;
						newLock = outline.transform.position;
					}
					
				}
				
				
				}
			}
			if (trashpossible) {
				foreach (GameObject trash in trashcans) {
					distance = Vector3.Distance (trash.transform.position, piece.transform.position);
					if (4*minDistance > distance) {
						minDistance = distance;
						newLock = trash.transform.position;
					}
				}
			}

			if (minDistance != saveMinDistance) {
				piece.GetComponent<ObjectInfo> ().newLock (newLock);
			}


		}
	}

	public void addToRocketPieces(GameObject selectedBodyPiece) {
		//if the piece is locked into the body add it to the list of rocket pieces
		if (selectedBodyPiece != null) {
			var script = selectedBodyPiece.GetComponent<ObjectInfo> ();

			if (script.seeMe == true && selectedBodyPiece.transform.position == script.lockPosition &&
				script.lockPosition != script.initialLockPosition && script.added == false) {
				int count = 0;
				//first check if the piece is on a trashcan
				/*foreach (GameObject trashcan in trashcans) {
					if (Vector3.Distance(selectedBodyPiece.transform.position, trashcan.transform.position) < 10) {
						//it will be destroyed, dont add it, but do create a new object in its old spot
						count++;
					}
				}*/

				if (count == 0) {
					rocketPieces.Add (selectedBodyPiece);
					script.added = true;
				}

				Vector3 lockP = script.initialLockPosition;
				if (lockP.x <= 0) {
					//lockP.x +=20;
					lockP.x = -86;
					}else {
					//lockP.x -=20;
					lockP.x= 86;
				}

				GameObject newObject = null;
				if (currentState == coneSelected) {
				 newObject = GameObjectUtil.Instantiate(conePieces[10], lockP);
					conePieceList.Add (newObject);
				} else
				if (currentState == finSelected) {
					 newObject = GameObjectUtil.Instantiate(finPieces[10], lockP);
					finPieceList.Add (newObject);
				} else 
				if (currentState == bodySelected) {
					 newObject = GameObjectUtil.Instantiate(bodyPieces[10], lockP);
					bodyPieceList.Add (newObject);
				} else
				if (currentState == boosterSelected) {
					newObject = GameObjectUtil.Instantiate(boosterPieces[10], lockP);
					boosterPieceList.Add (newObject);
				} 
				if (newObject != null) {
					if (lockP.x <0) {
						newObject.transform.parent = GameObject.Find("LeftPiecePanel").transform;
					} else {
						newObject.transform.parent = GameObject.Find("RightPiecePanel").transform;
					}
				}
				var newscript = newObject.GetComponent<ObjectInfo>();
				newscript.seeMe = false;
				newscript.initialLockPosition = newObject.transform.position = lockP; //also questionable line of code
				newscript.jump = false;
				//lockAll ();
			}
		}
	}

	void searchToAdd() {
		search (conePieceList);
		search (boosterPieceList);
		search (bodyPieceList);
		search (finPieceList);
	}
	void search (List<GameObject> list) {
		foreach (GameObject piece in list) {
			if (piece.GetComponent<ObjectInfo> ().lockPosition == piece.transform.position &&
			    piece.GetComponent<ObjectInfo>().lockPosition == piece.GetComponent<ObjectInfo>().initialLockPosition) {
				addToRocketPieces (piece);
			}
		}
	}

	void checkTrash(GameObject selected) {
		foreach (GameObject selectedBodyPiece in rocketPieces) {
			if (selectedBodyPiece != null) {
				foreach (GameObject trashcan in trashcans) {
					if (Vector3.Distance (selectedBodyPiece.transform.position, trashcan.transform.position) < 10 ||
					    (Mathf.Abs(selectedBodyPiece.transform.position.x) > 70 && selectedBodyPiece.GetComponent<ObjectInfo>().lockPosition !=
					 selectedBodyPiece.GetComponent<ObjectInfo>().initialLockPosition) ) {
						removeFromRocketPieces (selectedBodyPiece);
				
						GameObject.Destroy (selectedBodyPiece);
					}
				}
			}
		}
	}
	void removeFromRocketPieces(GameObject selectedBodyPiece) {
		if (currentState == coneSelected) {
			conePieceList.Remove (selectedBodyPiece);
		} else
		if (currentState == finSelected) {
			finPieceList.Remove (selectedBodyPiece);
		} else 
		if (currentState == bodySelected) {
			bodyPieceList.Remove (selectedBodyPiece);
		} else
		if (currentState == boosterSelected) {
			boosterPieceList.Remove (selectedBodyPiece);
		} 
		rocketPieces.Remove (selectedBodyPiece);
		GameObjectUtil.Destroy (selectedBodyPiece);
	}

	
	/* Returns the GameObject of the piece that the mouse is over or null if the mouse isn't over a piece */
	GameObject MouseOverPiece(Vector3 mousePos, int pieceType) {
		// initialize outputs
		GameObject output = null;

		// check if the last selected piece is pieceType
		// if it is then see if my mouse is over that piece
		// if so return it
		if (usingMouse) {
			if (savedBodyPiece != null) {
				if (Contains (savedBodyPiece, mousePos)) {
					output = savedBodyPiece;
					return output;
				}
			}
		} else if (usingTouch) {
			foreach (GameObject saved in savedPiece) {
				if (saved != null) {
					if (Contains (saved, mousePos)) {
						output = saved;
						return output;
					}
				}
			}
		}

		GameObject[] pieces = null;
		if (pieceType == rocketPiece) {
			if (currentState == coneSelected) {
				pieces = conePieces;
			} else if (currentState == finSelected) {
				pieces = finPieces;
			} else if (currentState == bodySelected) {
				pieces = bodyPieces;
			} else if (currentState == boosterSelected) {
				pieces = boosterPieces;
			} else {
				return output;
			}
		} else if (pieceType == outlinePiece) {
			pieces = outlinePieces;
		} else if (pieceType == selectedOutlinePiece) {
			pieces = selectedOutlinePieces;
		} else if (pieceType == trashcan) {
			pieces = trashcans;
		}

		if (pieces != conePieces && pieces != finPieces && pieces != bodyPieces && pieces != boosterPieces) {
			// Detect whether the mouse position is within one of the pieces
			foreach (GameObject piece in pieces) {
				if (Contains (piece, mousePos)) {
					output = piece;
					break;
				}
			}
		} else {
			List<GameObject> pieceList = null;
			if (pieces == conePieces) {
				pieceList = conePieceList;
			}
			if (pieces == finPieces) {
				pieceList = finPieceList;
			}
			if (pieces == boosterPieces) {
				pieceList = boosterPieceList;
			}
			if (pieces == bodyPieces) {
				pieceList = bodyPieceList;
			}
			foreach (GameObject piece in pieceList) {
				if (Contains (piece, mousePos)) {
					output = piece;
					break;
				}
			}
			/*foreach (GameObject piece in rocketPieces) {
				if (Contains (piece, mousePos)) {
					output = piece;
					break;
				}
			}*/ //add in if we want to be able to move pieces that arent selected
		}

		return output;
	}
	
	/* Returns true if the touchItem location is within the container, which must have a SpriteRenderer */
	bool Contains (GameObject container, Vector3 touchItem) {
		// get the position, width, and height of the container GameObject
		Vector3 containerPosition = container.transform.position;
		float containerWidth = container.GetComponent<SpriteRenderer> ().bounds.size.x;
		float containerHeight = container.GetComponent<SpriteRenderer> ().bounds.size.y;
		
		// the mouse (0,0) is at the bottom left corner of the screen, not the middle like the container
		// so we need to have a scaled mouse position
		float touchItemScaledx = (touchItem.x * cameraWidth / Screen.width) - (cameraWidth / 2);
		float touchItemScaledy = (touchItem.y * cameraHeight / Screen.height) - (cameraHeight / 2);
		
		if (touchItemScaledx >= (containerPosition.x - containerWidth / 2) && touchItemScaledx <= (containerPosition.x + containerWidth / 2)
		    && touchItemScaledy >= (containerPosition.y - containerHeight / 2) && touchItemScaledy <= (containerPosition.y + containerHeight / 2)) {
			return true;
		} else {
			return false;
		}
	}


	
	//this checks for a collision at a point with all of the rocket objects
	bool collisionLoop(Vector3 pos) {
		foreach (GameObject obj in boosterPieceList) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		foreach (GameObject obj in conePieceList) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		foreach (GameObject obj in finPieceList) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		foreach (GameObject obj in bodyPieceList) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		return false;
	}

	//to be triggered by GameManager when the liftoff happens
	public void endSequence() {
		lockAll ();
		firstTouchReset ();
		hideOutlinePieces (coneOutlinePiece, true);
		hideOutlinePieces (bodyOutlinePiece, true);
		hideOutlinePieces (boosterOutlinePiece, true);
		hideOutlinePieces (finOutlinePiece, true);
		showOutlinePieces (coneOutlinePiece, false);
		showOutlinePieces (finOutlinePiece, false);
		showOutlinePieces (bodyOutlinePiece, false);
		showOutlinePieces (boosterOutlinePiece, false);
		leftPanelAnimator.SetTrigger ("stateChangeTriggerLeft");
		leftPanelAnimator.SetTrigger ("stateChangeTriggerTakeoff");
		rightPanelAnimator.SetTrigger ("stateChangeTriggerRight");
		rightPanelAnimator.SetTrigger ("stateChangeTriggerTakeoff");
		ending = true;

	}

	public int calculateDistance() {
		if ((power - resistance) > 0) {
			return (fuel * (power - resistance) / 5) - weight;
		} else {
			return 0;
		}
	}

}
