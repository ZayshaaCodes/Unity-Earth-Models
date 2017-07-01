using UnityEngine;
using System.Collections;

public class ImageOverlay : MonoBehaviour
{

    public bool enabled = false;
    public Material mat;
    public Texture2D image;

    [Range(0, 1)]
    public float sizeX = 1;
    [Range(0, 1)]
    public float sizeY = 1;
    [Range(-1, 1)]
    public float posX = 0;
    [Range(-1, 1)]
    public float posY = 0;

    public bool lockImageAspect = true;

    public Rect overlayArea = Rect.zero;


    [ExecuteInEditMode]
    void OnPostRender()
    {
        if (!enabled)
        {
            return;
        }

        if (!mat)// || !image)
        {
            Debug.LogError("Please Assign a material and image in the inspector");
            return;
        }

        float aspect = image.width / image.height;

        mat.SetTexture("_MainTex", image);

        float left = posX * Screen.width;
        float right = posX * Screen.width + image.width * sizeX;
        float bottom = posY * Screen.height;
        float top = posY * Screen.height + image.height * (lockImageAspect? sizeX / aspect : sizeY);

        //print(left + ", " + right + ", " + left + ", " + top + ", " + bottom);

        GL.PushMatrix();
        mat.SetPass(0);
        GL.Color(new Color(0,0, 0, .5f));
        GL.LoadPixelMatrix();
        //GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.TexCoord3(0, 0, 0);
        GL.Vertex3(left, bottom, 0);
        GL.TexCoord3(0, 1, 0);
        GL.Vertex3(left, top, 0);
        GL.TexCoord3(1, 1, 0);
        GL.Vertex3(right, top, 0);
        GL.TexCoord3(1, 0, 0);
        GL.Vertex3(right, bottom, 0);
        GL.End();
        GL.PopMatrix();
    }
}