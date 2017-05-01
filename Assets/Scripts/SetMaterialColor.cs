using UnityEngine;

public class SetMaterialColor : MonoBehaviour {

	public Color _color;
	public string materialReplacement;

	void Start () {
		foreach (Material mat in GetComponent<Renderer> ().materials) {
			if (mat.name == materialReplacement)
				mat.color = _color;
		}
	}
}