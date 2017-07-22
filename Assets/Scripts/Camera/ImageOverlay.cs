using UnityEngine;
using System.Collections;

public class ImageOverlay : MonoBehaviour
{

    public bool enabled = false;
    public Material mat;
    public Texture2D image;

    [Range(0, 1)]
    public float opacity = .5f;

    public GameObject followObjectScreenspace;

    [Range(0, 1)]
    public float sizeX = 1;
    [Range(0, 1)]
    public float sizeY = 1;
    [Range(0, 1)]
    public float posX = 0;
    [Range(0, 1)]
    public float posY = 0;

    public bool lockImageAspect = true;

    public Rect overlayArea = Rect.zero;


    [ExecuteInEditMode]
    void OnPostRender()
    {

        var cam = Camera.main;

        if (followObjectScreenspace)
        {
            cam.WorldToScreenPoint(followObjectScreenspace.transform.position);
            

        }

        if (!enabled)
        {
            return;
        }

        if (!mat)// || !image)
        {
            Debug.LogError("Please Assign a material and image in the inspector");
            return;
        }

        float aspect = (float)image.width / image.height;
        float screenAspect = (float)Screen.width / Screen.height;

        mat.SetTexture("_MainTex", image);
        mat.SetFloat("_Opacity", opacity);

        if (lockImageAspect)
        {
            //sizeY = sizeX / aspect;
        }

        float left = posX * Screen.width - image.width / 2 * sizeX;
        float right = posX * Screen.width + image.width / 2 * sizeX;
        float bottom = posY * Screen.height - image.height / 2 * sizeX;
        float top = posY * Screen.height + image.height / 2 * sizeX;

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