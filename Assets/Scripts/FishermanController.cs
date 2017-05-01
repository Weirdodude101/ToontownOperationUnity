using System.Collections;
using UnityEngine;

public class FishermanController : MonoBehaviour {

	public Animator[] childAnimators;

	private int randomChoice;
	private int lastChoice;

	void Start () {
		InvokeRepeating ("DecidePos", 0f, 15f);
	}

	void DecidePos () {
		randomChoice = Random.Range (0, 3);
		lastChoice = randomChoice;

		while (randomChoice == lastChoice)
			randomChoice = Random.Range (0, 3);
		
		StartCoroutine (RandomAnimation (randomChoice));
	}

	IEnumerator RandomAnimation (int decision) {
		if (decision == 0) {
			for (int i = 0; i > childAnimators.Length; i++) {
				childAnimators [i].SetTrigger ("castLong");
			}
		}
		yield return null;
	}
}
