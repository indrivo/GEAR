namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class SalesStatisticViewModel
    {
        public virtual OrderReceivedStatisticViewModel OrderReceived { get; set; } = new OrderReceivedStatisticViewModel();
        public virtual TotalEarningsStatisticViewModel TotalEarnings { get; set; } = new TotalEarningsStatisticViewModel();
        public virtual NewCustomersStatisticViewModel NewCustomers { get; set; } = new NewCustomersStatisticViewModel();
    }

    public class OrderReceivedStatisticViewModel
    {
        public virtual int TotalOrderReceived { get; set; }
        public virtual int Percentage { get; set; }
    }

    public class TotalEarningsStatisticViewModel
    {
        public virtual decimal TotalEarnings { get; set; }
        public virtual int Percentage { get; set; }
    }

    public class NewCustomersStatisticViewModel
    {
        public virtual int NewCustomers { get; set; }
        public virtual int Percentage { get; set; }
    }
}
