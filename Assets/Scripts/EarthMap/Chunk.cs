using System;
using System.IO;
using UnityEngine;

namespace Cookie042.EarthHeightmap
{
    [System.Serializable]
    public class Chunk
    {
        [NonSerialized] public Mesh mesh;
        [NonSerialized] public Texture2D texture;

        public int min;
        public int max;

        public int size;

        public short[,] data;

        public Chunk(short[,] pointData, int minValue, int maxValue)
        {
            data = pointData;
            min = minValue;
            max = maxValue;
            size = Mathf.RoundToInt(Mathf.Sqrt(data.Length));
        }

        public void BuildTexture()
        {
            var tex = new Texture2D(size, size);

            Color[] pixelData = new Color[size * size];

            //Debug.Log($"min = {min}, max = {max}");

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float value = CookieMath.InverseLerpInt(data[x, (size - 1) - y], min, max);

                    pixelData[y * size + x] = Color.Lerp(Color.black, Color.white, value);
                }
            }

            tex.SetPixels(0, 0, size, size, pixelData);
            tex.Apply();

            texture = tex;
        }

        public void SaveTexture(string filename)
        {
            if (texture != null)
            {
                byte[] pngData = texture.EncodeToPNG();
                File.WriteAllBytes(filename, pngData);
            }
        }

        public void LoadTexture(string filename)
        {
            if (!File.Exists(filename))
                return;

            var filedata = File.ReadAllBytes(filename);

            texture.LoadImage(filedata);

        }

        public void BuildMesh()
        {
            Vector3[] points = new Vector3[size * size];
            int[] tris = new int[(size - 1) * (size - 1) * 6];

            double spacing = 1.0 / (size-1);

            short lastvalid = 0;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (data[x, y] > short.MaxValue - 10)
                        data[x, y] = lastvalid;
                    else
                        lastvalid = data[x, y];

                    points[y * size + x] = new Vector3((float) (x * spacing), data[x,y], (float) ((size - y) * spacing));
                }
            }

            int t = 0;
            for (int y = 0; y < size-1; y++)
            {
                for (int x = 0; x < size-1; x++)
                {
                    var si = y * size + x;

                    tris[t + 0] = si + size + 1;
                    tris[t + 1] = si + 0;
                    tris[t + 2] = si + 1;
                    tris[t + 3] = si + size + 1;
                    tris[t + 4] = si + size;
                    tris[t + 5] = si + 0;
                    t += 6;
                }
            }

            if (mesh == null)
                mesh = new Mesh();

            mesh.triangles = null;
            mesh.vertices = points;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}