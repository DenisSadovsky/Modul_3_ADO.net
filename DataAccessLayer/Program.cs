using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Enums;

namespace DataAccessLayer
{
    class Program
    {
        static void Main(string[] args)
        {

            var connectionString = @"data source=(localdb)\ProjectsV13; database=Northwind; Trusted_Connection=True";
            var provider = "System.Data.SqlClient";
            OrderRepository orderRepository = new OrderRepository(connectionString, provider );


            #region GetOrders
            //var orders = orderRepository.GetOrders();
            #endregion

            #region GetProductInfoByOrderId
            //var orderInfo = orderRepository.GetProductInfoByOrderId(11009);
            #endregion

            #region AddNewOrder
            //Order newOrder = new Order();
            //newOrder.OrderID = 33333;
            //newOrder.CustomerID = "DENZZ";
            //newOrder.EmployeeID = 2;
            //newOrder.OrderDate =  DateTime.Parse("10/21/1996");
            //newOrder.RequiredDate = DateTime.Parse("10/21/1996");
            //newOrder.ShippedDate = DateTime.Parse("10/3/1996");
            //newOrder.ShipVia = 2;
            //newOrder.Freight = (decimal)(40.26);
            //newOrder.ShipName = "Vitalyr";
            //newOrder.ShipAddress = "Malina";
            //newOrder.ShipCity = "Minsk";
            //newOrder.ShipRegion = "";
            //newOrder.ShipPostalCode = "22222";
            //newOrder.ShipCountry = "Belarus";
            //orderRepository.AddNewOrder(newOrder);
            #endregion

            #region DeleteOrders
            //orderRepository.DeleteOrders();
            #endregion

            //orderRepository.UpdateOrderById(11077,"Prostore","Gomel","RB");

            //orderRepository.ChangedOrderDate(11079, DateTime.Parse("10/21/2020"));

            //orderRepository.ChangedShippedDate(11079, DateTime.Parse("10/21/2020"));
        }
    }
}
