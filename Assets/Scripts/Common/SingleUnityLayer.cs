using UnityEngine;

[System.Serializable]
public class SingleUnityLayer
{
    [SerializeField]
    private int _layerIndex;
    public int LayerIndex => _layerIndex;

    public void Set(int layerIndex)
    {
        if (layerIndex > 0 && layerIndex < 32)
        {
            _layerIndex = layerIndex;
        }
    }

    public int Mask => 1 << _layerIndex;
}