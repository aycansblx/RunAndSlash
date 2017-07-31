using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// menu controller waits for the space key to start the game
public class MenuController : MonoBehaviour {
	
	// menu elements in the scene
	public GameObject [] Menu;
	// game elements in the scene
	public GameObject [] Game;

	// following boolean variable is true when the menu is active
	private bool _active;

	// Use this for initialization
	void Start () {
		_active = true;
	}

	// Update is called once per frame
	void Update () {
		// if the menu is active and player hit the space button
		if (_active && Input.GetKeyDown (KeyCode.Space)) {
			_active = false;
			FindObjectOfType<GameController> ().Reset ();
			FindObjectOfType<ForegroundBehaviour> ().Reset ();
			// yeah tweeeeeeens <3 menu elements first
			iTween.MoveBy (Menu [0], iTween.Hash ("y", 500f, "time", 0.7f, "easeType", "easeInBack"));
			iTween.MoveBy (Menu [1], iTween.Hash ("y", 10f,  "time", 0.7f, "easeType", "easeInBack"));
			// and game elements 
			iTween.MoveBy (Game [0], iTween.Hash ("y", 10f,  "time", 0.7f, "delay", 0.7f, "easeType", "easeOutBack"));
			iTween.MoveBy (Game [1], iTween.Hash ("y", 500f, "time", 0.7f, "delay", 0.7f, "easeType", "easeOutBack"));
			iTween.MoveBy (Game [2], iTween.Hash ("y", 10f,  "time", 0.7f, "delay", 0.7f, "easeType", "easeOutBack"));
			iTween.MoveBy (Game [3], iTween.Hash ("y", 500f, "time", 0.7f, "delay", 0.7f, "easeType", "easeOutBack"));
			iTween.MoveBy (Game [4], iTween.Hash ("y", 500f, "time", 0.7f, "delay", 0.7f, "easeType", "easeOutBack", "oncomplete", "ShowUp", "oncompletetarget", this.gameObject));
			// last tween calls the ShowUp method in the GameController
		}
	}

	// when the game is over, the menu is activated with following method
	public void Activate() {
		_active = true;
		iTween.MoveBy (Menu [0], iTween.Hash ("y", -500f, "time", 0.7f, "easeType", "easeOutBack"));
		iTween.MoveBy (Menu [1], iTween.Hash ("y", -10f,  "time", 0.7f, "easeType", "easeOutBack"));
	}
}
