using System.Collections;
using UnityEngine;

public class ToonHeadManager : MonoBehaviour {

	public GameObject closed, open;
	public GameObject leftPupil, rightPupil;
	public GameObject eyes;
	public Texture2D eyesNormal, eyesClosed;

	void Start () {
		InvokeRepeating ("RunBlink", 0f, 3f);
	}

	void RunBlink () {
		StartCoroutine (Blink ());
	}

	IEnumerator Blink () {
		if (Random.value > 0f) {
			if (open != null)
				open.SetActive (false);
			if (closed != null)
				closed.SetActive (true);
			
			leftPupil.SetActive (false);
			rightPupil.SetActive (false);
			eyes.GetComponent<Renderer> ().material.mainTexture = eyesClosed;

			yield return new WaitForSeconds (0.2f);

			if (closed != null)
				closed.SetActive (false);
			if (open != null)
				open.SetActive (true);
			
			leftPupil.SetActive (true);
			rightPupil.SetActive (true);
			eyes.GetComponent<Renderer> ().material.mainTexture = eyesNormal;
		}
	}
}