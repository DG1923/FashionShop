namespace FashionShop.OrderService.Model
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipping,
        Delivered,
        ReturnRequested,
        ReturnApproved,
        ReturnRejected,
        Completed,
        Cancelled
    }
}
