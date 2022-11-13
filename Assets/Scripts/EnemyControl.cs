using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemyControl : AbstractMovement<AllyControl, EnemyControl>
{
    public List<Behaviour<EnemyControl>> Behaviour = new List<Behaviour<EnemyControl>>() {
        new KillPlayerBehaviour(),
        new AttackPlayerBehaviour(),
        new FollowPackBehaviour(),
        new ExploreKnownTargetLocationBehaviour(),
        new PatrolBehaviour()
    };
    public Dictionary<AllyControl, TargetMemory> Memory = new Dictionary<AllyControl, TargetMemory>();
    public HashSet<EnemyControl> Pack { get {
        HashSet<EnemyControl> packMembers = new HashSet<EnemyControl>();
        HashSet<EnemyControl> waitingMembers = this.GetSameTypeNearby();
        while (waitingMembers.Count > 0) {
            packMembers.UnionWith(waitingMembers);
            HashSet<EnemyControl> computingElements = new HashSet<EnemyControl>(waitingMembers);
            waitingMembers.Clear();
            foreach(EnemyControl enemy in computingElements)
                waitingMembers.UnionWith(enemy.Pack);
            waitingMembers.ExceptWith(packMembers);
        }
        return packMembers;
    }}
    public Intent? Intent;
    public int ViewRadius = 10;
    public AlliesData EnemyData;
    public InstantiateCharacters instances;
    public GameObject gameManager;
    public GameObject mapManager;

    public Sprite sprite;
    public int health;
    public new string name;
    public int initiative;
    public int armor;
    public int attack;
    public int tempArmor;

    private AllyControl waitingForBattle;

    public void Awake()
    {
        if (EnemyData != null) {
            LoadData(EnemyData);
        }
    }

    public void LoadData (AlliesData data) {
        health = data.Health;
        sprite = data.Sprite;
        name = data.CharacterName;
        initiative = data.Initiative;
        armor = data.Armor;
        attack = data.Attack;
        tempArmor = data.TempArmor;
        MovementRange = data.MovementRange;
    }


    public override SelectorTile GetCurrentTile() {
        return this.GetTileAtReal(this.GetPosition());
    }

    private SelectorTile GetTileAtReal(Vector3 position) {
        Vector3Int cellPosition = mapManager.GetComponent<Tilemap>().WorldToCell(position);
        return GetTileAt(new Vector2Int(cellPosition.x, cellPosition.y));
    }

    private HashSet<EnemyControl> GetSameTypeNearby() {
        HashSet<EnemyControl> packMembers = new HashSet<EnemyControl>();
        Vector2Int cellPosition = (Vector2Int) mapManager.GetComponent<Tilemap>().WorldToCell(this.GetPosition());
        for (int i = 0; i < 4; i++) {
            Vector2Int location = cellPosition + new Vector2Int((i & 2) == 0 ? (i & 1) * 2 - 1 : 0, (i & 2) != 0 ? (i & 1) * 2 - 1 : 0);
            EnemyControl enemy = GetTileAt(location).GetCharacterOnTile<AllyControl, EnemyControl>();
            if (enemy != null) packMembers.Add(enemy);
        }
        return packMembers;
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
        if (!this.IsMoving()) this.ExecuteAction();
    }

    protected override MapManager GetMapManager() {
        return mapManager.GetComponent<MapManager>();
    }

    // Debug method
    protected override void DebugPath(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
    }

    public override MovementResult Move(Vector2Int to) {
        if (to == this.GetTilePosition()) return MovementResult.NONE;
        MovementResult result = base.Move(to);
        if (result != MovementResult.NONE) {
            this.HighlightCurrentTile();
            if (result == MovementResult.PARTIAL) return result;
            // Detect whether a battle will start after movement
            AllyControl other = GetTileAt(to).GetCharacterOnTile<EnemyControl, AllyControl>();
            if (other != null) {
                this.waitingForBattle = other;
                this.PopDestination();
            }
            return result;
        }
        return result;
    }

    protected override void NotifyTileAnimationEnd(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    protected override void OnMovementFinished() {
        if (this.waitingForBattle != null) {
            // Start battle here. Retrieve Enemy in `this.waitingForBattle`
            this.tempArmor = this.armor;
            int attackerDamage = attack - this.waitingForBattle.armor;
            int defenderDamage = Mathf.RoundToInt(this.waitingForBattle.attack - armor * 0.5f);
            this.waitingForBattle.health -= attack;
            health -= defenderDamage;
            Debug.Log(this.waitingForBattle.name + " lost " + attackerDamage + " HP !");
            Debug.Log(name + " inflicted " + defenderDamage + " damage in return !");
            this.waitingForBattle = null;
        }
        // End enemy turn
        this.GetMapManager().battleManager.GetComponent<BattleManager>().NextTurnStep();
    }

    public HashSet<Vector2Int> GetPossibleMovements() {
        HashSet<Vector2Int> locations = new HashSet<Vector2Int>();
        MapManager manager = this.GetMapManager();
        HashSet<Vector2Int> lastZoneLevel = new HashSet<Vector2Int>();
        HashSet<Vector2Int> currentLevel = new HashSet<Vector2Int>();
        lastZoneLevel.Add(this.GetTilePosition());
        for (int d = 0; d < this.MovementRange; d++) {
            foreach (var ZoneTile in lastZoneLevel) {
                SelectorTile tile = manager.map[ZoneTile].GetComponent<SelectorTile>();
                for (int i = 0; i < 4; i++) {
                    Vector2Int location = ZoneTile + new Vector2Int((i & 2) == 0 ? (i & 1) * 2 - 1 : 0, (i & 2) != 0 ? (i & 1) * 2 - 1 : 0);
                    if (tile.CanAccessTo<AllyControl, EnemyControl>(manager.map[location].GetComponent<SelectorTile>(), true)) currentLevel.Add(location);
                }
            }
            locations.UnionWith(lastZoneLevel);
            lastZoneLevel.Clear();
            lastZoneLevel.UnionWith(currentLevel);
            currentLevel.Clear();
        }
        locations.UnionWith(lastZoneLevel);
        locations.Remove(this.GetTilePosition());
        return locations;
    }

    private Intent? ChooseAction() {
        for (int i = 0; i < this.Behaviour.Count; i++) {
            Intent? action = this.Behaviour[i].Compute(this);
            if (action != null) return action;
        }
        return null;
    }

    public void ExecuteAction() {
        Intent? intent = this.ChooseAction();
        if (intent == null) {
            Debug.LogWarning("No Intent chosen: skipping turn");
            this.GetMapManager().battleManager.GetComponent<BattleManager>().NextTurnStep();
        }
        else {
            Debug.Log(intent.Value.Type);
            this.Move(intent.Value.Location);
        }
    }
}
