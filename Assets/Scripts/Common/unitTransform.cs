using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UnitTransform : MonoBehaviour
{
	public Vector3 positionFeet;
	public Vector3 scaleFeet;
    
    private void OnEnable()
    {
        positionFeet = transform.localPosition * 5280;
        scaleFeet = transform.localScale * 5280;
    }

    // Update is called once per frame
	void Update ()
	{
		positionFeet = transform.localPosition * 5280;
		scaleFeet = transform.localScale * 5280;
	}

	private void OnValidate()
	{
		transform.localPosition = positionFeet / 5280;
		transform.localScale = scaleFeet / 5280;
	}
}
