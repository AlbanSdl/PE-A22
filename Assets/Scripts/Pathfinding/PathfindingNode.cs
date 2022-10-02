using UnityEngine;
using System;
using System.Collections.Generic;

public class PathfindingNode {
    public Vector2Int Location;
    PathfindingDirection Direction;

    public int GCost, // Distance to start
               HCost; // Distance to target
    public int FCost { get { return this.GCost + this.HCost; }} // GCost + HCost
    public PathfindingNode Connection = null;

    public PathfindingNode(Vector2Int Location, PathfindingDirection Direction, Vector2Int origin, Vector2Int destination) {
        this.Location = Location;
        this.Direction = Direction;
        this.GCost = Math.Abs((Location - origin).x) + Math.Abs((Location - origin).y);
        this.HCost = Math.Abs((Location - destination).x) + Math.Abs((Location - destination).y);
    }

    public List<Vector2Int> GetNeighbours() {
        List<Vector2Int> nodes = new List<Vector2Int>();
        foreach (PathfindingDirection direction in Enum.GetValues(typeof(PathfindingDirection))) {
            if ((direction & Direction) != 0) {
                if (direction == PathfindingDirection.UP) nodes.Add(new Vector2Int(Location.x, Location.y + 1));
                else if (direction == PathfindingDirection.RIGHT) nodes.Add(new Vector2Int(Location.x + 1, Location.y));
                else if (direction == PathfindingDirection.DOWN) nodes.Add(new Vector2Int(Location.x, Location.y - 1));
                else if (direction == PathfindingDirection.LEFT) nodes.Add(new Vector2Int(Location.x - 1, Location.y));
            }
        }
        return nodes;
    }
}