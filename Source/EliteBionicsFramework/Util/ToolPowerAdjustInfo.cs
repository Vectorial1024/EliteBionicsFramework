namespace EBF.Util
{
    public struct ToolPowerAdjustInfo()
    {
        public float linearAdj = 0;
        public float scalingAdj = 1;

        public readonly bool HasAdjustment => linearAdj != 0 || scalingAdj != 1;

        public override readonly string ToString()
        {
            return "linearAdj: " + linearAdj + ", scalingAdj: " + scalingAdj;
        }
    }
}
