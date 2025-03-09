namespace Infrastructure.Comparer
{
    public class InventStockTakeLineComparer : IEqualityComparer<InventStockTakeLine>
    {
        public bool Equals(InventStockTakeLine x, InventStockTakeLine y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(InventStockTakeLine obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
