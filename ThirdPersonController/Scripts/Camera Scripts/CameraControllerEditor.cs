using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor 
{
	CameraController camController;
	string rangeString;
	
	void OnEnable()
	{
		camController = (CameraController)target;
	}

	public override void OnInspectorGUI()
	{
		camController.pointOfFocus = (GameObject)EditorGUILayout.ObjectField("Point of focus", camController.pointOfFocus,
		                                                                     typeof(object), true);

		camController.groundLayerMask = EditorGUILayout.LayerField("Ground Layer Mask", camController.groundLayerMask);

		GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(1f));

		//Camera Type

		EditorGUILayout.LabelField("Camera Type", EditorStyles.boldLabel);
		
		camController.staticDistanceCam = EditorGUILayout.Toggle("Static Distance", camController.staticDistanceCam);
		if(!camController.staticDistanceCam)
		{
			camController.camHeightCurve = EditorGUILayout.CurveField("Cam Height",
			                                                          camController.camHeightCurve);			
			camController.camDistanceCurve = EditorGUILayout.CurveField("Cam Distance",
			                                                            camController.camDistanceCurve);			
			camController.camHeightFocusWithDistanceCurve = EditorGUILayout.CurveField("Cam Focus w/Dist",
			                                                                           camController.camHeightFocusWithDistanceCurve);
			rangeString = "Min Range";
		}
		
		else if(camController.staticDistanceCam)
		{
			rangeString = "Cam Range";
			
			camController.heightDistOffsetMin = EditorGUILayout.Slider("Height Dist Min", camController.heightDistOffsetMin, -0.99f, 0f);
			camController.heightDistOffsetMax = EditorGUILayout.Slider("Height Dist Max", camController.heightDistOffsetMax, 0f, 0.99f);
		}

		GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(1f));

		//Camera Follow

		camController.camFollow = EditorGUILayout.Toggle("Cam Follow", camController.camFollow);

		if(camController.camFollow)
		{
			EditorGUILayout.LabelField("Restrict Cam Follow To: ", EditorStyles.boldLabel);
		
			camController.onlyFollowWhileMoving = EditorGUILayout.Toggle("Player Movement", camController.onlyFollowWhileMoving);
			camController.onlyFollowWhileBehind = EditorGUILayout.Toggle("Behind Player", camController.onlyFollowWhileBehind);

			camController.followDampeningValue = EditorGUILayout.Slider("Follow Dampening Value", camController.followDampeningValue, 0f, 100f);
			camController.followMaxRotSpeed = EditorGUILayout.FloatField("Follow Max Rot Speed", camController.followMaxRotSpeed);
		}

		GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(1f));

		//Camera Variables

		camController.minRange = EditorGUILayout.FloatField(rangeString, camController.minRange);
		camController.minHeight = EditorGUILayout.FloatField("Min Height", camController.minHeight);

		camController.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", camController.rotationSpeed);
		camController.distanceSpeed = EditorGUILayout.FloatField("Distance Speed", camController.distanceSpeed);

		camController.minRSSensitivity = EditorGUILayout.Slider("Min RS Sensitivity", camController.minRSSensitivity, 0f, 1f);

		GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(1f));

		GUILayout.Label("Wall Compensation Variables");

		camController.increaseWallCheckDistance = EditorGUILayout.Slider("% ^Dist F/Wall Check", camController.increaseWallCheckDistance, 0f, 1f);

		if(GUI.changed)
		{
			EditorUtility.SetDirty(camController);
			camController.InitialiseCameraSetup();
		}
	}
}

public static class MyGUIStyles
{
	private static GUIStyle m_line = null;
	
	//constructor
	static MyGUIStyles()
	{		
		m_line = new GUIStyle("box");
		m_line.border.top = m_line.border.bottom = 1;
		m_line.margin.top = m_line.margin.bottom = 1;
		m_line.padding.top = m_line.padding.bottom = 1;
	}
	
	public static GUIStyle EditorLine
	{
		get { return m_line; }
	}
}
