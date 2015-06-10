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
	
	public List<GameObject> rocketPieces = new List<GameObject> ();
	
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
	
	// camera settings
	private float cameraHeight;
	private float cameraWidth;
	
	// animator and related variables
	private Animator leftPanelAnimator;
	private Animator rightPanelAnimator;
	private bool firstStateChangeOccured = false;
	
	// Use this for initialization
	void Start () {
		// set the state to nothingSelected
		currentState = nothingSelected;
		
		// hide all of the pieces to start out
		hidePieces (conePieces);
		hidePieces (finPieces);
		hidePieces (bodyPieces);
		hidePieces (boosterPieces);
		
		// hide all of the selected outlines to start out
		hidePieces (selectedOutlinePieces);
		
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

				if (switching == false) {
					selectedBodyPiece = MouseOverPiece(Input.mousePosition, rocketPiece);
				} else {
					selectedBodyPiece = null;
				}
				//find the closest new lock position, add it to the list of pieces snapped to the rocket
				newLock(selectedBodyPiece);
				//add it to the rocketPiece set possibly
				addToRocketPieces(selectedBodyPiece);

				//only select outline under certain conditions
				GameObject selectedOutlinePiece;
				if (selectedBodyPiece == null && switching == false && switchDelay == 0) {
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
					} else if (pieceName.Contains("box")) {
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
				}
			}
		}
		
		//get the current state
		AnimatorStateInfo currentRightPanelBaseState = rightPanelAnimator.GetCurrentAnimatorStateInfo (0);
		AnimatorStateInfo currentLeftPanelBaseState = leftPanelAnimator.GetCurrentAnimatorStateInfo (0);
		
		if (switching && currentRightPanelBaseState.IsName ("Base Layer.RightPanelIn") && currentLeftPanelBaseState.IsName ("Base Layer.LeftPanelIn")) {
			if (firstStateChangeOccured == true) {
				// hide the old pieces + the old selected outline
				if (currentState == coneSelected) {
					hidePieces (conePieces);
				} else if (currentState == finSelected) {
					hidePieces (finPieces);
				} else if (currentState == bodySelected) {
					hidePieces (bodyPieces);
				} else if (currentState == boosterSelected) {
					hidePieces (boosterPieces);
				}
			} else {
				firstStateChangeOccured = true;
			}
			
			// show the new pieces
			if (nextState == coneSelected) {
				showPieces (conePieces);
			} else if (nextState == finSelected) {
				showPieces (finPieces);
			} else if (nextState == bodySelected) {
				showPieces (bodyPieces);
			} else if (nextState == boosterSelected) {
				showPieces (boosterPieces);
			}
			
			// set the global variable to indicate the currentState
			currentState = nextState;
			switching = false;
			//I fixed a bunch of bugs with this... its hacky but it works
			switchDelay = 60;
			firstTouchReset();
		}
		int resistance = 0;
		int power = 0;
		int fuel = 0;
		int weight = 0;
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
	//manually hide a piece whether or not it is locked into rocket
	void hidePiece (GameObject piece) {
		piece.GetComponent<SpriteRenderer>().enabled = false; 
	}
	
	void showPieces (GameObject[] pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<SpriteRenderer>().enabled = true; 
			
		}
	}
	
	void showPiece (GameObject piece) {
		piece.GetComponent<SpriteRenderer>().enabled = true; 
	}
	
	//should set all non outline objects back to their lock positions. call this before setting dragged object positions
	void lockSet(GameObject[] pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<ObjectInfo> ().reLock (); 
		}
	}
	void lockAll() {
		lockSet (boosterPieces);
		lockSet (conePieces);
		lockSet (bodyPieces);
		lockSet (finPieces);
	}
	void firstTouch (GameObject[] pieces) {
		foreach (GameObject piece in pieces) {
			piece.GetComponent<ObjectInfo> ().firstTouch = false;
		}
	}
	
	void firstTouchReset() {
		firstTouch (boosterPieces);
		firstTouch (finPieces);
		firstTouch (bodyPieces);
		firstTouch (conePieces);
	}
	
	void newLock(GameObject piece) {
		if (piece != null) {
			float distance;
			float minDistance = Vector3.Distance(piece.transform.position, piece.GetComponent<ObjectInfo>().lockPosition);
			float saveMinDistance = minDistance;
			Vector3 newLock = piece.GetComponent<ObjectInfo> ().lockPosition;
			foreach (GameObject outline in outlinePieces) {
				distance = Vector3.Distance (outline.transform.position, piece.transform.position);
				if (minDistance > distance  && !collisionLoop(outline.transform.position)) {
					minDistance = distance;
					newLock = outline.transform.position;
				}
			}
			foreach (GameObject trash in trashcans) {
				distance = Vector3.Distance (trash.transform.position, piece.transform.position);
				if (minDistance > distance) {
					minDistance = distance;
					newLock = trash.transform.position;
				}
			}
			if (minDistance != saveMinDistance) {
				piece.GetComponent<ObjectInfo> ().newLock (newLock);
			}
		}
	}

	void addToRocketPieces(GameObject selectedBodyPiece) {
		//if the piece is locked into the body add it to the list of rocket pieces
		if (selectedBodyPiece != null) {
			var script = selectedBodyPiece.GetComponent<ObjectInfo> ();

			if (script.seeMe == true && selectedBodyPiece.transform.position == script.lockPosition &&
				script.lockPosition != script.initialLockPosition && script.added == false) {
				rocketPieces.Add (selectedBodyPiece);
				script.added = true;
			}
		}
	}
	
	/* Returns the GameObject of the piece that the mouse is over or null if the mouse isn't over a piece */
	GameObject MouseOverPiece(Vector3 mousePos, int pieceType) {
		// initialize outputs
		GameObject output = null;
		
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
		
		// Detect whether the mouse position is within one of the pieces
		foreach (GameObject piece in pieces) {
			if (Contains (piece, mousePos)) {
				output = piece;
				break;
			}
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
		foreach (GameObject obj in boosterPieces) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		foreach (GameObject obj in conePieces) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		foreach (GameObject obj in finPieces) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		foreach (GameObject obj in bodyPieces) {
			if (obj.transform.position == pos) {
				return true;
			}
		}
		return false;
	}
}
