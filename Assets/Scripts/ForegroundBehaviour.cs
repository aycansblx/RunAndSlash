using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// the platform under the barbarian - partially similar to BackgroundBehaviour
public class ForegroundBehaviour : MonoBehaviour {

	// moving speed of the platform
	public float Speed;

	// tiles which will be used for building the platform
	// upper tiles are for the top row, lower tiles are for the rest
	public Sprite [] UpperTiles;
	public Sprite [] LowerTiles;

	// PREFABS - this script spawns goblins and food
	public GameObject Chips;
	public GameObject Drink;
	public GameObject Hamburger;
	public GameObject IdleGoblin;
	public GameObject RunningGoblin;

	// a list for goblins (idle ones) and food in the scene
	private List<GameObject> _objects;

	// there are two platform pieces - for circular movement
	private Transform _first;
	private Transform _second;

	// true when the barbarian crawls the dungeon
	private bool _moving;

	// timers and counters
	private int _hordeSize;				// current horde size
	private int _energyCreated;			// energy value of the food created so far
	private int _lastEnergyHorde;		// when did we spawn the last food?
	private float _timerHorde;			// horde timer
	private float _timerSingle;			// single goblins timer

	// initial values of in-class variables
	public void Reset () {
		_moving = false;
		_hordeSize = 5;
		_energyCreated = 0;
		_lastEnergyHorde = 5;
		_timerHorde = 0f;
		_timerSingle = 0f;
		_objects = new List<GameObject> ();
	}

	// Use this for initialization
	void Start () {
		Reset ();
		_first = transform.GetChild (0);
		_second = transform.GetChild (1);

		// creating the first wall
		for (float y = 0.12f; y >= -0.12f; y -= 0.08f) {
			for (float x = -1.6f; x <= 1.6f; x += 0.08f) {
				GameObject tile = new GameObject ();
				tile.transform.parent = _first;
				SpriteRenderer sr = tile.AddComponent<SpriteRenderer> ();
				if (y == 0.12f)
					sr.sprite = UpperTiles [Random.Range (0, UpperTiles.Length)];
				else
					sr.sprite = LowerTiles [Random.Range (0, LowerTiles.Length)];
				sr.sortingOrder = 5;
				tile.transform.localPosition = new Vector3 (x, y, 0.0f);
			}
		}

		// second wall
		for (float y = 0.12f; y >= -0.12f; y -= 0.08f) {
			for (float x = -1.6f; x <= 1.6f; x += 0.08f) {
				GameObject tile = new GameObject ();
				tile.transform.parent = _second;
				SpriteRenderer sr = tile.AddComponent<SpriteRenderer> ();
				if (y == 0.12f)
					sr.sprite = UpperTiles [Random.Range (0, UpperTiles.Length)];
				else
					sr.sprite = LowerTiles [Random.Range (0, LowerTiles.Length)];
				sr.sortingOrder = 5;
				tile.transform.localPosition = new Vector3 (x, y, 0.0f);
			}
		}

		// scaling and positioning platform pieces
		_first.transform.localPosition = new Vector3 (0f, -0.25f, 0f);
		_second.transform.localPosition = new Vector3 (3.2f, -0.25f, 0f);
		transform.localScale = new Vector3 (16f, 16f, 16f);
	}

	// horde generator
	void HandleHorde () {
		_timerHorde += Time.deltaTime;
		if ((int)_timerHorde == 3f) {
			_timerHorde += 1f;
			for (int i = 0; i < _hordeSize; i++) {
				if (Random.value < 0.3f)
					CreateIdleGoblin ();
				else
					CreateRunningGoblin ();
			}
			_hordeSize++;
		}
		if (_timerHorde > 6f)
			_timerHorde = 0f;
	}

	// single goblin (or a couple of goblins) generator
	void HandleGoblins () {
		_timerSingle += Time.deltaTime;
		if ((int)_timerSingle == 1f) {
			_timerSingle += 1f;
			for (int i = 0; i < Random.Range (1, 4); i++) {
				if (Random.value < 0.3f)
					CreateIdleGoblin ();
				else
					CreateRunningGoblin ();
			}
		}
		if (_timerSingle > 9f)
			_timerSingle = 0f;
	}

	// food spawner
	void HandleFood () {
		if (_hordeSize >= _lastEnergyHorde + 2) {
			int choice = Random.Range (1,5);
			if (choice == 1 && _energyCreated + 20 <= 200) {
				_objects.Add (Instantiate (Hamburger, new Vector3 (30f, -1.45f, 0f), Quaternion.identity));
				_energyCreated += 20;
			} else if (choice == 2  && _energyCreated + 10 <= 200) {
				_objects.Add (Instantiate (Chips, new Vector3 (30f, -1.45f, 0f), Quaternion.identity));
				_energyCreated += 10;
			} else if (choice == 3  && _energyCreated + 5 <= 200) {
				_objects.Add (Instantiate (Drink, new Vector3 (30f, -1.45f, 0f), Quaternion.identity));
				_energyCreated += 5;
			} 
			if (choice != 4)
				_lastEnergyHorde = _hordeSize;
		}
	}

	// Update is called once per frame
	void Update () {
		// platform logic runs if the barbarian crawls the dungeon
		if (_moving) {
			HandleHorde ();
			HandleGoblins ();
			HandleFood ();
			// a simple horizontal moving logic
			_first.localPosition = _first.localPosition - new Vector3 (Speed*Time.deltaTime, 0f, 0f);
			_second.localPosition = _second.localPosition - new Vector3 (Speed*Time.deltaTime, 0f, 0f);
			if (_first.localPosition.x < -3.15f)
				_first.localPosition = _second.localPosition + new Vector3 (3.2f, 0f, 0f);
			if (_second.localPosition.x < -3.15f)
				_second.localPosition = _first.localPosition + new Vector3 (3.2f, 0f, 0f);
			// flush operation for idle goblins and food
			for (int i = 0; i < _objects.Count; i++) {
				Transform t = _objects [i].transform;
				t.position = t.position - new Vector3 (Speed * Time.deltaTime * 10f, 0f, 0f);
				if (t.position.x < -15f) {
					GameObject go = _objects [i];
					_objects.RemoveAt (i--);
					Destroy (go);
				}
			}
		}
	}

	// services for other controllers;
	public void Move () {
		_moving = true;
	}

	public void Stop () {
		_moving = false;
	}

	public bool IsMoving () {
		return _moving;
	}

	// create idle goblin
	void CreateIdleGoblin () {
		Vector3 pos = Vector3.zero;
		GameObject[] goblins = GameObject.FindGameObjectsWithTag ("Goblin");
		float distance = 1.0f;
		do {
			pos = new Vector3 (Random.Range (13f, 25f), -1.45f, 0f);
			foreach (GameObject go in goblins) {
				if (Vector3.Distance (go.transform.position, pos) < distance) {
					distance -= 0.05f;
					pos = Vector3.zero;
					break;
				}
			}
		} while (pos == Vector3.zero);
		GameObject goblin = Instantiate (IdleGoblin, pos, Quaternion.identity);
		_objects.Add (goblin);
	}

	// create running goblin
	void CreateRunningGoblin () {
		Vector3 pos = Vector3.zero;
		GameObject[] goblins = GameObject.FindGameObjectsWithTag ("Goblin");
		float distance = 1.0f;
		do {
			pos = new Vector3 (Random.Range (13f, 25f), -1.45f, 0f);
			foreach (GameObject go in goblins) {
				if (Vector3.Distance (go.transform.position, pos) < distance) {
					distance -= 0.05f;
					pos = Vector3.zero;
					break;
				}
			}
		} while (pos == Vector3.zero);
		Instantiate (RunningGoblin, pos, Quaternion.identity);
	}
}
