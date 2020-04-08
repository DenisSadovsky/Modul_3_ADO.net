using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccessLayer.Enums;


namespace DataAccessLayer
{
    internal static class SQLGueries
    {
        public const string GetOrders =
            "Select OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry from dbo.Orders";

        public const string CountOrders = "Select count(OrderID) from dbo.Orders";

        public const string GetOrderById = "Select OrderID, OrderDate from dbo.Orders where Orders = @id;" +
                                           "select * from dbo.[Orders Details] where OrderID = @id";

        public const string GetProductInfoByOrderId = "SELECT products.ProductID, products.ProductName " +
                                                      "FROM[dbo].Orders orders " +
                                                      "join[dbo].[Order Details] orderDetails on orders.OrderID = orderDetails.OrderID " +
                                                      "join[dbo].[Products] products on orderDetails.ProductID = products.ProductID " +
                                                      "where orders.OrderID = @id ";
        public const string AddNewRowToOrders =
            "INSERT [dbo].Orders(EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry) " +
            " VALUES (@EmployeeID,@OrderDate,@RequiredDate,@ShippedDate,@ShipVia,@Freight,@ShipName,@ShipAddress, @ShipCity,@ShipRegion, @ShipPostalCode,@ShipCountry)";

        public const string UpdateOrderById = "UPDATE [dbo].Orders set ShipName = @ShipName, ShipCountry = @ShipCountry,ShipCity = @ShipCity WHERE ShippedDate IS NULL AND OrderID = @id;";

        public const string DeletedOrder = "DELETE FROM [dbo].Orders WHERE ShippedDate = NULL OR ShippedDate > RequiredDate;";

        public const string ChangedOrderDateById = "UPDATE [dbo].Orders set OrderDate = @OrderDate WHERE OrderID = @id;";

        public const string ChangedShippedDateById = "UPDATE [dbo].Orders set ShippedDate = @ShippedDate WHERE OrderID = @id;";

        //public const string CustOrderHist =
        //    "CREATE PROCEDURE CustOrderHist @CustomerID nchar(5) " +
        //    "AS " +
        //    "SELECT ProductName, Total=SUM(Quantity) " +
        //    "FROM Products P, [Order Details] OD, Orders O, Customers C " +
        //    "WHERE C.CustomerID = @CustomerID " +
        //    "AND C.CustomerID = O.CustomerID AND O.OrderID = OD.OrderID AND OD.ProductID = P.ProductID " +
        //    "GROUP BY ProductName ";
    }

    class OrderRepository : IOrderRepository
    {

        private readonly DbProviderFactory ProviderFactory;
        private readonly string ConnectionString;

        public OrderRepository(string connectionString, string provider)
        {
            ProviderFactory = DbProviderFactories.GetFactory(provider);
            ConnectionString = connectionString;

        }

        public IEnumerable<Order> GetOrders()
        {
            var resultOrders = new List<Order>();

            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.GetOrders;
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order();
                            order.OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")); 
                            order.CustomerID =
                                reader.GetString(reader.GetOrdinal("CustomerID"));
                            order.EmployeeID =
                                reader.GetInt32(reader.GetOrdinal("EmployeeID")); 
                            order.OrderDate = reader.GetNullableDateTime("OrderDate");
                            order.RequiredDate = reader.GetNullableDateTime("RequiredDate");
                            order.ShippedDate = reader.GetNullableDateTime("ShippedDate");
                            order.ShipVia = (int) reader["ShipVia"];
                            order.Freight = (decimal) reader["Freight"];
                            order.ShipName = reader["ShipName"].ToString();
                            order.ShipAddress = reader["ShipAddress"].ToString();
                            order.ShipCity = reader["ShipCity"].ToString();
                            order.ShipRegion = reader["ShipRegion"].ToString();
                            order.ShipPostalCode = reader["ShipPostalCode"].ToString();
                            order.ShipCountry = reader["ShipCountry"].ToString();
                            if (order.ShippedDate != null)
                            {
                                order.OrderStatus = order.ShippedDate < order.RequiredDate
                                    ? OrderStatus.Delivered
                                    : OrderStatus.OnTheWay;
                            }
                            else
                            {
                                order.OrderStatus = OrderStatus.New;
                            }

                            resultOrders.Add(order);
                        }
                    }
                }

                return resultOrders;
            }
        }


        public IEnumerable<Products> GetProductInfoByOrderId(int id)
        {
            var resultOrderInfo = new List<Products>();
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.GetProductInfoByOrderId;
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@id";
                    paramId.DbType = DbType.Int32;
                    paramId.Value = id;
                    command.Parameters.Add(paramId);

                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var products = new Products();
                            products.ProductID = (int) reader["ProductID"];
                            products.ProductName = (string) reader["ProductName"];



                            resultOrderInfo.Add(products);
                        }

                        return resultOrderInfo;
                    }
                }

            }
        }

        public void AddNewOrder(Order newOrder)
        {

            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.AddNewRowToOrders;
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();

                    command.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int));
                    command.Parameters["@EmployeeID"].Value = newOrder.EmployeeID;
                    command.Parameters.Add(new SqlParameter("@OrderDate", SqlDbType.DateTime));
                    command.Parameters["@OrderDate"].Value = newOrder.OrderDate;
                    command.Parameters.Add(new SqlParameter("@RequiredDate", SqlDbType.DateTime));
                    command.Parameters["@RequiredDate"].Value = newOrder.RequiredDate;
                    command.Parameters.Add(new SqlParameter("@ShippedDate", SqlDbType.DateTime));
                    command.Parameters["@ShippedDate"].Value = newOrder.ShippedDate;
                    command.Parameters.Add(new SqlParameter("@ShipVia", SqlDbType.Int));
                    command.Parameters["@ShipVia"].Value = newOrder.ShipVia;
                    command.Parameters.Add(new SqlParameter("@Freight", SqlDbType.Money));
                    command.Parameters["@Freight"].Value = newOrder.Freight;
                    command.Parameters.Add(new SqlParameter("@ShipName", SqlDbType.NVarChar, 40));
                    command.Parameters["@ShipName"].Value = newOrder.ShipName;
                    command.Parameters.Add(new SqlParameter("@ShipAddress", SqlDbType.NVarChar, 60));
                    command.Parameters["@ShipAddress"].Value = newOrder.ShipAddress;
                    command.Parameters.Add(new SqlParameter("@ShipCity", SqlDbType.NVarChar, 15));
                    command.Parameters["@ShipCity"].Value = newOrder.ShipCity;
                    command.Parameters.Add(new SqlParameter("@ShipRegion", SqlDbType.NVarChar, 15));
                    command.Parameters["@ShipRegion"].Value = newOrder.ShipRegion;
                    command.Parameters.Add(new SqlParameter("@ShipPostalCode", SqlDbType.NVarChar, 10));
                    command.Parameters["@ShipPostalCode"].Value = newOrder.ShipPostalCode;
                    command.Parameters.Add(new SqlParameter("@ShipCountry", SqlDbType.NVarChar, 15));
                    command.Parameters["@ShipCountry"].Value = newOrder.ShipCountry;

                    command.ExecuteNonQuery();
                }

            }

        }


        public void UpdateOrderById(int orderID, string shipName, string shipCity, string shipCountry)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.UpdateOrderById;
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters["@id"].Value = orderID;
                    command.Parameters.Add(new SqlParameter("@ShipName", SqlDbType.NVarChar, 40));
                    command.Parameters["@ShipName"].Value = shipName;
                    command.Parameters.Add(new SqlParameter("@ShipCity", SqlDbType.NVarChar, 15));
                    command.Parameters["@ShipCity"].Value = shipCity;
                    command.Parameters.Add(new SqlParameter("@ShipCountry", SqlDbType.NVarChar, 15));
                    command.Parameters["@ShipCountry"].Value = shipCountry;

                    var dd = command.ExecuteNonQuery();
                }

            }
        }
    


        public void DeleteOrders()
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.DeletedOrder;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ChangedOrderDate(int orderId, DateTime orderDate)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.ChangedOrderDateById;
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters["@id"].Value = orderId;
                    command.Parameters.Add(new SqlParameter("@OrderDate", SqlDbType.DateTime));
                    command.Parameters["@OrderDate"].Value = orderDate;

                    var dd = command.ExecuteNonQuery();
                }
            }
        }

        public void ChangedShippedDate(int orderId, DateTime shippedDate)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.ChangedShippedDateById;
                    command.CommandType = CommandType.StoredProcedure;

                    var paramId = command.CreateParameter();
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters["@id"].Value = orderId;
                    command.Parameters.Add(new SqlParameter("@ShippedDate", SqlDbType.DateTime));
                    command.Parameters["@ShippedDate"].Value = shippedDate;

                    var dd = command.ExecuteNonQuery();
                }
            }
        }

        public void CustOrderHist(string customerID)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CustOrderHist";
                    command.CommandType = CommandType.StoredProcedure;

                    var paramId = command.CreateParameter();
                    command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.NChar,5));
                    command.Parameters["@CustomerID"].Value = customerID;
                    //SqlParameter retval = command.Parameters.Add("@returnValue", SqlDbType.NVarChar, 40);
                    //retval.Direction = ParameterDirection.ReturnValue;
                    var dd = command.ExecuteNonQuery();
                    //var result = (int)command.Parameters["@returnValue"].Value;
                }
            }
        }

        public void CustOrderDetail(string orderID)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "CustOrderDetail";

                    var paramId = command.CreateParameter();
                    command.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
                    command.Parameters["@OrderID"].Value = orderID;

                    var dd = command.ExecuteNonQuery();
                }

            }
        }



        public Order GetOrderById(int id)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = SQLGueries.GetOrderById;
                    command.CommandType = CommandType.Text;

                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@id";
                    paramId.Value = id;

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return null;

                        reader.Read();
                        var order = new Order();
                        order.OrderID = reader.GetInt32(0);
                        order.OrderDate = reader.GetDateTime(1);

                        reader.NextResult();

                        //order.Details = new List<OrderDetail>();
                        //while (reader.Read())
                        //{
                        //    var detail = new OrderDetail();
                        //    detail.UnitPrice = (decimal) reader["unitPrice"];
                        //    detail.Quantity = (int) reader["Quantity"];


                        //    order.Details.Add(detail);
                        //}

                        return order;
                    }
                }

            }
        }
    }

    public static class ReaderExtensions
    {

        public static DateTime? GetNullableDateTime(this DbDataReader reader, string name)
        {
            var col = reader.GetOrdinal(name);
            return reader.IsDBNull(col) ? (DateTime?)null : (DateTime?)reader.GetDateTime(col);
        }
    }
}
