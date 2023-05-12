using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace FG.Gridmap {
    public static class GridMapUtility {
        public static void SortBlocksByPriority(ref Block[] blocks) {
            Array.Sort(blocks,
                (blockA, blockB) => ReferenceEquals(blockA, null) ? 1 :
                    ReferenceEquals(blockB, null) ? -1 : blockB.Priority.CompareTo(blockA.Priority));
        }

        public static bool LoadMap(in string levelGroup, in string level, GridMap map, Action<bool> onLoadDone = null, bool fromEditor = false) {
            Assert.IsNotNull(map, "gridMap != null");
            string file =
                $"{GameSettings.Instance.LevelFolder}{levelGroup}{Path.AltDirectorySeparatorChar}{level}.pmap";
            if (!File.Exists(file)) {
                onLoadDone?.Invoke(false);
                return false;
            }

            map.ClearMap();

            using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open))) {
                int blockTypeCount = reader.ReadInt32();
                for (int xIndex = 0; xIndex < blockTypeCount; xIndex++) {
                    List<MeshFilter> meshFilters = null;
                    if (GameSettings.Instance.MergeMeshes) {
                        meshFilters = new List<MeshFilter>();
                    }

                    int blockTypeIndex = reader.ReadInt32();
                    int positionCount = reader.ReadInt32();

                    for (int yIndex = 0; yIndex < positionCount; yIndex++) {
                        map.CreateBlock(blockTypeIndex,
                            new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                            out Block block);
                        if (GameSettings.Instance.MergeMeshes) {
                            meshFilters.Add(block.GetComponent<MeshFilter>());
                        }
                    }
                    
                    if (!fromEditor && GameSettings.Instance.MergeMeshes && blockTypeIndex != 1) {
                        MergeMeshes(ref meshFilters);
                    }
                }
            }

            onLoadDone?.Invoke(true);
            return true;
        }

        public static void SaveMap(in string levelGroup, in string level, GridMap map) {
            string file =
                $"{GameSettings.Instance.LevelFolder}{levelGroup}{Path.AltDirectorySeparatorChar}{level}.pmap";
            string directoryPath = Path.GetDirectoryName(file);

            Dictionary<int, List<Vector3Int>>
                saveData = new Dictionary<int, List<Vector3Int>>(map.transform.childCount);
            foreach (Transform child in map.transform) {
                Block block = child.GetComponent<Block>();
                if (block) {
                    if (!saveData.ContainsKey(block.PrefabIndex)) {
                        saveData.Add(block.PrefabIndex, new List<Vector3Int>());
                    }

                    saveData[block.PrefabIndex].Add(block.PositionInt);
                }
            }

            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Create))) {
                writer.Write(saveData.Count);
                foreach (var list in saveData) {
                    writer.Write(list.Key);
                    writer.Write(list.Value.Count);
                    List<Vector3Int> positions = list.Value;
                    foreach (Vector3Int position in positions) {
                        writer.Write(position.x);
                        writer.Write(position.y);
                        writer.Write(position.z);
                    }
                }
            }
            
            if (!File.Exists($"{directoryPath}{Path.AltDirectorySeparatorChar}info.txt")) {
                File.WriteAllText($"{directoryPath}{Path.AltDirectorySeparatorChar}info.txt",
                    $"{levelGroup}:Not set:0.00");
            }
        }

        public static void RandomSnapRotation(out Quaternion rotation) {
            rotation = Quaternion.Euler(new Vector3Int((int) (Random.Range(0, 4) * 90f),
                (int) (Random.Range(0, 4) * 90f),
                (int) (Random.Range(0, 4) * 90f)));
        }

        private static void MergeMeshes(ref List<MeshFilter> meshFilters) {
            Material[] baseMaterials = meshFilters[0].GetComponent<MeshRenderer>()?.sharedMaterials;
            CombineInstance[] combine = new CombineInstance[meshFilters.Count];
            for (int i = 0; i < meshFilters.Count; i++) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            Block block = meshFilters[0].GetComponent<Block>();
            GameObject go = new GameObject(block.BlockName, typeof(MeshRenderer), typeof(MeshFilter));
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(combine);

            if (GameSettings.Instance.ShowBlockSymbols) {
                go.GetComponent<MeshRenderer>().sharedMaterials = baseMaterials;
            }
            else {
                go.GetComponent<MeshRenderer>().sharedMaterial = baseMaterials[0];
            }
        }
    }
}