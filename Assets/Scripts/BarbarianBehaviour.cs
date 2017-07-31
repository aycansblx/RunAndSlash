using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// barbarian behaviour - animation events, collisions and etc.
public class BarbarianBehaviour : MonoBehaviour {

	// controller
	public GameObject Controller;

	// sound fx
	public AudioClip [] FX;

	// throwing axe
	public GameObject ThrowingAxe;

	// modifier outputs (-1, +5, +10, +20 power)
	public GameObject [] Modifiers;

	// this script moves the camera back to its original position
	private bool _cameraSizeUp;
	private bool _cameraSizeDown;

	// Use this for initialization
	void Start () {
		_cameraSizeUp = false;
		_cameraSizeDown = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (_cameraSizeDown) {
			float size = Camera.main.orthographicSize;
			size += Time.deltaTime * 2.5f / 0.19f;
			Camera.main.orthographicSize = size;
			if (size >= 5f) {
				Camera.main.orthographicSize = 5f;
				_cameraSizeDown = false;
				if (GetComponent<Animator> ().GetInteger ("Skill") == 3)
					iTween.MoveTo (this.gameObject, iTween.Hash ("x", 7f, "time", 0.25f, "easeType", "linear"));
			}
		} else if (_cameraSizeUp) {
			float size = Camera.main.orthographicSize;
			size -= Time.deltaTime * 1.5f / 0.33f;
			Camera.main.orthographicSize = size;
			if (size <= 5f) {
				Camera.main.orthographicSize = 5f;
				_cameraSizeUp = false;
			}
		}
	}
		
	// first pass of skill q
	public void SkillQP1 () {
		GetComponent<AudioSource> ().clip = FX [0];
		GetComponent<AudioSource> ().Play ();
		iTween.MoveTo (this.gameObject, iTween.Hash ("x", -7f, "time", 0.1f, "easeType", "linear"));
	}

	// second pass of skill q
	public void SkillQP2 () {
		GetComponent<AudioSource> ().clip = FX [1];
		GetComponent<AudioSource> ().Play ();
		iTween.MoveTo (this.gameObject, iTween.Hash ("x", -5.5f, "time", 0.1f,"easeType", "linear"));
	}

	// third pass of skill q
	public void SkillQP3 () {
		GetComponent<AudioSource> ().clip = FX [2];
		GetComponent<AudioSource> ().Play ();
		iTween.MoveTo (this.gameObject, iTween.Hash ("x", -4f, "time", 0.1f, "easeType", "linear"));
	}

	// fourth pass of skill q
	public void SkillQP4 () {
		GetComponent<AudioSource> ().clip = FX [3];
		GetComponent<AudioSource> ().Play ();
		iTween.MoveTo (this.gameObject, iTween.Hash ("x", -2.5f, "time", 0.1f, "easeType", "linear"));
	}

	// first pass of skill w
	public void SkillWP1 () {
		GetComponent<AudioSource> ().clip = FX [4];
		GetComponent<AudioSource> ().Play ();
		iTween.MoveTo (this.gameObject, iTween.Hash ("x", -1.5f, "y", 3f, "time", 0.3f, "easeType", "linear"));
	}

	// second pass of skill w
	public void SkillWP2 () {
		iTween.MoveTo (this.gameObject, iTween.Hash ("x", 6.5f, "y", -1.45f, "time", 0.3f, "easeType", "linear"));
	}

	// first pass of skill e
	public void SkillEP1 () {
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", 0f, "y", 0f, "time", 0.19f, "easeType", "linear"));
		_cameraSizeDown = true;
	}

	// first pass of skill r
	public void SkillRP1 () {
		int numberOfAxes = Random.Range (7, 12);
		for (int i = 0; i < numberOfAxes; i++)
			Instantiate (ThrowingAxe, new Vector3(Random.Range(-6.5f, -6f), Random.Range(-1.5f, 1.5f), 0f), Quaternion.identity);
	}

	// termination of skill r
	public void BackFromSkillR () {
		_cameraSizeDown = true;
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", 0f, "time", 0.19f, "easeType", "linear", "oncomplete", "Go", "oncompletetarget", Controller));
	}

	// termination of skill e
	public void BackFromSkillE () {
		FindObjectOfType<GameController> ().Go ();
	}

	// termination of skill w
	public void BackFromSkillW () {
		iTween.ShakePosition (Camera.main.gameObject, iTween.Hash ("x", 1.5f, "y", 1.5f, "time", 0.67f, "easeType", "linear"));
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", 0f, "time", 0.33f, "easeType", "linear", "oncomplete", "Go", "oncompletetarget", Controller));
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Goblin")) {
			if (go.transform.position.x <= transform.position.x + 1f) {
				float x = Random.value < 0.5f ? Random.Range (-20f, -15f) : Random.Range (15f, 20f);
				iTween.MoveBy (go, iTween.Hash ("y", Random.Range (10f, 19f), "x", x, "time", 0.7f, "easeType", "linear"));
				Destroy (go.GetComponent<BoxCollider2D> ());
				go.GetComponent<AudioSource> ().Play ();
			}
		}
		_cameraSizeUp = true;
	}

	// termination of skill q
	public void BackFromSkillQ () {
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", 0f, "time", 0.19f, "easeType", "linear", "oncomplete", "Go", "oncompletetarget", Controller));
		_cameraSizeDown = true;
	}

	// events for skill e and r which play sound fx

	public void PlayEFX () {
		GetComponent<AudioSource> ().clip = FX [5];
		GetComponent<AudioSource> ().Play ();
	}

	public void PlayRFX () {
		GetComponent<AudioSource> ().clip = FX [6];
		GetComponent<AudioSource> ().Play ();
	}

	// yeah
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals ("Goblin") && GetComponent<Animator> ().GetInteger ("Skill") == 0 && transform.position.x < -5f) {
			FindObjectOfType<GameController> ().ModifyPower (-1f);
			GameObject go = Instantiate (Modifiers [0]);
			go.transform.parent = this.gameObject.transform;
			go.transform.localPosition = new Vector3 (0.1f, 0.5f, 0f);
			iTween.MoveBy (go, iTween.Hash ("y", 1f, "time", 0.19f, "easeType", "linear", "oncomplete", "KillObject", "oncompleteparams", go, "oncompletetarget", this.gameObject));
			Destroy (other.GetComponent<BoxCollider2D> ());
			GetComponent<AudioSource> ().clip = FX [8];
			GetComponent<AudioSource> ().Play ();
		} else if (other.tag.Equals ("Goblin")) {
			int skill = GetComponent<Animator> ().GetInteger ("Skill");
			if (skill == 1 || skill == 3) {
				float x = Random.value < 0.5f ? Random.Range (-20f, -15f) : Random.Range (15f, 20f);
				iTween.MoveBy (other.gameObject, iTween.Hash ("y", Random.Range (10f, 19f), "x", x, "time", 0.7f, "easeType", "linear"));
				other.GetComponent<AudioSource> ().Play ();
			}
		} else if (other.tag.Equals ("Hamburger")) {
			FindObjectOfType<GameController> ().ModifyPower (20f);
			GameObject go = Instantiate (Modifiers [3]);
			go.transform.parent = this.gameObject.transform;
			go.transform.localPosition = new Vector3 (0.1f, 0.5f, 0f);
			iTween.MoveBy (go, iTween.Hash ("y", 1f, "time", 0.19f, "easeType", "linear", "oncomplete", "KillObject", "oncompleteparams", go, "oncompletetarget", this.gameObject));
			other.gameObject.SetActive (false);
			GetComponent<AudioSource> ().clip = FX [7];
			GetComponent<AudioSource> ().Play ();
		} else if (other.tag.Equals ("Chips")) {
			FindObjectOfType<GameController> ().ModifyPower (10f);
			GameObject go = Instantiate (Modifiers [2]);
			go.transform.parent = this.gameObject.transform;
			go.transform.localPosition = new Vector3 (0.1f, 0.5f, 0f);
			iTween.MoveBy (go, iTween.Hash ("y", 1f, "time", 0.19f, "easeType", "linear", "oncomplete", "KillObject", "oncompleteparams", go, "oncompletetarget", this.gameObject));
			other.gameObject.SetActive (false);
			GetComponent<AudioSource> ().clip = FX [7];
			GetComponent<AudioSource> ().Play ();
		} else if (other.tag.Equals ("Drink")) {
			FindObjectOfType<GameController> ().ModifyPower (5f);
			GameObject go = Instantiate (Modifiers [1]);
			go.transform.parent = this.gameObject.transform;
			go.transform.localPosition = new Vector3 (0.1f, 0.5f, 0f);
			iTween.MoveBy (go, iTween.Hash ("y", 1f, "time", 0.19f, "easeType", "linear", "oncomplete", "KillObject", "oncompleteparams", go, "oncompletetarget", this.gameObject));
			other.gameObject.SetActive (false);
			GetComponent<AudioSource> ().clip = FX [7];
			GetComponent<AudioSource> ().Play ();
		}
	}

	public void KillObject (GameObject go) {
		Destroy (go);
	}
}
