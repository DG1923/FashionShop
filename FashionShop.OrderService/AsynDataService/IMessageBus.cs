namespace FashionShop.OrderService.AsynDataService
{
    public interface IMessageBus
    {
        void PublishOrderCreated(OrderMessage message);

    }
}
