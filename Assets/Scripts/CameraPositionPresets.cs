using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[ExecuteInEditMode]
public class CameraPositionPresets : MonoBehaviour
{

    public Camera cam;
    public AnimationCurve ac = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Serializable]
    public class Preset
    {
        public Vector3 pos;
        public Quaternion rot;
        public float hFov;
    }

    public List<Preset> presets = new List<Preset>();

    public int selectedPreset = 0;

    [Range(.01f, 4f)]
    public float animationSpeed = 1;

    private void OnEnable()
    {
        cam = GetComponentInChildren<Camera>();

        if (presets.Count < 1)
        {
            presets.Add(new Preset()
            {
                rot = transform.rotation,
                pos = transform.position,
                hFov = cam.fieldOfView
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedPreset++;
            selectedPreset %= presets.Count;

            AnimateToPreset(selectedPreset);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedPreset = selectedPreset - 1 < 0 ? presets.Count - 1 : selectedPreset - 1;
            AnimateToPreset(selectedPreset);
        }
    }

    public async void AnimateToPreset(int index)
    {

        var t = 0.0f;
        var startpos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / animationSpeed;

            var dt = ac.Evaluate(Mathf.Clamp01(t));

            transform.position = Vector3.Lerp(startpos, presets[index].pos, dt);

            await new WaitForEndOfFrame();
        }
    }
}
