namespace EBF.Util
{
    public struct ToolPowerAdjustInfo()
    {
        public int linearAdj = 0;
        public float scalingAdj = 1;

        public readonly bool HasAdjustment => linearAdj != 0 || scalingAdj != 1;
    }
}
