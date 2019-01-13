using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cookie042.EarthHeightmap
{
    public class SelectionTree
    {
        public Dictionary<Vector2Int, List<Vector2Int>> selectedPositions = new Dictionary<Vector2Int, List<Vector2Int>>();

        public void SetSelected(Vector2Int block, Vector2Int chunk, bool newState)
        {
            var hasBlock = HasBlock(block);
            var hasChunk = HasChunk(block, chunk);

            //if the new state is true, add it to the collections if it's not there;
            if (newState)
            {
                if (!hasBlock)
                {
                    var cList = new List<Vector2Int> {chunk};

                    selectedPositions.Add(block, cList);
                }
                else //has the block
                {
                    if (!hasChunk)
                        selectedPositions[block].Add(chunk);
                }
            }
            else //if setting the new state to false, remove it from the collection, if it exists
            {
                if (hasChunk)
                {
                    selectedPositions[block].Remove(chunk);
                    if (selectedPositions.Count == 0)
                    {
                        selectedPositions.Remove(block);
                    }
                }
            }
        }

        public bool GetSelected(Vector2Int block, Vector2Int chunk) => HasChunk(block, chunk);

        public bool HasBlock(Vector2Int block) => selectedPositions.ContainsKey(block);
        public bool HasChunk(Vector2Int block, Vector2Int chunk) => HasBlock(block) && selectedPositions[block].Contains(chunk);
        public void Clear() => selectedPositions.Clear();

        //returns true if it did make a selection
        public bool Select(Vector2Int block, Vector2Int chunk)
        {
            if (!GetSelected(block, chunk))
            {
                SetSelected(block, chunk, true);
                return true;
            }
            return false;
        }

        public Selection[] GetSelectedChunks()
        {
            List<Selection> selection = new List<Selection>();

            foreach (var block in selectedPositions)  
            {
                foreach (Vector2Int chunk in block.Value)
                {
                    selection.Add(new Selection {block = block.Key, chunk = chunk});
                }
            }

            return selection.ToArray();
        }

        public Vector2Int[] GetSelectedBlocks()
        {
            return selectedPositions.Keys.ToArray();
        }

    }

    public struct Selection
    {
        public Vector2Int block;
        public Vector2Int chunk;
    }
}