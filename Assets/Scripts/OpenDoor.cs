using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenDoor : MonoBehaviour {

	public Transform firstPoint;
	public Transform secondPoint;
	public Animator linkedAnimator;
	public string sceneName;

	private float timer = 0f;

	public void SendToScene () {
		SceneManager.LoadScene (sceneName);
	}

	public IEnumerator DoorLerp (Transform p_transform,float _speed) {
		linkedAnimator.SetTrigger ("openDoor");

		while (timer < 3f) {
			timer += Time.deltaTime * _speed;
			p_transform.rotation = Quaternion.Lerp (p_transform.rotation, firstPoint.rotation, timer);
			p_transform.position = Vector3.Lerp (p_transform.position, firstPoint.position, timer);
			if (p_transform.position == firstPoint.position) {
				timer = 0f;
				break;
			}
		}
		while (timer < 3f) {
			timer += Time.deltaTime * _speed;
			p_transform.position = Vector3.Lerp (p_transform.position, secondPoint.position, timer);
			if (p_transform.position == secondPoint.position) {
				SendToScene ();
			}
		}
		yield return null;
	}
}