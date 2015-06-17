using UnityEngine;
using System;
using System.Collections;
using TouchScript.Gestures;

public class MovePiece : MonoBehaviour {

	void OnEnable()
	{
		// subscribe to gesture's Tapped event
		GetComponent<TapGesture>() .Tapped += tappedHandler;
		GetComponent<PressGesture> ().Pressed += pressedHandler;
		GetComponent<LongPressGesture> ().LongPressed += longPressedHandler;
		GetComponent<ReleaseGesture> ().Released += releasedHandler;
	}
	
	void OnDisable()
	{
		// don't forget to unsubscribe
		GetComponent<TapGesture>() .Tapped -= tappedHandler;
		GetComponent<PressGesture> ().Pressed -= pressedHandler;
		GetComponent<LongPressGesture> ().LongPressed -= longPressedHandler;
		GetComponent<ReleaseGesture> ().Released -= releasedHandler;
	}
	
	void longPressedHandler(object sender, EventArgs e)
	{
		Debug.Log ("Long Pressed: " + gameObject.name);
	}
	
	void pressedHandler(object sender, EventArgs e)
	{
		Debug.Log ("Pressed: " + gameObject.name);
	}
	
	void releasedHandler(object sender, EventArgs e)
	{
		Debug.Log ("Released: " + gameObject.name);
	}

	void tappedHandler(object sender, EventArgs e)
	{
		Debug.Log ("Tapped: " + gameObject.name);
	}
}
