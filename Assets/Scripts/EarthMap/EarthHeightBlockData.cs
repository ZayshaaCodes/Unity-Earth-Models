using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace Cookie042.EarthHeightmap
{
    public class EarthHeightBlockData
    {
        public int blockDivisions = 16;
        private readonly Dictionary<Vector2Int, BlockData> _blockDataTree = new Dictionary<Vector2Int, BlockData>();

        private string _dataPath;

        public EarthHeightBlockData(string dataPath)
        {
            _dataPath = dataPath;
        }

        public bool IsBlockLoaded(Vector2Int block)
        {
            return _blockDataTree.ContainsKey(block)
                   && _blockDataTree[block].RawHeightData != null
                   && _blockDataTree[block].RawHeightData.Length != 0;
        }

        public bool BlockFileExists(Vector2Int block)
        {
            return File.Exists(GetBlockPath(block));
        }

        public string GetBlockPath(Vector2Int block)
        {
            var lls = LatLonString(block.x, block.y);
            return _dataPath + $"{lls}.hgt";
        }

        public bool IsChunkLoaded(Vector2Int block, Vector2Int chunk)
        {
            if (_blockDataTree.ContainsKey(block) && _blockDataTree[block].chunks != null)
            {
                var b = _blockDataTree[block];
                var i = GetChunkListIndex(chunk);
                if (b.chunks.ContainsKey(i))
                    return b.chunks[i].data.Length > 0;
            }
            return false;
        }

        public int GetChunkListIndex(Vector2Int chunk)
        {
            return chunk.y * blockDivisions + chunk.x;
        }

        public bool ChunkFileExists(Vector2Int block, Vector2Int chunk)
        {
            return File.Exists(GetChunkPath(block, chunk));
        }

        public string GetChunkPath(Vector2Int block, Vector2Int chunk)
        {
            var lls = LatLonString(block);
            return Path.Combine(_dataPath, lls) + $"\\{chunk.x}x{chunk.y}y.chunk";
        }

        public IEnumerator LoadHgtData(Vector2Int block)
        {
            //if it's already loaded, break early
            if (IsBlockLoaded(block))
            {
                MonoBehaviour.print(LatLonString(block) + " is already loaded to memory.");
                yield break;
            }

            if (!BlockFileExists(block))
            {
                yield return DownloadData(block);
            }

            //now that we're sure we have the file, lets load it
            var byteData = File.ReadAllBytes(GetBlockPath(block));
            var shortData = new short[3601, 3601];
            var pStep = byteData.Length / 2 / 20;

            int min = short.MaxValue;
            int max = short.MinValue;

            short lastValid = 0;

            for (int i = 0; i < byteData.Length / 2; i++)
            {
                var hi = byteData[i * 2];
                var low = byteData[i * 2 + 1];
                short val = (short)((hi << 8) | low);

                if (val > short.MaxValue - 5)
                    val = lastValid;
                else
                    lastValid = val;

                if (val < min) min = val;
                if (val > max) max = val;

                shortData[i % 3601, i / 3601] = val;

                if (i % pStep == 0)
                {
                    MonoBehaviour.print($"lat:{block.y} lon:{block.x} | File loaded {(float)i / (byteData.Length / 2) * 100:F2}%");
                    yield return new WaitForUpdate();
                }
            }

            MonoBehaviour.print("min = " + min + " max = " + max);

            if (_blockDataTree.ContainsKey(block))
                _blockDataTree[block].RawHeightData = shortData;
            else
                _blockDataTree.Add(block, new BlockData(shortData));

            _blockDataTree[block].minValue = min;
            _blockDataTree[block].maxValue = max;
        }

        public string LatLonString(int lon, int lat) => LatLonString(new Vector2Int(lon, lat));

        public string LatLonString(Vector2Int block)
        {
            var ew = block.x < 0 ? "W" : "E";
            var ns = block.y < 0 ? "S" : "N";
            int lo = Mathf.Abs(block.x);
            int la = Mathf.Abs(block.y);

            return $"{ns}{la:D2}{ew}{lo:D3}";
        }

        public static string Decompress(string filePath)
        {
            var fi = new FileInfo(filePath);

            using (FileStream originalFileStream = fi.OpenRead())
            {
                string currentFileName = fi.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fi.Extension.Length) + ".hgt";

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
                return newFileName;
            }

        }

        public IEnumerator DownloadData(Vector2Int block)
        {
            var ew = block.x < 0 ? "W" : "E";
            var ns = block.y < 0 ? "S" : "N";
            int lo = Mathf.Abs(block.x);
            int la = Mathf.Abs(block.y);

            string path = $"https://s3.amazonaws.com/elevation-tiles-prod/skadi/{ns}{la:D2}/{ns}{la:D2}{ew}{lo:D3}.hgt.gz";

            var lls = LatLonString(block);
            string filename = _dataPath + $"{lls}.gz";

            UnityWebRequest www = new UnityWebRequest(path) { downloadHandler = new DownloadHandlerFile(filename) };

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }
            Debug.Log($"DL {lls} successful");
            Decompress(filename);
            File.Delete(filename);
        }

        public IEnumerator GetChunk(Vector2Int block, Vector2Int chunk, Action<Chunk> callback)
        {

            var chunkFile = new FileInfo(GetChunkPath(block, chunk));
            if (!chunkFile.Directory?.Exists ?? false)
            {
                chunkFile.Directory.Create();
            }
            if (!_blockDataTree.ContainsKey(block))
            {
                _blockDataTree.Add(block, new BlockData(null));
            }
            var bd = _blockDataTree[block];

            var cIndex = GetChunkListIndex(chunk);
            Chunk cd = bd.chunks.ContainsKey(cIndex) && bd.chunks[cIndex].data?.Length != 0 ? bd.chunks[cIndex] : null;

            //if already loaded
            if (cd != null)
            {

            }
            else if (!chunkFile.Exists) //not loaded and no file exists
            {
                if (bd.RawHeightData == null || bd.RawHeightData.Length == 0)
                {
                    yield return LoadHgtData(block);
                }
                int sizexy = 3600 / blockDivisions;

                //FileStream fs = new FileStream(dir + $"\\{x}x{y}y.chunk", FileMode.Create);
                var fs = chunkFile.Create();

                short[,] data = new short[3600 / blockDivisions, 3600 / blockDivisions];


                for (int yIndex = 0; yIndex < sizexy; yIndex++)
                {
                    for (int xIndex = 0; xIndex < sizexy; xIndex++)
                    {
                        data[xIndex, yIndex] = bd.RawHeightData[sizexy * chunk.x + xIndex,
                            sizexy * (blockDivisions - 1 - chunk.y) + yIndex];
                    }
                }

                Chunk c = new Chunk(data, bd.minValue, bd.maxValue);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fs, c);
                fs.Close();
                fs.Dispose();

                bd.chunks.Add(GetChunkListIndex(chunk), c);
                callback.Invoke(c);
            }
            else // not loaded and files exists
            {
                if (bd.RawHeightData?.Length == 0)
                {
                    yield return LoadHgtData(block);
                }

                var fs = chunkFile.OpenRead();
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                var fileChunkData = binaryFormatter.Deserialize(fs) as Chunk;
                bd.chunks.Add(GetChunkListIndex(chunk), fileChunkData);
                callback.Invoke(fileChunkData);

                fs.Close();
                fs.Dispose();
            }

            yield return null;
        }

        public IEnumerable<KeyValuePair<Vector2Int, BlockData>> LoadedBlockIterator()
        {
            foreach (KeyValuePair<Vector2Int, BlockData> blockData in _blockDataTree)
            {
                if (blockData.Value.RawHeightData != null
                    && blockData.Value.RawHeightData.Length != 0
                    || blockData.Value.chunks.Count != 0)
                    yield return blockData;
            }
        }
    }
}