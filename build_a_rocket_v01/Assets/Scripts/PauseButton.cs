using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		var touch = GameObject.Find ("GameManager").GetComponent<TouchInputHandler> ();
		if (touch.usingMouse) {
			//Debug.Log ("mouse");
			if (Input.GetMouseButton (0)) {
				//Debug.Log ("button");
				//Debug.Log (Input.mousePosition);
				//Debug.Log(Vector3.Distance (Input.mousePosition, gameObject.transform.position));
				
				if (touch.Contains(gameObject, Input.mousePosition)) {
					Debug.Log ("toggling");
					touch.paused = true;
				} else {
					touch.paused = false;
				}
			}
		} else {
			if (Input.touchCount > 0) {
				foreach (Touch t in Input.touches) {
					if (touch.Contains (gameObject, t.position)) {
						touch.paused = !touch.paused;
					}
				}
			}
		}

	}
}
