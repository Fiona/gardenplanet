using System;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrawberryNova
{
    public class AutoTileManager: MonoBehaviour
    {
        private const int numTiles = 48;
        private const int tilesPerRow = 8;
        private int[] tileMapping =
        {
            46, 46, 46, 46, 46, 46, 46, 46,     46, 46, 46, 46, 46, 46, 46, 46, // 15
            5, 5, 0, 5, 5, 0, 0, 5,             5, 5, 5, 5, 5, 5, 0, 5, // 31
            12, 12, 12, 12, 12, 12, 12, 12,         12, 12, 12, 12, 12, 12, 12, 12, // 47
            2, 0, 2, 0, 2, 0, 0, 0,             2, 0, 0, 0, 2, 0, 2, 0, // 63
            4, 5, 4, 4, 4, 0, 4, 4,             4, 4, 0, 4, 4, 4, 4, 4, // 79
            6, 0, 6, 6, 6, 0, 6, 6,             6, 6, 6, 6, 6, 6, 6, 6, // 95
            10, 10, 8, 8, 10, 10, 8, 8,           10, 10, 0, 8, 10, 10, 8, 8, // 111
            31, 17, 25, 20, 31, 0, 25, 20,             31, 17, 0, 20, 0, 17, 25, 20, // 127
            13, 13, 0, 13, 13, 13, 13, 13,          13, 13, 13, 0, 13, 13, 13, 13, // 143
            3, 3, 0, 3, 3, 3, 3, 3,            1, 1, 1, 1, 1, 1, 1, 1, // 159
            7, 7, 7, 7, 7, 7, 7, 7,             7, 7, 7, 7, 7, 7, 7, 7, // 175
            23, 26, 23, 26, 23, 26, 23, 26,            27, 28, 0, 28, 27, 28, 27, 28, // 191
            11, 0, 11, 0, 9, 9, 9, 9,           11, 11, 11, 11, 9, 9, 9, 9, // 207
            30, 30, 30, 30, 24, 24, 24, 24,            16, 16, 0, 0, 21, 21, 21, 21, // 223
            22, 22, 18, 18, 19, 19, 29, 29,           22, 22, 18, 18, 19, 19, 29, 29, // 239
            38, 41, 33, 42, 32, 14, 35, 37,            40, 34, 15, 45, 43, 44, 36, 47, // 255
        };

        public Material[] soilMaterials;
        public Material[] wateredSoilMaterials;

        public void Awake()
        {
            soilMaterials = GenerateTiledMaterials(Consts.MATERIAL_PATH_HOED_SOIL);
            wateredSoilMaterials = GenerateTiledMaterials(Consts.MATERIAL_PATH_WATERED_HOED_SOIL);
        }

        private void OnDestroy()
        {
            foreach(var mat in soilMaterials)
                Destroy(mat);
            soilMaterials = null;
            foreach(var mat in wateredSoilMaterials)
                Destroy(mat);
            wateredSoilMaterials = null;
        }

        public void SetMaterialOfSoil(GameObject soilObject, TilePosition tilePos, bool isWatered, bool isNew)
        {
            var meshRenderer = soilObject.GetComponentInChildren<MeshRenderer>();
            var materialIndex = GetTileIndex(tilePos, isNew);
            meshRenderer.material = isWatered ? wateredSoilMaterials[materialIndex] : soilMaterials[materialIndex];
        }

        private Material[] GenerateTiledMaterials(string materialPath)
        {
            var mats = new Material[numTiles];
            var numRows = numTiles / 8;
            var singleTileXOffset = 1.0f / tilesPerRow;
            var singleTileYOffset = 1.0f / numRows;
            var baseMaterial = Resources.Load<Material>(materialPath);
            baseMaterial.mainTextureScale = new Vector2(singleTileXOffset, singleTileYOffset);
            foreach(var i in Enumerable.Range(0, numTiles))
            {
                var newMaterial = new Material(baseMaterial);
                var column = (i % tilesPerRow);
                var row = (numRows - ((int) ((float) i / tilesPerRow) + 1));
                newMaterial.mainTextureOffset = new Vector2(
                    singleTileXOffset * column,
                    singleTileYOffset * row
                );
                mats[i] = newMaterial;
            }
            return mats;
        }

        private int GetTileIndex(TilePosition tilePos, bool isNew)
        {
            int index = 0;

            var northEast = tilePos.Offset(1, -1).GetTileWorldObjects("crop");
            if(northEast.Count > 0)
            {
                index += 1;
                if(isNew)
                    northEast[0].SetAppearence();
            }

            var southEast = tilePos.Offset(1, 1).GetTileWorldObjects("crop");
            if(southEast.Count > 0)
            {
                index += 2;
                if(isNew)
                    southEast[0].SetAppearence();
            }

            var southWest = tilePos.Offset(-1, 1).GetTileWorldObjects("crop");
            if(southWest.Count > 0)
            {
                index += 4;
                if(isNew)
                    southWest[0].SetAppearence();
            }

            var northWest = tilePos.Offset(-1, -1).GetTileWorldObjects("crop");
            if(northWest.Count > 0)
            {
                index += 8;
                if(isNew)
                    northWest[0].SetAppearence();
            }

            var north = tilePos.Offset(0, -1).GetTileWorldObjects("crop");
            if(north.Count > 0)
            {
                index += 16;
                if(isNew)
                    north[0].SetAppearence();
            }

            var east = tilePos.Offset(1, 0).GetTileWorldObjects("crop");
            if(east.Count > 0)
            {
                index += 32;
                if(isNew)
                    east[0].SetAppearence();
            }

            var south = tilePos.Offset(0, 1).GetTileWorldObjects("crop");
            if(south.Count > 0)
            {
                index += 64;
                if(isNew)
                    south[0].SetAppearence();
            }

            var west = tilePos.Offset(-1, 0).GetTileWorldObjects("crop");
            if(west.Count > 0)
            {
                index += 128;
                if(isNew)
                    west[0].SetAppearence();
            }
            if(isNew)
                Debug.Log(index);
            return tileMapping[index];
        }

    }
}