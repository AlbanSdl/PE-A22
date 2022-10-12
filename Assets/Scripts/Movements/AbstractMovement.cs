using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using System.Collections.Generic;

public abstract class AbstractMovement : MonoBehaviour {

    public const float MOVEMENT_ANIMATION_DURATION = .09f;

    protected abstract Vector2Int GetTilePosition();

    protected abstract Vector2 GetPosition();

    protected abstract void SetTilePosition(Vector2Int position);

    protected abstract void SetTileAnimationPosition(Vector2Int from, Vector2Int to, float progression);

    protected abstract MapManager GetMapManager();

    private List<Vector2Int> currentAnimatedPath = new List<Vector2Int>();
    private float? lastAnimationTime = null;

    public virtual void Move(Vector2Int to) {
        MapManager manager = this.GetMapManager();
        var path = new PathfindingMap(manager, this.GetTilePosition(), to).ComputePath();
        if (path == null) {
            Debug.LogWarning("Il n'y a pas de chemin vers la case sélectionnée");
            return;
        }
        foreach (var pathPart in path) this.DebugPath(pathPart);
        path.Reverse();
        this.currentAnimatedPath.Add(this.GetTilePosition());
        this.currentAnimatedPath.AddRange(path);
    }

    protected void Update() {
        if (currentAnimatedPath.Count() > 1) {
            if (lastAnimationTime == null) {
                lastAnimationTime = Time.time;
            } else if (lastAnimationTime < Time.time - MOVEMENT_ANIMATION_DURATION) {
                NotifyTileAnimationEnd(currentAnimatedPath[0]);
                currentAnimatedPath.RemoveAt(0);
                if (currentAnimatedPath.Count() > 1) {
                    lastAnimationTime = Time.time;
                } else {
                    lastAnimationTime = null;
                    NotifyTileAnimationEnd(currentAnimatedPath[0]);
                    currentAnimatedPath.RemoveAt(0);
                    OnMovementFinished();
                    return;
                }
            }
            // Animate here
            float animationPercentage = Math.Min(1, (Time.time - lastAnimationTime ?? 0) / MOVEMENT_ANIMATION_DURATION);
            this.SetTileAnimationPosition(this.currentAnimatedPath[0], this.currentAnimatedPath[1], animationPercentage);
        }
    }

    protected virtual void DebugPath(Vector2Int position) {
        // Override this method to add behaviour
    }

    protected virtual void NotifyTileAnimationEnd(Vector2Int position) {
        // Override this method
    }

    protected virtual void OnMovementFinished() {
        // Override this method
    }
}