using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class unitTransform : MonoBehaviour {

	public Vector3 positionFeet;
	public Vector3 scaleFeet;
	
	
	// Update is called once per frame
	void Update ()
	{
		positionFeet = transform.position * 5280;
		scaleFeet = transform.localScale * 5280;
	}

	private void OnValidate()
	{
		transform.position = positionFeet / 5280;
		transform.localScale = scaleFeet / 5280;
	}
}
