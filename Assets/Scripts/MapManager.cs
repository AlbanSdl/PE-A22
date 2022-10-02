using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace finished1
{
    public class MapManager : MonoBehaviour
    {
        // Singleton instance
        private static MapManager instance;
        public static MapManager Instance { get { return instance; } }

        public GameObject selectorPrefab;
        public GameObject selectorContainer;

        // Map tiles dictionary
        public Dictionary<Vector2Int, GameObject> map;
        public Vector2Int? selection = null;

        public bool ignoreBottomTiles;

        private void Awake() {
            if(instance != null && instance != this) {
                Destroy(this.gameObject);
            } else {
                instance = this;
            }
        }

        void Start() {
            // Checks every tile to instantiate a selector prefab on it
            var tileMap = gameObject.GetComponentInChildren<Tilemap>();
            map = new Dictionary<Vector2Int, GameObject>();

            BoundsInt bounds = tileMap.cellBounds;

            for (int z = bounds.max.z; z >= bounds.min.z; z--) {
                for (int y = bounds.min.y; y < bounds.max.y; y++) {
                    for (int x = bounds.min.x; x < bounds.max.x; x++) {

                        if (z == 0 && ignoreBottomTiles)
                            return;

                        var tileLocation = new Vector3Int(x, y, z);
                        var tileKey = new Vector2Int(x, y);

                        if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey)) {
                            var selectorTile = Instantiate(selectorPrefab, selectorContainer.transform);
                            var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
                            selectorTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z+1);
                            selectorTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                            selectorTile.GetComponent<SelectorTile>().InitializeLocation(tileLocation);
                            map.Add(tileKey, selectorTile);
                        }
                    }
                }
            }
        }

        void Update()
        {

        }
    }
}