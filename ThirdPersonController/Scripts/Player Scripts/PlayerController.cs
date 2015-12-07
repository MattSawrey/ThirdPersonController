using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Transform groundCheckPos;
	public LayerMask groundLayer;

	public float maxSpeed;
	public float jumpForce;

	private float moveSpeed;
	private float distanceConstrainer;

	private float rotationAngleInput;
	private float rsInputAngle;
	private float rotationSpeed = 8f;

	private Camera mainCam;

	private Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Awake () 
	{
		mainCam = Camera.main;

		InputManager.characterMove += CharacterMover;
	}

	void OnDestroy()
	{
		InputManager.characterMove -= CharacterMover;
	}

	private void CharacterMover()
	{
		//Rotation
		rsInputAngle = Mathf.Atan2(0f - InputManager.lsXAxisInput, 0f - InputManager.lsZAxisInput);	
		rotationAngleInput = -rsInputAngle.ConvertBetweenRanges(Mathf.PI, -Mathf.PI, 0f, 360f) 
			+ mainCam.transform.rotation.eulerAngles.y;

		gameObject.transform.rotation = Quaternion.Lerp(transform.rotation,
		                                                Quaternion.Euler(new Vector3(0f, rotationAngleInput, 0f)),
		                								Mathf.Lerp(0f, 1f, Time.deltaTime * rotationSpeed));

		//Movement
		moveDirection = transform.TransformDirection(new Vector3(InputManager.lsXAxisInput, 0f, InputManager.lsZAxisInput));
		distanceConstrainer = Mathf.Clamp(Vector3.Distance(Vector3.zero, moveDirection), 0.1f, 0.9f);
		moveSpeed = (distanceConstrainer * maxSpeed);
		gameObject.transform.Translate(new Vector3(0f, 0f, moveSpeed));
	}
}
