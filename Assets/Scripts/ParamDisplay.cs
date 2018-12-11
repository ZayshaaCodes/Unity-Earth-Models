using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ParamDisplay : MonoBehaviour {

	[System.Serializable]
	public class Param
	{
		public string prefix;

		public delegate string DisplayFunction();
		public DisplayFunction displayFunction;

        //for display and access to the last evaluated value;
		public string currentValue;

		public TextMeshProUGUI textUi;

	}

	public List<Param> displayParams = new List<Param>();

	public GameObject textUiPrefab;

	// Use this for initialization
	void OnEnable () {

		displayParams.Clear();
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			if (!Application.isPlaying)
				DestroyImmediate(transform.GetChild(i).gameObject);
			else
				Destroy(transform.GetChild(i).gameObject);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

	}
	
	// Update is called once per frame
	void Update () {
		foreach (Param v in displayParams)
		{
		    v.currentValue = v.displayFunction?.Invoke();
			v.textUi.text = v.prefix + (v.displayFunction?.Invoke() ?? "null");
		}
	}

	public void AddParam(Param param)
	{
		param.textUi = Instantiate(textUiPrefab, transform).GetComponent<TextMeshProUGUI>();
		displayParams.Add(param);

		LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
	}
}
