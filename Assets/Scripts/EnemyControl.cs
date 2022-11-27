using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

sealed public class EnemyControl : Character<EnemyControl, AllyControl>
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

    public void Select() {
        if (!this.IsMoving()) this.ExecuteAction();
    }

    protected override void NotifyTileAnimationEnd(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    protected override void OnMovementFinished() {
        if (this.waitingForBattle != null) {
            // Start battle here. Retrieve Enemy in `this.waitingForBattle`
            this.tempArmor = this.armor;
            this.Attack();
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
