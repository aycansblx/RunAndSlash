using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningGoblinBehaviour : MonoBehaviour {

	private float _speed;
	private ForegroundBehaviour _fb;

	// Use this for initialization
	void Start () {
		_speed = Random.Range (7f, 11f);
		_fb = FindObjectOfType<ForegroundBehaviour> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_fb.IsMoving ()) {
			if (GetComponent<Animator>() != null)
				if (GetComponent<Animator> ().speed == 0.0f)
					GetComponent<Animator> ().speed = 1.0f;
			transform.position = transform.position - new Vector3 (_speed * Time.deltaTime, 0f, 0f);
			if (transform.position.x < -15f)
				Destroy (this.gameObject);
		} else if (GetComponent<Animator>() != null){
			GetComponent<Animator> ().speed = 0.0f;
		}
	}
}
