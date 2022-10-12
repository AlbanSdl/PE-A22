public enum PathfindingDirection: byte {
    UP = 0b0001, // means y++
    RIGHT = 0b0010, // means x++
    DOWN = 0b0100, // means y--
    LEFT = 0b1000 // means x--
}