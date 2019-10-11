using UnityEngine;

public class CameraController : MonoBehaviour {

	#region Basic Variables

	public GameObject pointOfFocus;

	public LayerMask groundLayerMask;

	//movement curves for the height and the distance of the camera
	public AnimationCurve camHeightCurve;
	public AnimationCurve staticCamHeightCurve;

	public AnimationCurve camDistanceCurve;
	public AnimationCurve staticCamDistanceCurve;

	public AnimationCurve camHeightFocusWithDistanceCurve;

	//calue that the Height and Distance animation curves will be evaluated with, 0 - 1;
	private float heightDistOffset = 0f;

	private Vector3 offset;

	public float minRange;
	public float minHeight;

	public float distanceSpeed;

	public float rotationSpeed;
	private float xRotationOffset;

	public float heightDistOffsetMin;
	public float heightDistOffsetMax;

	public float minRSSensitivity;

	public bool staticDistanceCam;

	public bool camFollow;
	public bool onlyFollowWhileMoving;
	public bool onlyFollowWhileBehind;
	private Vector3 followRotPosition;
	public float followDampeningValue;
	public float followMaxRotSpeed = 20f;	

	public float increaseWallCheckDistance;
	public float minDistanceToPlayer;

	#endregion

	#region Basic Setup and Movement

	public void InitialiseCameraSetup()
	{
		if(!staticDistanceCam)
		{
			//start the camera at the minimum range behind the pointOfFocus
			offset = new Vector3(0f,
			                     camHeightCurve.Evaluate(heightDistOffset) + minHeight,
			                     camDistanceCurve.Evaluate(heightDistOffset) + minRange);
		}
		else if(staticDistanceCam)
		{
			//Reassign the camheight curve and the cam distance curve based on the minimum range specified
			staticCamDistanceCurve = new AnimationCurve(new Keyframe(-1, minRange, 1, -(minRange*3)),
			                                            new Keyframe(0, 0, 0, 0),
			                                            new Keyframe(1, minRange, (minRange*3), 1));			
			staticCamHeightCurve = AnimationCurve.Linear(-1, -(minRange), 1, (minRange));

			//start the camera at the minimum range behind the pointOfFocus
			offset = new Vector3(0f,
			                     staticCamHeightCurve.Evaluate(heightDistOffset) + minHeight,
			                     staticCamDistanceCurve.Evaluate(heightDistOffset) + minRange);
		}
		
		//When the pointOfFocus inputs are correct to move the camera
		InputManager.rightJoystickActive += MoveCamera;

		//Assign the follow code to when the player is moving
		if(camFollow && onlyFollowWhileMoving)
		{
			InputManager.leftJoystickActive += FollowPointOfFocus;
		}
	}
	//Only gets called when pointOfFocus messes with left stick
	private void MoveCamera()
	{
		//check that the pointOfFocus input is strong enough to warrant cam movement on dist and height
		if(!InputManager.Instance.rightJoystickInputValue.y.IsBetween(-minRSSensitivity, minRSSensitivity))
		{
			if(!staticDistanceCam)
			{
				heightDistOffset = Mathf.Clamp(heightDistOffset + ((InputManager.Instance.rightJoystickInputValue.y/100f) * distanceSpeed), 0f, 1f);

				offset = new Vector3(0f,
				                     camHeightCurve.Evaluate(heightDistOffset) + minHeight,
				                     (camDistanceCurve.Evaluate(heightDistOffset)) + minRange);
			}
			else if(staticDistanceCam)
			{
				heightDistOffset = Mathf.Clamp(heightDistOffset + ((InputManager.Instance.rightJoystickInputValue.y/100f) * distanceSpeed), heightDistOffsetMin, heightDistOffsetMax);

				offset = new Vector3(0f,
				                     staticCamHeightCurve.Evaluate(heightDistOffset) + minHeight,
				                     (-staticCamDistanceCurve.Evaluate(heightDistOffset)) + minRange);
			}
		}

		//check that the pointOfFocus input is strong enough to warrant cam movement on xRotationOffset
		if(!InputManager.Instance.rightJoystickInputValue.x.IsBetween(-minRSSensitivity, minRSSensitivity))
		{
			xRotationOffset += InputManager.Instance.rightJoystickInputValue.x * rotationSpeed;
		}
	}
		
	#endregion

	#region Wall Comp and Follow

	private void CompForWalls()
	{
		Vector3 increaseCheckDistance = ((pointOfFocus.transform.position - transform.position) * increaseWallCheckDistance);

		Debug.DrawLine(transform.position - increaseCheckDistance, pointOfFocus.transform.position, Color.cyan);	

		RaycastHit wallHit = new RaycastHit();
		if(Physics.Linecast(pointOfFocus.transform.position, transform.position - increaseCheckDistance, out wallHit, 1<<groundLayerMask))
		{
			Debug.DrawRay(wallHit.point, Vector3.left, Color.red);

			Vector3 distToMoveCam = (pointOfFocus.transform.position - wallHit.point) * increaseWallCheckDistance;

			Vector3 newPositionForWallComp = wallHit.point + distToMoveCam;

			if(!staticDistanceCam)
			{
				transform.position = new Vector3(newPositionForWallComp.x, transform.position.y, newPositionForWallComp.z);
			}
			else
			{
				transform.position = newPositionForWallComp;
			}
			Debug.DrawLine(pointOfFocus.transform.position, new Vector3(newPositionForWallComp.x, transform.position.y, newPositionForWallComp.z), Color.green);
		}
	}

	private void FollowPointOfFocus()
	{
		if(camFollow)
		{
			Vector3 pointOfFocusDirection = pointOfFocus.transform.rotation * pointOfFocus.transform.forward;
			Vector3 cameraDirection = transform.rotation * pointOfFocus.transform.forward;
			
			float pointOfFocusAngle = Mathf.Atan2(pointOfFocusDirection.x, pointOfFocusDirection.z) * Mathf.Rad2Deg;
			float cameraAngle = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
			
			float rotationDiff = Mathf.Clamp(Mathf.DeltaAngle(cameraAngle, pointOfFocusAngle), -followMaxRotSpeed, followMaxRotSpeed);
			
			//			Debug.DrawRay(pointOfFocus.transform.position, 
			//			              transform.position.RotateVector3AroundPivot(pointOfFocus.transform.position, new Vector3(0f, rotationDiff, 0f)),
			//			              Color.green);

			if(onlyFollowWhileBehind)
			{
				//check whether player is facing camera
				bool isPlayerFacingCamera = Vector3.Dot(pointOfFocusDirection, cameraDirection) < 0f ? true : false;

				if(isPlayerFacingCamera)
					rotationDiff = 0f;
			}

			//Apply the rotation difference
			xRotationOffset += rotationDiff/followDampeningValue;
		}
	}

	#endregion

	#region Mono Functions

	void Awake()
	{
		InitialiseCameraSetup();
	}
	
	void OnDestroy()
	{
		InputManager.rightJoystickActive -= MoveCamera;
		if(camFollow && onlyFollowWhileMoving)
		{
			InputManager.leftJoystickActive -= FollowPointOfFocus;
		}
	}
	
	void LateUpdate()
	{
		if(camFollow && !onlyFollowWhileMoving)
		{
			FollowPointOfFocus();
		}
		
		transform.position = pointOfFocus.transform.position + offset;
		transform.RotateAround(pointOfFocus.transform.position, Vector3.up, xRotationOffset);
		
		CompForWalls();
		
		if(!staticDistanceCam)
		{
			transform.LookAt(pointOfFocus.transform.position + new Vector3(0f,
			                                                               camHeightFocusWithDistanceCurve.Evaluate(heightDistOffset),
			                                                               0f));
		}
		else if(staticDistanceCam)
		{
			transform.LookAt(pointOfFocus.transform.position);
		}
	}

	#endregion
}
