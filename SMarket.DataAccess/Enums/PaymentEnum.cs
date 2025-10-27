using System.ComponentModel;

namespace SMarket.DataAccess.Enums
{
    public enum PaymentMethod
    {
        [Description("Bank Transfer")]
        Banking = 1,

        [Description("Cash on Delivery")]
        COD = 2
    }
}