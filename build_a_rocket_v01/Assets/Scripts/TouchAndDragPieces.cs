using UnityEngine;
using System.Collections;

public class TouchAndDragPieces : MonoBehaviour {
	
	// flags to be able to use with either mouse or touch or both
	public bool usingMouse = false;
	public bool usingTouch = true;
	public int pixelsPerUnit = 10;
	
	private float cameraHeight;
	private float cameraWidth;
	private SpriteRenderer spriteRenderer;
	private Vector3 piecePosition;
	private float pieceHeight = 0f;
	private float pieceWidth = 0f;
	private bool touched = false;
	//for jumping back to the start
	private Vector3 lockPosition;
	private bool firstTouch = false;
	private float jumpSize;
	
	// pre-initialization
	void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
	}
	
	// Use this for initialization
	void Start () {
		// get the initial position, width, and height of the piece 
		piecePosition = gameObject.transform.position;
		pieceWidth = spriteRenderer.bounds.size.x;//* pixelsPerUnit;
		pieceHeight = spriteRenderer.bounds.size.y ;//* pixelsPerUnit;
		lockPosition = piecePosition; 
		
		//setting initials t be either the width to the elft or the width to the right
		GameObject objLeft = GameObject.Find ("LeftPanel");
		jumpSize = objLeft.GetComponent<MeshRenderer> ().bounds.size.x;
		if (piecePosition.x < 0) { 
			lockPosition.x = piecePosition.x + jumpSize; 
		} else {
			lockPosition.x = piecePosition.x - jumpSize;
		}
		
		// obtain the width and height of the camera
		Camera cam = Camera.main;
		cameraHeight = 2f * cam.orthographicSize;
		cameraWidth = Mathf.Round (cameraHeight * cam.aspect);
		
		// debugs
		//		Debug.Log (Screen.width);
		//		Debug.Log (Screen.height);
		//		Debug.Log (cameraHeight);
		//		Debug.Log (cameraWidth);
		//		Debug.Log (piecePosition);
		//		Debug.Log (pieceWidth);
		//		Debug.Log (pieceHeight);
	}
	
	// Update is called once per frame
	void Update () {
		
		// for the mouse inputs
		if (usingMouse) {
			// if the left mouse button is clicking on our object
			if(Input.GetMouseButton(0)) {
				if (Contains(gameObject.transform.position, pieceWidth, pieceHeight, Input.mousePosition))
				{
					Vector3 mousePos = Input.mousePosition;
					
					touched = true;
					piecePosition = new Vector3((mousePos.x * cameraWidth / Screen.width) - (cameraWidth / 2), 
					                            (mousePos.y * cameraHeight / Screen.height) - (cameraHeight / 2), 
					                            0);
				}
			}
		}
		//attempt at makingt his work for touch inputs, but probably needs to be updated
		if (usingTouch) {
			if (Input.touchCount >= 1) {
				Vector3 touchCache;
				//For each touch
				foreach (Touch touch in Input.touches) {
					//Cache touch position
					touchCache = touch.position;
					
					if (Contains (gameObject.transform.position, pieceWidth, pieceHeight, touchCache)) {
						touched = true;
						piecePosition = new Vector3 ((touchCache.x * cameraWidth / Screen.width) - (cameraWidth / 2), 
						                             (touchCache.y * cameraHeight / Screen.height) - (cameraHeight / 2), 
						                             0);
					}
				}
			}
		}
		if (Input.touchCount > 0)
			Debug.Log ("More than 0 touches");
		
		if (touched) {
			gameObject.transform.position = piecePosition;
			touched = false;
			if (firstTouch == false) {
				firstTouch = true;
			}
		}else {
			if (firstTouch == true) {
				gameObject.transform.position = lockPosition;
			}
		}
		
	}
	
	
	
	bool Contains (Vector3 containerPosition, float containerWidth, float containerHeight, Vector3 touchItem) {
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
}



