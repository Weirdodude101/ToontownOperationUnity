using UnityEngine;
using UnityEngine.UI;

public class CharacterBodyInfoLoader : MonoBehaviour {

	public Text toonNameText;

	private GameObject currentHead;
	private GameObject currentBody;
	private GameObject currentLegs;
	private Transform currentBodyHolder;
	private Transform currentHeadHolder;

	private string speciesType;
	private string headType;
	private string bodyType;
	private string legType;

	void Start () {
		if (PlayerPrefs.GetInt ("speciesIntPref") == 0)
			speciesType = "dog";
		else if (PlayerPrefs.GetInt ("speciesIntPref") == 1)
			speciesType = "bear";
		else if (PlayerPrefs.GetInt ("speciesIntPref") == 2)
			speciesType = "cat";
		else if (PlayerPrefs.GetInt ("speciesIntPref") == 3)
			speciesType = "duck";
		else if (PlayerPrefs.GetInt ("speciesIntPref") == 4)
			speciesType = "horse";

		if (PlayerPrefs.GetInt ("headTypePref") == 0)
			headType = "ss";
		else if (PlayerPrefs.GetInt ("headTypePref") == 1)
			headType = "sl";
		else if (PlayerPrefs.GetInt ("headTypePref") == 2)
			headType = "ls";
		else if (PlayerPrefs.GetInt ("headTypePref") == 3)
			headType = "ll";

		if (PlayerPrefs.GetInt ("bodyTypePref") == 0)
			bodyType = "fat";
		else if (PlayerPrefs.GetInt ("bodyTypePref") == 1)
			bodyType = "short";
		else if (PlayerPrefs.GetInt ("bodyTypePref") == 2)
			bodyType = "tall";

		if (PlayerPrefs.GetInt ("legTypePref") == 0)
			legType = "fat";
		else if (PlayerPrefs.GetInt ("legTypePref") == 1)
			legType = "short";
		else if (PlayerPrefs.GetInt ("legTypePref") == 2)
			legType = "tall";

		currentLegs = Instantiate (Resources.Load ("Prefabs/" + legType + "_legs", typeof(GameObject)), new Vector3 (0f, 3.5f, 0f), Quaternion.Euler (0f, -180f, 0f)) as GameObject;
		currentBodyHolder = currentLegs.transform.FindDeepChild ("joint_hips1");

		if (PlayerPrefs.GetInt ("femaleIntPref") == 1) {
			currentBody = Instantiate (Resources.Load ("Prefabs/" + bodyType + "_skirt", typeof(GameObject)), currentBodyHolder.position, new Quaternion (currentBodyHolder.rotation.x, -6.3f, currentBodyHolder.rotation.z, 0f)) as GameObject;
			currentHeadHolder = currentBody.transform.FindDeepChild ("def_head 1");
			currentHead = Instantiate (Resources.Load ("Prefabs/" + speciesType + "_head_" + headType + "_f"), currentHeadHolder.position, new Quaternion (currentBodyHolder.rotation.x, -6.3f, currentBodyHolder.rotation.z, 0f)) as GameObject;
		} else {
			currentBody = Instantiate (Resources.Load ("Prefabs/" + bodyType + "_torso", typeof(GameObject)), currentBodyHolder.position, new Quaternion (currentBodyHolder.rotation.x, -6.3f, currentBodyHolder.rotation.z, 0f)) as GameObject;
			currentHeadHolder = currentBody.transform.FindDeepChild ("def_head 1");
			currentHead = Instantiate (Resources.Load ("Prefabs/" + speciesType + "_head_" + headType), currentHeadHolder.position, new Quaternion (currentBodyHolder.rotation.x, -6.3f, currentBodyHolder.rotation.z, 0f)) as GameObject;
		}
		/*foreach (Renderer rends in currentHead.GetComponentsInChildren<Renderer> ()) {
			if (rends.CompareTag ("ColoredHeadPart"))
				rends.GetComponent<Renderer> ().material.color = ColorParser.ParseColor (PlayerPrefs.GetString ("headColorPref"));
		}
		foreach (Renderer rends in currentBody.GetComponentsInChildren<Renderer> ()) {
			if (rends.CompareTag ("ColoredBodyPart"))
				rends.GetComponent<Renderer> ().material.color = ColorParser.ParseColor (PlayerPrefs.GetString ("bodyColorPref"));
		}
		foreach (Renderer rends in currentLegs.GetComponentsInChildren<Renderer> ()) {
			if (rends.CompareTag ("ColoredLegPart"))
				rends.GetComponent<Renderer> ().material.color = ColorParser.ParseColor (PlayerPrefs.GetString ("legColorPref"));
		}*/
		toonNameText.text = PlayerPrefs.GetString ("toonNamePref");

		currentLegs.transform.parent = this.transform;
		currentBody.transform.parent = currentBodyHolder;
		currentHead.transform.parent = currentHeadHolder;

		float cameraY = 1.25f;
		float cameraZ = -12f;

		if (PlayerPrefs.GetInt ("bodyTypePref") == 0)
			cameraY = 1.5f;
		else if (PlayerPrefs.GetInt ("bodyTypePref") == 1)
			cameraY = 1.75f;
		else if (PlayerPrefs.GetInt ("bodyTypePref") == 2)
			cameraY = 1.8f;
			cameraZ = -12.5f;

		Camera.main.transform.localPosition = new Vector3 (0f, cameraY, cameraZ);
	}
}
