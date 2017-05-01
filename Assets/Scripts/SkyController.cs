using UnityEngine;
using System.Collections;

public class SkyController : MonoBehaviour {

	public float secondsInFullDay = 120f; [Range(0,1)]
	public float currentTimeOfDay = 0;

	private float timeMultiplier = 1f;

	void Update() {
		currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

		float intensityMultiplier = 1;
		if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
			intensityMultiplier = 0;
		else if (currentTimeOfDay <= 0.25f)
			intensityMultiplier = Mathf.Clamp01 ((currentTimeOfDay - 0.23f) * (1 / 0.02f));
		else if (currentTimeOfDay >= 0.73f)
			intensityMultiplier = Mathf.Clamp01 (1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));

		RenderSettings.ambientEquatorColor = new Color (intensityMultiplier + 0.1f, intensityMultiplier + 0.1f, intensityMultiplier + 0.1f);
		RenderSettings.ambientSkyColor = new Color (intensityMultiplier + 0.1f, intensityMultiplier + 0.1f, intensityMultiplier + 0.1f);

		if (currentTimeOfDay >= 1) {
			currentTimeOfDay = 0;
		}
	}
}
