using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Character<This, Enemy> : AbstractMovement<Enemy, This> where This : Character<This, Enemy> where Enemy : Character<Enemy, This> {
    public AlliesData AllyData;
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
    protected bool turnUsed;
    public Weapon weapon;
    public HealthBar healthBar;

    protected Character<Enemy, This> waitingForBattle;

    GameObject swordIcon;
    GameObject shieldIcon;
    GameObject bowIcon;
    public void Awake()
    {
        swordIcon = transform.GetChild(0).GetChild(5).gameObject;
        shieldIcon = transform.GetChild(0).GetChild(4).gameObject;
        bowIcon = transform.GetChild(0).GetChild(3).gameObject;
        swordIcon.SetActive(false);
        shieldIcon.SetActive(false);
        bowIcon.SetActive(false);
        if (AllyData != null) {
            LoadData(AllyData);
        }
    }

    private void LoadData(AlliesData data) {
        health = data.Health;
        sprite = data.Sprite;
        name = data.CharacterName;
        initiative = data.Initiative;
        armor = data.Armor;
        attack = data.Attack;
        tempArmor = data.TempArmor;
        MovementRange = data.MovementRange;
        weapon = data.Weapon;
        if (weapon.name == "Sword") {
            Debug.Log(swordIcon);
            swordIcon.SetActive(true);
        } else if (weapon.name == "Bow") {
            transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        } else {
            transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        healthBar = (HealthBar)transform.GetChild(0).gameObject.GetComponentInChildren(typeof(HealthBar), false);
    }

    public override SelectorTile GetCurrentTile() {
        return this.GetTileAtReal(this.GetPosition());
    }

    private SelectorTile GetTileAtReal(Vector3 position) {
        Vector3Int cellPosition = mapManager.GetComponent<Tilemap>().WorldToCell(position);
        return GetTileAt(new Vector2Int(cellPosition.x, cellPosition.y));
    }

    protected SelectorTile GetTileAt(Vector2Int position) {
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

    protected override MapManager GetMapManager() {
        return mapManager.GetComponent<MapManager>();
    }

    // Debug method
    protected override void DebugPath(Vector2Int position) {
        SelectorTile tile = this.GetTileAt(position);
        tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
    }

    public sealed override MovementResult Move(Vector2Int to) {
        if (to == this.GetTilePosition()) return MovementResult.NONE;
        MovementResult result = base.Move(to);
        if (result != MovementResult.NONE) {
            this.HighlightCurrentTile();
            if (result == MovementResult.PARTIAL) return result;
            // Detect whether a battle will start after movement
            Character<Enemy, This> other = GetTileAt(to).GetCharacterOnTile<This, Enemy>();
            if (other != null) {
                this.waitingForBattle = other;
                this.PopDestination();
            }
            return result;
        }
        return result;
    }

    public void Attack() {
        int maxHealth = AllyData.Health;
        int enemyMaxHealth = this.waitingForBattle.AllyData.Health;
        // Compute damage for opponent
        int attackerDamage = attack;
        if (this.weapon.ExtraDamageForWeapon == this.waitingForBattle.weapon)
            attackerDamage = Mathf.RoundToInt(attackerDamage * this.weapon.ExtraDamageMultiplier);
        attackerDamage -= this.waitingForBattle.tempArmor;
        // Compute self damage
        int defenderDamage = this.waitingForBattle.attack;
        if (this.waitingForBattle.weapon.ExtraDamageForWeapon == this.weapon)
            defenderDamage = Mathf.RoundToInt(defenderDamage * this.waitingForBattle.weapon.ExtraDamageMultiplier);
        defenderDamage = Mathf.RoundToInt(defenderDamage - tempArmor * 0.5f);
        // Apply damage
        this.waitingForBattle.health -= attack;
        health -= defenderDamage;
        healthBar.HealthSize(((float)health/maxHealth));
        this.waitingForBattle.healthBar.HealthSize(((float)this.waitingForBattle.health/enemyMaxHealth));
        Debug.Log(this.waitingForBattle.name + " lost " + attackerDamage + " HP !");
        Debug.Log(this.waitingForBattle.name + " inflicted " + defenderDamage +" damage in return !");
        turnUsed = true;
    }

    public void Defend() {
        this.tempArmor = Mathf.RoundToInt(1.5f*this.armor);
        turnUsed = true;
    }

    public void Wait() {
        turnUsed = true;
    }

}
