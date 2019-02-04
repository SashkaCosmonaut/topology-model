namespace TopologyModel.Enumerations
{
    /// <summary>
    /// Перечисление вариантов месторасположения узлов графа внутри участка (перечисляются по часовой стрелке).
    /// </summary>
    public enum LocationInRegion
    {
        TopBorder,

        RightBorder,

        BottomBorder,

        LeftBorder,

        LeftTopCorder,

        RightTopCorner,

        RightBottomCorner,

        LeftBottomCorner,

        Inside
    }
}
