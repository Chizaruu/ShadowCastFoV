using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smooth_Camera : MonoBehaviour 
{
	public Transform target;
	public float smoothSpeed = 0.225f;
	public Vector3 offset;

	void FixedUpdate()
	{
		Vector3 desiredPosition = target.position + offset;
		desiredPosition.z = -9;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;
		//transform.LookAt(target);
		//transform.rotation = Quaternion.Euler(0,0,0);
	}
}
