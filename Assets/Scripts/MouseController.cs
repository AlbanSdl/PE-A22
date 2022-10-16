using UnityEngine;
using System.Linq;

public class MouseController : MonoBehaviour {
    public GameObject cursor;
    public GameObject mapManager;

    void LateUpdate() {
        MapManager mapManagerScript = mapManager.GetComponent<MapManager>();

        // Select the right tile when the mouse hovers by raycasting
        RaycastHit2D? hit = GetFocusedOnTile();

        if (hit != null && hit.HasValue) {
            SelectorTile selectorTile = hit.Value.collider.gameObject.GetComponent<SelectorTile>();
            cursor.transform.position = selectorTile.transform.position;
            cursor.GetComponent<SpriteRenderer>().sortingOrder = selectorTile.GetComponent<SpriteRenderer>().sortingOrder;

            // Highlights clicked tile
            if (Input.GetMouseButtonDown(0)) {
                if (mapManagerScript.selection != null) {
                    // execute movement for `mapManagerScript.selection` to reach `selectorTile.Location`
                    if (mapManagerScript.selection.Move(new Vector2Int(selectorTile.Location.x, selectorTile.Location.y)))
                        mapManagerScript.selection = null;
                }
            }
        }
    }

    public RaycastHit2D? GetFocusedOnTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        foreach (RaycastHit2D hit in hits.OrderByDescending(i => i.collider.transform.position.z))
            if (hit.collider.gameObject.GetComponent<SelectorTile>() is SelectorTile)
                return hit;

        return null;
    }

}
