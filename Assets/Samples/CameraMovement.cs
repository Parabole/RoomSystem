using UnityEngine;

public class CameraMovement : MonoBehaviour {
     
	public float lookSpeedH = 2f;
	public float lookSpeedV = 2f;
	public float zoomSpeed = 2f;
	public float moveSpeed = 6f;
	public float moveAcceleration = 1;
	
	private float yaw = 0f;
	private float pitch = 0f;
	private Vector3 currentVelocity = new Vector3();
         
	void Update ()
	{
		yaw += lookSpeedH * Input.GetAxis("Mouse X");
		pitch -= lookSpeedV * Input.GetAxis("Mouse Y");
     
		transform.eulerAngles = new Vector3(pitch, yaw, 0f);

		Vector3 scrollDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Vector3.forward;
		
		float xInput = 0;
		float zInput = 0;
		
		if (Input.GetKey(KeyCode.A))
		{
			xInput -= moveSpeed;
		}

		if (Input.GetKey(KeyCode.D))
		{
			xInput += moveSpeed;
		}
		
		if (Input.GetKey(KeyCode.W))
		{
			zInput += moveSpeed;
		}

		if (Input.GetKey(KeyCode.S))
		{
			zInput -= moveSpeed;
		}

		var velocityDelta = moveAcceleration * Time.deltaTime;
		var targetVelocity = new Vector3(xInput, 0, zInput);
		if (Input.GetKey(KeyCode.LeftShift))
		{
			targetVelocity *= 3;
		}
		currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, velocityDelta);
		transform.Translate(currentVelocity * Time.deltaTime + scrollDelta, Space.Self);
	}
}