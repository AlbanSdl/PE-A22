using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MouseController : MonoBehaviour {
        public GameObject cursor;

        void Start()
        {

        }

        void LateUpdate() {
            // Select the right tile when the mouse hovers by raycasting
            RaycastHit2D? hit = GetFocusedOnTile();

            if (hit.HasValue) {
                GameObject selectorTile = hit.Value.collider.gameObject;
                cursor.transform.position = selectorTile.transform.position;
                cursor.GetComponent<SpriteRenderer>().sortingOrder = selectorTile.GetComponent<SpriteRenderer>().sortingOrder;

                // Highlights clicked tile
                if (Input.GetMouseButtonDown(0)) {
                    Color activeSelector = new Color(1, 1, 1, 1);
                    if (selectorTile.GetComponent<SpriteRenderer>().color == activeSelector) {      //TODO: fix the deactivating of the last selected tile
                        selectorTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                        Debug.Log("Deactivated tile");
                    }
                    selectorTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
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
    }
