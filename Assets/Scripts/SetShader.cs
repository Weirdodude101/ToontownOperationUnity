using UnityEngine;

public class SetShader : MonoBehaviour {

	public Shader _shader;
	public string materialReplacement;

	void Start () {
		foreach (Material mat in GetComponent<Renderer> ().materials) {
			if (mat.name == materialReplacement)
				mat.shader = _shader;
		}
	}
}
