using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// da real sh*t
public class GameController : MonoBehaviour {
	// necessary gameobjects in the scene
	public GameObject Message;
	public GameObject SkillUI;
	public GameObject PowerBar;
	public GameObject Barbarian;
	public GameObject GameItems;
	public GameObject ProgressBar;

	// another bool variable that indicates if the barbarian crawls the dungeon
	private bool _moving;

	// and another bool variable that indicates if skill usage is available
	private bool _skill;

	// sometimes, game controller changes orthographic size of the camera
	// set one of the following bools and the camera will change its size
	private bool _camera2Face;
	private bool _cameraSizeUp;	
	private bool _cameraSizeDown;

	// skill cooldown timers
	private float _cooldownQ;
	private float _cooldownW;
	private float _cooldownE;
	private float _cooldownR;

	private float _power; 			// the theme - power left (initially 100)
	private float _progress;		// level progress - you reach 100, you win (initially 0)

	private float _fadeTimer;

	// initial values of bool vars
	public void Reset() {
		_skill = true;
		_moving = false;
		_camera2Face = false;
		_cameraSizeUp = false;
		_cameraSizeDown = false;
		_power = 100f;
		_progress = 0f;
		_cooldownQ = 0f;
		_cooldownW = 0f;
		_cooldownE = 0f;
		_cooldownR = 0f;
		_fadeTimer = -1f;
		Barbarian.transform.position = new Vector3 (-19f, -1.45f, 0f);
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", 0);
		HandlePower ();
		HandleProgress ();
		HandleCoolDowns ();
	}

	// Use this for initialization
	void Start () {
		Reset ();
	}

	// update power attribute
	void HandlePower () {
		_power -= Time.deltaTime / 2f;
		if (_power < 1f) {
			_moving = false;
			FindObjectOfType<BackgroundBehaviour> ().Stop ();
			FindObjectOfType<ForegroundBehaviour> ().Stop ();
			Barbarian.GetComponent<Animator> ().SetInteger ("Skill", -2);
			iTween.ShakePosition (Barbarian, iTween.Hash ("x", 0.33f, "delay", 2f, "time", 0.7f, "easeType", "linear", "oncomplete", "Finish", "oncompletetarget", this.gameObject));
			Message.GetComponent<Text>().text = "you are tired";
			_fadeTimer = 0f;
		}
		PowerBar.transform.GetChild (1).localScale = new Vector3 (70f * _power / 100f, 3f, 1f);
		PowerBar.transform.GetChild (2).GetComponent<Text> ().text = "Power: " + (int)_power + "/100";
	}

	// update progress attribute
	void HandleProgress () {
		_progress += Time.deltaTime / 2f;
		ProgressBar.transform.GetChild (2).localPosition = new Vector3 (-270f + 540f * _progress / 100f, 0f, 0f);
		ProgressBar.transform.GetChild (1).localScale = new Vector3 (70f * _progress / 100f, 3f, 1f);
		ProgressBar.transform.GetChild (3).GetComponent<Text> ().text = "Progress: " + (int)_progress + "%";
		if (_progress >= 100) {
			_moving = false;
			FindObjectOfType<BackgroundBehaviour> ().Stop ();
			FindObjectOfType<ForegroundBehaviour> ().Stop ();
			iTween.MoveTo (Barbarian, iTween.Hash ("x", 20f, "time", 2f, "easeType", "linear", "oncomplete", "Finish", "oncompletetarget", this.gameObject));
			Message.GetComponent<Text>().text = "dungeon completed";
			_fadeTimer = 0f;
		}
	}

	// update cooldown timers
	void HandleCoolDowns() {
		if (_cooldownQ >= 0f) {
			_cooldownQ -= Time.deltaTime;
			SkillUI.transform.GetChild (0).GetChild (2).GetComponent<Text> ().text = "" + ((int)(_cooldownQ * 10f)) / 10f;
			if (_cooldownQ <= 0f) {
				GameItems.transform.GetChild (0).GetComponent<SpriteRenderer> ().color = Color.white;
				SkillUI.transform.GetChild (0).GetChild (2).GetComponent<Text> ().text = "";
			}
		}
		if (_cooldownW >= 0f) {
			_cooldownW -= Time.deltaTime;
			SkillUI.transform.GetChild (1).GetChild (2).GetComponent<Text> ().text = "" + ((int)(_cooldownW * 10f)) / 10f;
			if (_cooldownW <= 0f) {
				GameItems.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = Color.white;
				SkillUI.transform.GetChild (1).GetChild (2).GetComponent<Text> ().text = "";
			}
		}
		if (_cooldownE >= 0f) {
			_cooldownE -= Time.deltaTime;
			SkillUI.transform.GetChild (2).GetChild (2).GetComponent<Text> ().text = "" + ((int)(_cooldownE * 10f)) / 10f;
			if (_cooldownE <= 0f) {
				GameItems.transform.GetChild (2).GetComponent<SpriteRenderer> ().color = Color.white;
				SkillUI.transform.GetChild (2).GetChild (2).GetComponent<Text> ().text = "";
			}
		}
		if (_cooldownR >= 0f) {
			_cooldownR -= Time.deltaTime;
			SkillUI.transform.GetChild (3).GetChild (2).GetComponent<Text> ().text = "" + ((int)(_cooldownR * 10f)) / 10f;
			if (_cooldownR <= 0f) {
				GameItems.transform.GetChild (3).GetComponent<SpriteRenderer> ().color = Color.white;
				SkillUI.transform.GetChild (3).GetChild (2).GetComponent<Text> ().text = "";
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_fadeTimer >= 0f) {
			_fadeTimer += Time.deltaTime;
			if (_fadeTimer <= 1f) {
				Color color = Message.GetComponent<Text> ().color;
				color.a = _fadeTimer;
				Message.GetComponent<Text> ().color = color;
			} else if (_fadeTimer <= 2f) {
				Color color = Message.GetComponent<Text> ().color;
				color.a = 1f - (_fadeTimer - 1f);
				Message.GetComponent<Text> ().color = color;
			}
		}
		if (_moving) {
			HandlePower ();
			HandleProgress ();
			HandleCoolDowns ();
			// check if the player used any skills
			if (Input.GetKeyDown (KeyCode.Q) && _skill && _cooldownQ <= 0f)
				SkillQ ();
			else if (Input.GetKeyDown (KeyCode.W) && _skill && _cooldownW <= 0f)
				SkillW ();
			else if (Input.GetKeyDown (KeyCode.E) && _skill && _cooldownE <= 0f)
				SkillE ();
			else if (Input.GetKeyUp (KeyCode.R) && _skill && _cooldownR <= 0f)
				SkillR ();
		}

		// camera zooms
		if (_cameraSizeUp) {
			float size = Camera.main.orthographicSize;
			size -= Time.deltaTime * 2.5f / 0.33f;
			Camera.main.orthographicSize = size;
			if (size <= 2.5f) {
				Camera.main.orthographicSize = 2.5f;
				_cameraSizeUp = false;
			}
		} else if (_cameraSizeDown) {
			float size = Camera.main.orthographicSize;
			size += Time.deltaTime * 1.5f / 0.33f;
			Camera.main.orthographicSize = size;
			if (size >= 6.5f) {
				Camera.main.orthographicSize = 6.5f;
				_cameraSizeDown = false;
				PerformW ();
			}
		} else if (_camera2Face) {
			float size = Camera.main.orthographicSize;
			size -= Time.deltaTime * 4.5f / 0.2f;
			Camera.main.orthographicSize = size;
			if (size <= 0.5f) {
				Camera.main.orthographicSize = 0.5f;
				_camera2Face = false;
			}
		}
	}

	// Q skill
	public void SkillQ () {
		Stop ();
		_skill = false;
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", -4f, "time", 0.33f, "easeType", "linear", "oncomplete", "PerformQ", "oncompletetarget", this.gameObject));
		_cameraSizeUp = true;
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", -1);
		GameItems.transform.GetChild (0).GetComponent<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f);
		_cooldownQ = 3f;
		_power -= 1f;
	}

	// W skill
	public void SkillW () {
		Stop ();
		_skill = false;
		_cameraSizeDown = true;
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", -1);
		GameItems.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f);
		_cooldownW = 5f;
		_power -= 2f;
	}

	// E skill
	public void SkillE () {
		_skill = false;
		Stop ();
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", -7.36f, "y", 1.31f, "time", 0.2f, "easeType", "linear", "oncomplete", "PerformE", "oncompletetarget", this.gameObject));
		_camera2Face = true;
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", -1);
		GameItems.transform.GetChild (2).GetComponent<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f);
		_cooldownE = 5f;
		_power -= 2f;
	}

	// R skill
	public void SkillR () {
		Stop ();
		_skill = false;
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash ("x", -4f, "time", 0.33f, "easeType", "linear", "oncomplete", "PerformR", "oncompletetarget", this.gameObject));
		_cameraSizeUp = true;
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", -1);
		GameItems.transform.GetChild (3).GetComponent<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f);
		_cooldownR = 7f;
		_power -= 3f;
	}

	// entrance of the barbarian
	public void ShowUp () {
		Reset ();
		iTween.MoveTo (Barbarian, iTween.Hash ("x", -8.5f, "time", 1.0f, "easeType", "linear", "oncomplete", "Go", "oncompletetarget", this.gameObject));
	}

	// stops the scene
	public void Stop () {
		_moving = false;
		FindObjectOfType<BackgroundBehaviour> ().Stop ();
		FindObjectOfType<ForegroundBehaviour> ().Stop ();
		SkillUI.SetActive (false);
		PowerBar.SetActive (false);
		GameItems.SetActive (false);
		ProgressBar.SetActive (false);
	}

	// resumes the scene
	public void Go () {
		_moving = true;
		FindObjectOfType<BackgroundBehaviour> ().Move ();
		FindObjectOfType<ForegroundBehaviour> ().Move ();
		SkillUI.SetActive (true);
		PowerBar.SetActive (true);
		GameItems.SetActive (true);
		ProgressBar.SetActive (true);
		iTween.MoveTo (Barbarian, iTween.Hash ("x", -8.5f, "time", 0.1f, "easeType", iTween.EaseType.linear, "oncomplete", "SetSkillVar", "oncompletetarget", this.gameObject));
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", 0);
	}

	public void SetSkillVar () {
		_skill = true;
	}

	// skill starters

	public void PerformQ () {
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", 1);
	}

	public void PerformW () {
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", 2);
	}

	public void PerformE () {
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", 3);
	}

	public void PerformR () {
		Barbarian.GetComponent<Animator> ().SetInteger ("Skill", 4);
	}

	// power modifier method
	public void ModifyPower (float x) {
		_power += x;
		if (_power > 100f)
			_power = 100f;
	}

	// when the game finishes
	public void Finish() {
		iTween.MoveBy (GameItems, iTween.Hash ("y", -10f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack"));
		iTween.MoveBy (SkillUI, iTween.Hash ("y", -500f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack"));
		iTween.MoveBy (ProgressBar, iTween.Hash ("y", -500f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack"));
		iTween.MoveBy (PowerBar, iTween.Hash ("y", -500f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack"));
		iTween.MoveBy (Barbarian, iTween.Hash ("y", -10f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack"));
		iTween.MoveBy (GameObject.Find ("Foreground"), iTween.Hash ("y", -10f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack", "oncomplete", "Activate", "oncompletetarget", this.gameObject));
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Goblin")) {
			if (go.transform.position.y <= 0 && go.transform.position.y >= -3)
				iTween.MoveBy (go, iTween.Hash ("y", -10f, "time", 0.7f, "delay", 0.7f, "easeType", "easeInBack", "oncomplete", "Delete", "oncompleteparams", go, "oncompletetarget", this.gameObject));
			else
				Destroy (go);
		}
		if (GameObject.FindGameObjectWithTag ("Hamburger") != null)
			Destroy (GameObject.FindGameObjectWithTag ("Hamburger"));
		if (GameObject.FindGameObjectWithTag ("Chips") != null)
			Destroy (GameObject.FindGameObjectWithTag ("Chips"));
		if (GameObject.FindGameObjectWithTag ("Drink") != null)
			Destroy (GameObject.FindGameObjectWithTag ("Drink"));
	}

	public void Delete (GameObject go) {
		Destroy (go);
	}
}
