using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// the muddy background wall
public class BackgroundBehaviour : MonoBehaviour {

	// moving speed of the wall
	public float Speed;

	// tiles which will be used for building the wall
	public Sprite[] Tiles;

	// there are two wall pieces - for circular movement
	private Transform _first;
	private Transform _second;

	// true when the barbarian crawls the dungeon
	private bool _moving;

	// Use this for initialization
	void Start () {
		// initialization of the private variables
		_moving = false;
		_first = transform.GetChild (0);
		_second = transform.GetChild (1);

		// first wall
		for (float y = 0.24f; y >= -0.24f; y -= 0.08f) {					// building a wall tile by tile
			for (float x = -0.56f; x <= 0.56f; x += 0.08f) {				// (a poem by aycan atak)
				GameObject tile = new GameObject ();						// create a game object first
				tile.transform.parent = _first;								// set its parent
				SpriteRenderer sr = tile.AddComponent<SpriteRenderer> ();	// add a sprite renderer to it
				sr.sprite = Tiles [Random.Range (0, Tiles.Length)];			// randomly render one of the wall tiles
				sr.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);				// make it a little transparent
				tile.transform.position = new Vector3 (x, y, 0.0f);			// finally set its position
			}
		}

		// second wall
		for (float y = 0.24f; y >= -0.24f; y -= 0.08f) {
			for (float x = -0.56f; x <= 0.56f; x += 0.08f) {
				GameObject tile = new GameObject ();
				tile.transform.parent = _second;
				SpriteRenderer sr = tile.AddComponent<SpriteRenderer> ();
				sr.sprite = Tiles [Random.Range (0, Tiles.Length)];
				sr.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				tile.transform.position = new Vector3 (x, y, 0.0f);
			}
		}

		// yeah that's right
		_second.transform.position = new Vector3 (1.2f, 0f, 0f);
		transform.localScale = new Vector3 (33f, 33f, 33f);
	}
	
	// Update is called once per frame
	void Update () {
		// a simple horizontal moving logic which is active when _moving is true
		if (_moving) {
			_first.localPosition = _first.localPosition - new Vector3 (Speed*Time.deltaTime, 0f, 0f);
			_second.localPosition = _second.localPosition - new Vector3 (Speed*Time.deltaTime, 0f, 0f);
			// and for circular move
			if (_first.localPosition.x < -1.15f)
				_first.localPosition = _second.localPosition + new Vector3 (1.2f, 0f, 0f);
			if (_second.localPosition.x < -1.15f)
				_second.localPosition = _first.localPosition + new Vector3 (1.2f, 0f, 0f);
		}
	}

	// services for other controllers;
	public void Move () {
		_moving = true;
	}

	public void Stop () {
		_moving = false;
	}
}
