using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

	private Vector3 moveDir = Vector3.zero;
	private CharacterController c_Controller;

	void Awake () {
		c_Controller = GetComponent<CharacterController> ();
	}

	void Update () {
		if (c_Controller.isGrounded)
			if (Input.GetButton ("Jump"))
				moveDir.y = 15f;

		if (Input.GetButton ("Vertical"))
			foreach (Animator anim in GetComponentsInChildren<Animator> ())
				anim.SetBool ("running", true);
		else
			foreach (Animator anim in GetComponentsInChildren<Animator> ())
				anim.SetBool ("running", false);

		moveDir = new Vector3 (0f, 0f, Input.GetAxis ("Vertical"));
		moveDir = transform.TransformDirection (moveDir);
		moveDir *= 18.5f;
		moveDir.y -= 1500.0f * Time.deltaTime;
		c_Controller.Move (moveDir * Time.deltaTime);
		transform.Rotate (0f, Input.GetAxis ("Horizontal") / 2f, 0f);
	}
}