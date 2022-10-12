using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD

public class AllyControl : MonoBehaviour
{

    public AlliesData AllyData;

    public Sprite sprite;
    public new string name;
    public int health;
    public int initiative;
    public int attack;
    public int armor;
=======
using UnityEngine.Tilemaps;

sealed public class AllyControl : AbstractMovement
{

    public AlliesData AllyData;
    public GameObject mapManager;
    
    public GameObject pathPrefab;

    public Sprite sprite;
    public int health;
>>>>>>> main
    public void Start()
    {
        if (AllyData != null) {
            LoadData(AllyData);
        }
    }

<<<<<<< HEAD
    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData (AlliesData data) {
        sprite = data.Sprite;
        health = data.Health;
        initiative = data.Initiative;
        attack = data.Attack;
        armor = data.Armor;
        name = data.CharacterName;
=======
    public void LoadData (AlliesData data) {
        sprite = data.Sprite;
    }

    public SelectorTile GetCurrentTile() {
        return this.GetTileAtReal(this.GetPosition());
    }

    private SelectorTile GetTileAtReal(Vector2 position) {
        Vector3Int cellPosition = mapManager.GetComponent<Tilemap>().WorldToCell(position);
        return GetTileAt(new Vector2Int(cellPosition.x, cellPosition.y));
    }

    private SelectorTile GetTileAt(Vector2Int position) {
        return mapManager.GetComponent<MapManager>().map[new Vector2Int(position.x, position.y)].GetComponent<SelectorTile>();
    }

    // Debug method
    public void HighlightCurrentTile() {
        SelectorTile test = GetCurrentTile();
        test.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
    }

    protected override Vector2Int GetTilePosition() {
        Vector3Int currentLocation = this.GetCurrentTile().Location;
        return new Vector2Int(currentLocation.x, currentLocation.y);
    }

    protected override Vector2 GetPosition() {
        return new Vector2(transform.position.x, transform.position.y - ((RectTransform) transform).rect.height / 2);
    }

    private Vector3 ComputePosition(Vector2Int position) {
        SelectorTile tile = GetMapManager().map[position].GetComponent<SelectorTile>();
        return mapManager.GetComponent<Tilemap>().CellToWorld(tile.Location) + new Vector3(0, ((RectTransform) transform).rect.height * 3 / 4, 0);
    }

    protected override void SetTilePosition(Vector2Int position) {
        transform.position = ComputePosition(position);
    }

    protected override void SetTileAnimationPosition(Vector2Int from, Vector2Int to, float progression) {
        transform.position = ComputePosition(from) + (ComputePosition(to) - ComputePosition(from)) * progression;
    }

    public void Select() {
        this.GetMapManager().selection = this;
        this.GetMapManager().isSelectionFrame = true;
    }

    protected override MapManager GetMapManager() {
        return mapManager.GetComponent<MapManager>();
    }

    // Debug method
    protected override void DebugPath(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
    }

    public override void Move(Vector2Int to) {
        this.HighlightCurrentTile();
        base.Move(to);
    }

    protected override void NotifyTileAnimationEnd(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(0))
            this.Select();
>>>>>>> main
    }
}
