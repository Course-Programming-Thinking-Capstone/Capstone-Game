namespace GameScene
{
    [System.Serializable]
    public enum SelectType
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        Collect = 5,
        Loop = 6,
        Func = 7,
        Condition = 8,
        RoadHorizontal,
        RoadVertical,
        RoadTurn1,
        RoadTurn2,
        RoadTurn3,
        RoadTurn4,
    }
}