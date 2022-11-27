using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

sealed public class AllyControl : Character<AllyControl, EnemyControl>
{
    public GameObject[] battleMenu;
    public GameObject pathPrefab;

    private HashSet<Vector2Int> PreviewLocations = new HashSet<Vector2Int>();

    public void Select() {
        if (!this.IsMoving()) {
            this.GetMapManager().selection = this;
            this.PreviewMovementRange();
        }
    }

    protected override void NotifyTileAnimationEnd(Vector2Int position) {
        foreach (GameObject gameObject in this.instances.EnemiesList) {
            EnemyControl enemy = gameObject.GetComponent<EnemyControl>();
            Vector2Int enemyLocation = (Vector2Int) enemy.GetCurrentTile().Location;
            if ((position - enemyLocation).sqrMagnitude <= enemy.ViewRadius * enemy.ViewRadius) {
                // update location
                if (enemy.Memory.ContainsKey(this)) {
                    TargetMemory memory = enemy.Memory[this];
                    memory.IsAccurate = true;
                    memory.Location = position;
                    memory.health = this.health;
                    memory.initiative = this.initiative;
                    memory.armor = this.armor;
                    memory.attack = this.attack;
                }
                else enemy.Memory[this] = new TargetMemory(position, this.health, this.initiative, this.armor, this.attack);
            } else {
                // mark as non-accurate
                if (enemy.Memory.ContainsKey(this)) enemy.Memory[this].IsAccurate = false;
            }
        }
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    protected override void OnMovementFinished() {
        if (this.waitingForBattle != null) {
            // Start battle here. Retrieve Enemy in `this.waitingForBattle`
            this.tempArmor = this.armor;
            StartCoroutine(ContextualActions());
        } else {
            // End ally turn
            this.GetMapManager().battleManager.GetComponent<BattleManager>().NextTurnStep();
        }
    }

    public void ContextualMenu(bool active) {
        instances.BattleManager.GetComponent<BattleManager>().ally = active ? this : null;
        foreach (GameObject menu in this.battleMenu) {
                menu.SetActive(active);
        }
    }

    IEnumerator ContextualActions() {
        turnUsed = false;
        ContextualMenu(true);
        canMove = false;
        this.waitingForBattle.canMove = false;
        yield return new WaitUntil(() => turnUsed);
        ContextualMenu(false);
        canMove = true;
        this.waitingForBattle.canMove = true;
        this.waitingForBattle = null;
        this.GetMapManager().battleManager.GetComponent<BattleManager>().NextTurnStep();
    }


    public void PreviewMovementRange() {
        if (this.PreviewLocations.Count() > 0)
            throw new System.Exception("Il faut appeler AbstractMovement.HideMovementRange avant AbstractMovement.PreviewMovementRange !");

        // Select locations
        MapManager manager = this.GetMapManager();
        HashSet<Vector2Int> lastZoneLevel = new HashSet<Vector2Int>();
        HashSet<Vector2Int> currentLevel = new HashSet<Vector2Int>();
        lastZoneLevel.Add(this.GetTilePosition());
        for (int d = 0; d < this.MovementRange; d++) {
            foreach (var ZoneTile in lastZoneLevel) {
                SelectorTile tile = manager.map[ZoneTile].GetComponent<SelectorTile>();
                for (int i = 0; i < 4; i++) {
                    Vector2Int location = ZoneTile + new Vector2Int((i & 2) == 0 ? (i & 1) * 2 - 1 : 0, (i & 2) != 0 ? (i & 1) * 2 - 1 : 0);
                    if (tile.CanAccessTo<EnemyControl, AllyControl>(manager.map[location].GetComponent<SelectorTile>(), true)) currentLevel.Add(location);
                }
            }
            this.PreviewLocations.UnionWith(lastZoneLevel);
            lastZoneLevel.Clear();
            lastZoneLevel.UnionWith(currentLevel);
            currentLevel.Clear();
        }
        this.PreviewLocations.UnionWith(lastZoneLevel);
        this.PreviewLocations.Remove(this.GetTilePosition());

        // Highlight range
        foreach (var Location in this.PreviewLocations) this.DebugPath(Location);
    }

    public void HideMovementRange() {
        foreach (var tileLocation in this.PreviewLocations)
            this.NotifyTileAnimationEnd(tileLocation);
        this.PreviewLocations.Clear();
    }

    protected override void OnPathComputed() {
        this.HideMovementRange();
    }
}
