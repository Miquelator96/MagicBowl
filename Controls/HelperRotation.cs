using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//Rotación automática
		transform.eulerAngles = transform.eulerAngles + new Vector3 (0, 1, 0);


		//Cogemos orientación de la flecha para modificar posición de target usando trigonometria sabiendo que la distancia de disparo es aprox 4.9
		Vector3 position = transform.position;

		position.x = transform.parent.position.x + Mathf.Sin (transform.parent.eulerAngles.y / (180 / Mathf.PI))*4.9f;
		position.z = transform.parent.position.z + Mathf.Cos (transform.parent.eulerAngles.y / (180 / Mathf.PI))*4.9f;

		transform.position = position;

		
	}
}
