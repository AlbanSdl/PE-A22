using UnityEngine;
using UnityEngine.Tilemaps;

sealed public class AllyControl : AbstractMovement
{
    public AlliesData AllyData;
    public InstantiateCharacters instances;
    public GameObject gameManager;
    public GameObject mapManager;
    
    public GameObject pathPrefab;

    public Sprite sprite;
    public int health;
    public new string name;
    public int initiative;
    public int armor;
    public int attack;

    private AllyControl waitingForBattle;

    public void Awake()
    {
        if (AllyData != null) {
            LoadData(AllyData);
        }
        //instances = gameManager.GetComponent<InstantiateCharacters>();
    }

    public void LoadData (AlliesData data) {
        health = data.Health;
        sprite = data.Sprite;
        name = data.CharacterName;
        initiative = data.Initiative;
        armor = data.Armor;
        attack = data.Attack;
    }


    public SelectorTile GetCurrentTile() {
        return this.GetTileAtReal(this.GetPosition());
    }

    private SelectorTile GetTileAtReal(Vector3 position) {
        Vector3Int cellPosition = mapManager.GetComponent<Tilemap>().WorldToCell(position);
        return GetTileAt(new Vector2Int(cellPosition.x, cellPosition.y));
    }

    private SelectorTile GetTileAt(Vector2Int position) {
        return mapManager.GetComponent<MapManager>().map[position].GetComponent<SelectorTile>();
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

    protected override Vector3 GetPosition() {
        return new Vector3(transform.position.x, transform.position.y - ((RectTransform) transform).rect.height / 2, transform.position.z);
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
    }

    protected override MapManager GetMapManager() {
        return mapManager.GetComponent<MapManager>();
    }

    // Debug method
    protected override void DebugPath(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
    }

    public override bool Move(Vector2Int to) {
        if (to == this.GetTilePosition()) return false;
        if (base.Move(to)) {
            this.HighlightCurrentTile();
            // Detect whether a battle will start after movement
            AllyControl other = GetTileAt(to).GetAllyOnTile();
            if (other != null) {
                this.waitingForBattle = other;
                this.PopDestination();
            }
            return true;
        }
        return false;
    }

    protected override void NotifyTileAnimationEnd(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    protected override void OnMovementFinished() {
        if (this.waitingForBattle != null) {
            // Start battle here. Retrieve Enemy in `this.waitingForBattle`
            int attackerDamage = attack - this.waitingForBattle.armor;
            int defenderDamage = Mathf.RoundToInt(this.waitingForBattle.attack - armor * 0.5f);
            this.waitingForBattle.health -= attack;
            health -= defenderDamage;
            Debug.Log(this.waitingForBattle.name + " lost " + attackerDamage + " HP !");
            Debug.Log(name + " inflicted " + defenderDamage +" damage in return !");
            this.waitingForBattle = null;
        }
        // End ally turn
        this.GetMapManager().battleManager.GetComponent<BattleManager>().NextTurnStep();
    }

}
