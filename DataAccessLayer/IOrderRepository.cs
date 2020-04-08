using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();

        IEnumerable<Products> GetProductInfoByOrderId(int id);

        //Order GetOrderById(int id);

        void AddNewOrder(Order newOrder);

        void UpdateOrderById(int orderID, string shipName, string shipCity, string shipCountry);

        void DeleteOrders();

        void ChangedOrderDate(int orderId, DateTime orderDate);

        void ChangedShippedDate(int orderId, DateTime shippedDate);

        void CustOrderHist(string customerId);

        void CustOrderDetail(string orderId);
    }
}
