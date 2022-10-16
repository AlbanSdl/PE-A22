using UnityEngine;
using System.Collections.Generic;

public class PathfindingMap {
    private Dictionary<Vector2Int, PathfindingNode> Nodes;
    private Vector2Int StartingNodeLocation;

    public PathfindingMap(MapManager MapManager, Vector2Int from, Vector2Int to) {
        Nodes = new Dictionary<Vector2Int, PathfindingNode>();
        StartingNodeLocation = from;
        foreach (var tile in MapManager.map) {
            PathfindingDirection direction = 0;
            if (MapManager.map.ContainsKey(new Vector2Int(tile.Key.x, tile.Key.y + 1)) && tile.Value.GetComponent<SelectorTile>().CanAccessTo(
                MapManager.map[new Vector2Int(tile.Key.x, tile.Key.y + 1)]?.GetComponent<SelectorTile>(),
                new Vector2Int(tile.Key.x, tile.Key.y + 1) == to
            )) {
                direction |= PathfindingDirection.UP;
            }
            if (MapManager.map.ContainsKey(new Vector2Int(tile.Key.x, tile.Key.y - 1)) && tile.Value.GetComponent<SelectorTile>().CanAccessTo(
                MapManager.map[new Vector2Int(tile.Key.x, tile.Key.y - 1)]?.GetComponent<SelectorTile>(),
                new Vector2Int(tile.Key.x, tile.Key.y - 1) == to
            )) {
                direction |= PathfindingDirection.DOWN;
            }
            if (MapManager.map.ContainsKey(new Vector2Int(tile.Key.x + 1, tile.Key.y)) && tile.Value.GetComponent<SelectorTile>().CanAccessTo(
                MapManager.map[new Vector2Int(tile.Key.x + 1, tile.Key.y)].GetComponent<SelectorTile>(),
                new Vector2Int(tile.Key.x + 1, tile.Key.y) == to
            )) {
                direction |= PathfindingDirection.RIGHT;
            }
            if (MapManager.map.ContainsKey(new Vector2Int(tile.Key.x - 1, tile.Key.y)) && tile.Value.GetComponent<SelectorTile>().CanAccessTo(
                MapManager.map[new Vector2Int(tile.Key.x - 1, tile.Key.y)]?.GetComponent<SelectorTile>(),
                new Vector2Int(tile.Key.x - 1, tile.Key.y) == to
            )) {
                direction |= PathfindingDirection.LEFT;
            }
            Nodes[tile.Key] = new PathfindingNode(tile.Key, direction, from, to);
        }
    }

    public List<PathfindingNode> GetNeighbours(PathfindingNode Node) {
        return Node.GetNeighbours().ConvertAll((from) => Nodes[from]);
    }

    public List<Vector2Int> ComputePath() {
        List<PathfindingNode> toSearch = new List<PathfindingNode>() { Nodes[StartingNodeLocation] };
        List<PathfindingNode> processed = new List<PathfindingNode>();

        while (toSearch.Count != 0) {
            var current = toSearch[0];
            foreach (var t in toSearch) 
                if (t.FCost < current.FCost || t.FCost == current.FCost && t.HCost < current.HCost)
                    current = t;
            processed.Add(current);
            toSearch.Remove(current);

            if (current.HCost == 0) { // It means we reached target
                var currentPathTile = current;
                var path = new List<PathfindingNode>();
                while (currentPathTile.GCost != 0) {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                }
                return path.ConvertAll((node) => node.Location);
            }

            foreach (var neighbour in GetNeighbours(current).FindAll((t) => !processed.Contains(t))) {
                var inSearch = toSearch.Contains(neighbour);
                var costToNeighbour = current.GCost + 1;

                if (!inSearch || costToNeighbour < neighbour.GCost) {
                    neighbour.GCost = costToNeighbour;
                    neighbour.Connection = current;

                    if (!inSearch) toSearch.Add(neighbour);
                }
            }
        }

        return null;
    }
}