using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingAxeBehaviour : MonoBehaviour {

	private SpriteRenderer _sr;

	// Use this for initialization
	void Start () {
		_sr = GetComponent<SpriteRenderer> ();
		_sr.enabled = false;
		iTween.MoveBy (this.gameObject, iTween.Hash ("x", 0f, "time", Random.Range (0f, 0.3f), "oncomplete", "ShowUp", "oncompletetarget", this.gameObject, "easeType", "linear"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void ShowUp () {
		_sr.enabled = true;
		iTween.MoveBy (this.gameObject, iTween.Hash ("x", 20f, "time", 0.7f, "oncomplete", "Kill", "oncompletetarget", this.gameObject, "easeType", iTween.EaseType.linear));
	}

	void Kill () {
		Destroy (this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals ("Goblin")) {
			Destroy (other.gameObject.GetComponent<BoxCollider2D> ());
			float z = Random.value > 0.5 ? 90f : -90f;
			other.transform.eulerAngles = new Vector3 (0f, 0f, z);
			Vector3 pos = other.transform.position;
			pos.y = (z == 90) ? -2f : -0.45f;
			other.transform.position = pos;
			if (other.GetComponent<Animator> () != null)
				Destroy (other.gameObject.GetComponent<Animator> ());
		}
	}
}
