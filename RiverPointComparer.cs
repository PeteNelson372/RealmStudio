namespace RealmStudio
{
    internal class RiverPointComparer : IEqualityComparer<MapRiverPoint>
    {
        public bool Equals(MapRiverPoint? p1, MapRiverPoint? p2)
        {
            if (p1 == null || p2 == null) return false;
            return p1.RiverPoint.X == p2.RiverPoint.X && p1.RiverPoint.Y == p2.RiverPoint.Y;
        }

        public int GetHashCode(MapRiverPoint obj)
        {
            // don't want points compared by hashcode, so return 0, forcing comarison through Equals method
            return 0;
        }
    }
}
