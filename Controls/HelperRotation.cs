using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperRotation : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		//Automatic rotation
		transform.eulerAngles = transform.eulerAngles + new Vector3 (0, 1, 0);

		//We get the joystic's arrow's orientation to modify the target's position usando trig, knowing that the shooting distance is aprox 4.9
		Vector3 position = transform.position;

		position.x = transform.parent.position.x + Mathf.Sin (transform.parent.eulerAngles.y / (180 / Mathf.PI))*4.9f;
		position.z = transform.parent.position.z + Mathf.Cos (transform.parent.eulerAngles.y / (180 / Mathf.PI))*4.9f;

		transform.position = position;

		
	}
}
