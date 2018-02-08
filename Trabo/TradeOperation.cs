namespace Trabo
{
    public class TradeOperation
    {
        public decimal Amount;
        public decimal Price;
        public decimal PriceWithCommission;

        public override string ToString()
        {
            return $"{nameof(Amount)}: {Amount}, {nameof(Price)}: {Price}, {nameof(PriceWithCommission)}: {PriceWithCommission}";
        }
    }
}