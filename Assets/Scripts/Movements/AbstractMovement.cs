using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public abstract class AbstractMovement<O, T> : MonoBehaviour where T : AbstractMovement<O, T> where O : AbstractMovement<T, O> {

    public const float MOVEMENT_ANIMATION_DURATION = .09f;

    protected abstract Vector2Int GetTilePosition();

    protected abstract Vector3 GetPosition();

    public abstract void SetTilePosition(Vector2Int position);

    protected abstract void SetTileAnimationPosition(Vector2Int from, Vector2Int to, float progression);

    protected abstract MapManager GetMapManager();

    public int MovementRange;
    public bool canMove = true;

    private List<Vector2Int> currentAnimatedPath = new List<Vector2Int>();
    private float? lastAnimationTime = null;

    public virtual MovementResult Move(Vector2Int to) {
        if (canMove == true) {
            MapManager manager = this.GetMapManager();
            var path = new PathfindingMap<O, T>(manager, this.GetTilePosition(), to).ComputePath();
            if (path == null) {
                Debug.LogWarning("Il n'y a pas de chemin vers la case sélectionnée");
                return MovementResult.NONE;
            }
            this.OnPathComputed();
            path.Reverse();
            for (int i = 0; i < Math.Min(this.MovementRange, path.Count()); i++) this.DebugPath(path[i]);
            this.currentAnimatedPath.Add(this.GetTilePosition());
            this.currentAnimatedPath.AddRange(path.Take(this.MovementRange));
            return path.Count > this.MovementRange ? MovementResult.PARTIAL : MovementResult.FULL;
        }
        return MovementResult.NONE;
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
        } else if (currentAnimatedPath.Count == 1) {
            Vector2Int origin = this.currentAnimatedPath[0];
            this.currentAnimatedPath.RemoveAt(0);
            this.NotifyTileAnimationEnd(origin);
            this.OnMovementFinished();
        }
    }

    protected void PopDestination() {
        Vector2Int destination = this.currentAnimatedPath[this.currentAnimatedPath.Count - 1];
        this.currentAnimatedPath.RemoveAt(this.currentAnimatedPath.Count - 1);
        this.NotifyTileAnimationEnd(destination);
    }

    /// <summary>
    /// Called when the path has been computed (and is valid !) but has not been used yet
    /// </summary>
    protected virtual void OnPathComputed() {}

    protected virtual void DebugPath(Vector2Int position) {
        // Override this method to add behaviour
    }

    protected virtual void NotifyTileAnimationEnd(Vector2Int position) {
        // Override this method
    }

    protected virtual void OnMovementFinished() {
        // Override this method
    }

    public bool IsMoving() {
        return this.currentAnimatedPath.Count() > 0;
    }

    public abstract SelectorTile GetCurrentTile();

}

public enum MovementResult {
    FULL, PARTIAL, NONE
}