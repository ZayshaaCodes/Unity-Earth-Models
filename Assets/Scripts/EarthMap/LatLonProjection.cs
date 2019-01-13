using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Cookie042.EarthHeightmap
{
    [ExecuteInEditMode, SelectionBase]
    public class LatLonProjection : MonoBehaviour
    {
        public int blockDivisions = 16;

        public double radiusEquator = 39.59;
        public double elevationScale = 1.0;

        public Material surfaceMaterial;

        //position
        [Range(-90, 90)] public int latitude;
        [Range(-180, 180)] public int longitude;

        public bool centerAtCoords;

        [Serializable] public class GizmoSettings
        {
            public bool latLonLines = false;
            public Color latLonLineColor = new Color(1f, 0.92f, 0.02f, 0.2f);
            public bool paths= false;
            public Color pathsColor = new Color(1f, 0.92f, 0.02f, 0.1f);
            public bool pivotAxis = false;
            public float pivotAxisAlpha = .2f;
            public bool selectedBlocks = false;
            public Color selectedBlocksColor = new Color(0f, 1f, 0f, 0.2f);
            public bool selectedUnloadedChunks = false;
            public Color selectedUnloadedChunksColor= new Color(1f, 0f, 1f, 0.2f);
        }

        public GizmoSettings gizmoSettings = new GizmoSettings();

        public bool AutoScaleForLocalLatitude = true;

        //! Public Hidden Fields
        [HideInInspector]
        public Chunk activeChunk;

        public Texture2D equirectangularProjectionMap;

        //! Private Fields
        private double _heightUnitScale;
        private double _circumference;
        private double _halfCir;
        private double _degreeSize;
        private double _chunkSize;
        private EarthHeightBlockData _earthData;

        private readonly SelectionTree _selectionTree = new SelectionTree();

        //! State Memory
        private double _lastRad;

        //! MonoBehaviour Methods
        private void OnValidate()
        {
            //if the radius changes, update.
            if (Math.Abs(_lastRad - radiusEquator) > float.Epsilon)
            {
                SetRadius(radiusEquator);
                _lastRad = radiusEquator;
                _heightUnitScale = radiusEquator / 6371000;
            }
        }

        //? UPDATE
        private void Update()
        {
            if (AutoScaleForLocalLatitude)
            {
                var s = transform.localScale;
                s.x = transform.localScale.z * Mathf.Cos(latitude * Mathf.Deg2Rad);
                transform.localScale = s;
            }

            //
            if (surfaceMaterial)
            {
                foreach (var block in _earthData.LoadedBlockIterator())
                {
                    Vector3 blockPos = new Vector3((float)(block.Key.x * _degreeSize), 0, (float)(block.Key.y * _degreeSize));

                    if (centerAtCoords)
                    {
                        blockPos -= new Vector3((float)(_degreeSize * longitude), 0, (float)(_degreeSize * latitude));
                    }

                    foreach (var chunk in block.Value.chunks)
                    {
                        Vector3 chunkPos = new Vector3((float)(chunk.Key % blockDivisions * _chunkSize), 0, (float)(chunk.Key / blockDivisions * _chunkSize));
                        if (chunk.Value.mesh != null)
                        {
                            //Gizmos.color = new Color(0.71f, 0.71f, 0.71f, 1f);
                            Graphics.DrawMesh(chunk.Value.mesh,
                                transform.localToWorldMatrix
                                * Matrix4x4.TRS(
                                    blockPos + chunkPos,
                                    Quaternion.identity,
                                    new Vector3((float)_chunkSize, (float)(elevationScale * _heightUnitScale), (float)_chunkSize)),
                                surfaceMaterial, 0);


                            //Gizmos.DrawMesh(chunk.Value.mesh, blockPos + chunkPos, Quaternion.identity, new Vector3((float)angleStep2, (float)elevationScale, (float)angleStep2));
                        }

                    }
                }
            }

            // Update Selection
            _selectionTree.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var cpInfo = GetChunkPosInfo(child.position);

                var childchild = child.childCount > 0 ? child.GetChild(0) : null;
                if (childchild)
                {
                    if (gizmoSettings.paths)
                        Debug.DrawLine(child.position, childchild.position, gizmoSettings.pathsColor);

                    var lp1 = GetLocalPosition(child.position);
                    var lp2 = GetLocalPosition(childchild.position);

                    var points = new List<Vector3>();

                    var dlp = lp2 - lp1;
                    int c;
                    float step;
                    Vector3 pdir;

                    //print(dlp);
                    if (Mathf.Abs(dlp.x) > Mathf.Abs(dlp.z))
                    {
                        c = Mathf.RoundToInt((float)(Mathf.Abs(dlp.x) / _chunkSize));
                        step = (dlp.magnitude / c);
                        pdir = Vector3.forward * (float)_chunkSize;
                    }
                    else
                    {
                        c = Mathf.RoundToInt((float)(Mathf.Abs(dlp.z) / _chunkSize));
                        step = (dlp.magnitude / c);
                        pdir = Vector3.right * (float)_chunkSize;
                    }

                    for (int j = 0; j <= c; j++)
                    {
                        points.Add(transform.localToWorldMatrix.MultiplyPoint(lp1 + dlp.normalized * step * j));
                        points.Add(transform.localToWorldMatrix.MultiplyPoint(lp1 + dlp.normalized * step * j + pdir));
                        points.Add(transform.localToWorldMatrix.MultiplyPoint(lp1 + dlp.normalized * step * j - pdir));
                        points.Add(transform.localToWorldMatrix.MultiplyPoint(lp1 + dlp.normalized * step * j + pdir * 2));
                        points.Add(transform.localToWorldMatrix.MultiplyPoint(lp1 + dlp.normalized * step * j - pdir * 2));
                    }

                    foreach (Vector3 point in points)
                    {
                        var info = GetChunkPosInfo(point, true);
                        _selectionTree.Select(new Vector2Int(info.lon, info.lat), new Vector2Int(info.cx, info.cy));
                    }

                }
                else
                {
                    _selectionTree.Select(new Vector2Int(cpInfo.lon, cpInfo.lat), new Vector2Int(cpInfo.cx, cpInfo.cy));
                }

            }
        }

        public ChunkPosInfo GetChunkPosInfo(Vector3 WorldPos, bool centerOffset = false)
        {
            var localPos = transform.worldToLocalMatrix.MultiplyPoint(WorldPos);

            int lo = Mathf.FloorToInt((float)(localPos.x / _degreeSize));
            int la = Mathf.FloorToInt((float)(localPos.z / _degreeSize));

            var subPos = localPos - new Vector3((float)(lo * _degreeSize), 0, (float)(la * _degreeSize));

            int x = Mathf.FloorToInt((float)(subPos.x / _chunkSize));
            int y = Mathf.FloorToInt((float)(subPos.z / _chunkSize));

            return centerOffset 
                ? new ChunkPosInfo(lo + longitude, la + latitude, x, y) 
                : new ChunkPosInfo(lo, la, x, y);
        }

        public Vector3 GetLocalWorldPos(Vector3 worldPos, bool centerOffset = false)
        {
            var localPos = transform.worldToLocalMatrix.MultiplyPoint(worldPos);

            float lo = (float)(localPos.x / _degreeSize);
            float la = (float)(localPos.z / _degreeSize);
            float el = (float)(localPos.y / (_heightUnitScale * elevationScale));
            return centerOffset
                ? new Vector3(lo + longitude, la + latitude, el)
                : new Vector3(lo, la, el);

        }

        public Vector3 GetLocalPosition(Vector3 worldPosition)
        {
            return transform.worldToLocalMatrix.MultiplyPoint(worldPosition);
        }

        //? OnEnable
        private void OnEnable()
        {
            _earthData = new EarthHeightBlockData(Path.Combine(Application.persistentDataPath, "mapData\\"));
        }

        //! Set Methods
        public void SetRadius(double rad)
        {
            radiusEquator = rad;
            _circumference = rad * Math.PI * 2;
            _halfCir = _circumference / 2;
            _degreeSize = _circumference / 360;
            _chunkSize = _degreeSize / blockDivisions;
        }

        //Note: a chunk can be loaded without the whole blocks _earthData being loaded

        public IEnumerator LoadSelection()
        {
            var sel = _selectionTree.GetSelectedChunks();

            foreach (Selection selection in sel)
            {
                yield return StartCoroutine(_earthData.GetChunk(selection.block, selection.chunk, c => c.BuildMesh()));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            if (gizmoSettings.pivotAxis)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
                Gizmos.DrawLine(Vector3.zero, Vector3.right * (float)_degreeSize);

                Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
                Gizmos.DrawLine(Vector3.zero, Vector3.up * (float)_degreeSize);

                Gizmos.color = new Color(0f, 0f, 1f, 0.1f);
                Gizmos.DrawLine(Vector3.zero, Vector3.forward * (float)_degreeSize);
            }

            Vector3 blCornerPosition = new Vector3((float)-_halfCir, 0, (float)(-_halfCir / 2));

            if (centerAtCoords)
                blCornerPosition -= new Vector3((float)(_degreeSize * longitude), 0, (float)(_degreeSize * latitude));

            if (gizmoSettings.latLonLines)
            {
                Gizmos.color = gizmoSettings.latLonLineColor;

                //! lonLines
                for (int i = 0; i <= 360; i++)
                {
                    Gizmos.DrawLine(
                        blCornerPosition + new Vector3((float)(_degreeSize * i), 0, 0),
                        blCornerPosition + new Vector3((float)(_degreeSize * i), 0, (float)_halfCir));
                }

                //! LatLines
                for (int i = 0; i <= 180; i++)
                {
                    Gizmos.DrawLine(
                        blCornerPosition + new Vector3(0, 0, (float)(_degreeSize * i)),
                        blCornerPosition + new Vector3((float)_circumference, 0, (float)(_degreeSize * i)));
                }
            }

            if (gizmoSettings.selectedBlocks)
            {
                var selBlocks = _selectionTree.GetSelectedBlocks();
                Gizmos.color = gizmoSettings.selectedBlocksColor;

                foreach (Vector2Int v2i in selBlocks)
                    Gizmos.DrawWireCube(GetBlockLocalCenter(v2i),
                        new Vector3((float) _degreeSize, 0, (float) _degreeSize));
            }

            if (gizmoSettings.selectedUnloadedChunks)
            {
                var selChunks = _selectionTree.GetSelectedChunks();
                Gizmos.color = gizmoSettings.selectedUnloadedChunksColor;

                foreach (Selection selChunk in selChunks)
                    if (!_earthData.IsChunkLoaded(selChunk.block, selChunk.chunk))
                        Gizmos.DrawCube(
                            GetChunkLocalCenter(selChunk.block, selChunk.chunk),
                            new Vector3((float) _chunkSize, 0, (float) _chunkSize));
            }
        }

        public Vector3 GetBlockLocalCenter(Vector2Int block)
        {
            return new Vector3(
                (float)((block.x - longitude) * _degreeSize + _degreeSize / 2), 0,
                (float)((block.y - latitude) * _degreeSize + _degreeSize / 2));
        }
        public Vector3 GetChunkLocalCenter(Vector2Int block, Vector2Int chunk)
        {
            return new Vector3(
                (float)((block.x - longitude) * _degreeSize + chunk.x * _chunkSize + _chunkSize / 2), 0,
                (float)((block.y - latitude) * _degreeSize + chunk.y * _chunkSize + _chunkSize / 2));
        }
    }

    public class ChunkPosInfo
    {
        public int lon;
        public int lat;
        public int cx;
        public int cy;

        public ChunkPosInfo(int lon, int lat, int cx, int cy)
        {
            this.lon = lon;
            this.lat = lat;
            this.cx = cx;
            this.cy = cy;
        }
    }
}
