using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MouseController : MonoBehaviour {
    public GameObject cursor;
    public GameObject mapManager;
    public GameObject pathPrefab;


    void Start()
    {

    }

    void Update() {
    }

    void LateUpdate() {
        // Select the right tile when the mouse hovers by raycasting
        RaycastHit2D? hit = GetFocusedOnTile();

        if (hit.HasValue) {
            SelectorTile selectorTile = hit.Value.collider.gameObject.GetComponent<SelectorTile>();
            cursor.transform.position = selectorTile.transform.position;
            cursor.GetComponent<SpriteRenderer>().sortingOrder = selectorTile.GetComponent<SpriteRenderer>().sortingOrder;

            // Highlights clicked tile
            if (Input.GetMouseButtonDown(0)) {
                finished1.MapManager mapManagerScript = mapManager.GetComponent<finished1.MapManager>();

                if (selectorTile.GetComponent<SpriteRenderer>().color == new Color(1, 1, 1, 1)) {
                    if (selectorTile.HideTile()) mapManagerScript.selection = null;
                } else {
                    if (selectorTile.ShowTile()) {
                        if (mapManagerScript.selection != null) {
                            // execute action from `mapManagerScript.selection` to `selectorTile.Location`
                            // or unselect them
                            Move(
                                (Vector2Int) mapManagerScript.selection,
                                new Vector2Int(selectorTile.Location.x, selectorTile.Location.y)
                            );
                            mapManagerScript.selection = null;
                        } else {
                            mapManagerScript.selection = new Vector2Int(selectorTile.Location.x, selectorTile.Location.y);
                        }
                    }
                }
            }
        }
    }

    public RaycastHit2D? GetFocusedOnTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if(hits.Length > 0) {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    void Move(Vector2Int from, Vector2Int to) {
        finished1.MapManager manager = mapManager.GetComponent<finished1.MapManager>();
        var path = new PathfindingMap(manager, from, to).ComputePath();
        foreach (var pathPart in path) {
            Tilemap tileMap = manager.GetComponentInChildren<Tilemap>();
            var pathTile = Instantiate(pathPrefab, manager.selectorContainer.transform);
            var cellWorldPosition = tileMap.GetCellCenterWorld(manager.map[pathPart].GetComponent<SelectorTile>().Location);
            pathTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
            pathTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
            pathTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
        }
    }
}
