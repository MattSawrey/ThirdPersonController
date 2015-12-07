using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public static float rsXAxisInput{get; private set;}
	public static float rsZAxisInput{get; private set;}

	public static float lsXAxisInput{get; private set;}
	public static float lsZAxisInput{get; private set;}

	public delegate void InputControl();
	public static event InputControl characterMove;
	public static event InputControl cameraMove;
	public static event InputControl jump;
	public static event InputControl interact;

	void FixedUpdate()
	{
		lsXAxisInput = Input.GetAxis("Horizontal");
		lsZAxisInput = Input.GetAxis("Vertical");

		rsXAxisInput = Input.GetAxis("360_rsHoriz");
		rsZAxisInput = Input.GetAxis("360_rsVert");

		//Joystick inputs
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
		{
			if(characterMove != null){characterMove();}
		}

		if(Input.GetAxis("360_rsHoriz") != 0f || Input.GetAxis("360_rsVert") != 0f)
		{
			if(cameraMove != null){cameraMove();}
		}
	
		if(Input.GetButtonDown("360_AButton"))
		{
			if(jump != null){jump();}
		}
		if(Input.GetButtonDown("360_XButton"))
		{
			if(interact != null){interact();}
		}
	}
}
