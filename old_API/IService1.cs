using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace BookNexusWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        // User Related Contracts
        [OperationContract]
        int Register(string firstName, string lastName, string Email, string password, char userType);

        [OperationContract]
        char Login(String email, String password);

        [OperationContract]
        User LoginUser(string email, string password);

        [OperationContract]
        bool LogOut(String Email);

        [OperationContract]
        bool ResetUserDetails(String email, String oldpassword, String newPassword);

        [OperationContract]
        List<User> GetAllUsers();

        [OperationContract]
        User GetUser(int u_ID);

        [OperationContract]
        char GetUserType(User user);

        [OperationContract]
        int GetUserCount();

        [OperationContract]
        bool ForgotPassword(String email, String newPassword);

        [OperationContract]
        bool DeleteUserProfile(User user);

        //Product Related Contracts
        [OperationContract]
        //add book by admin
        bool AddProduct(Book books);

        [OperationContract]
        bool RemoveProductByID(int ID, int quantity);

        [OperationContract]
        bool UpdateProduct(Book book);

        [OperationContract]
        Book GetProduct(int ID);

        [OperationContract]
        bool ClearCart(int userId);

        [OperationContract]
        int GetProductCount();

        [OperationContract]
        List<Book> SearchProducts(String keyword);

        [OperationContract]
        List<Book> GetProducts();

        [OperationContract]
        List<Book> GetMostPurchased(int limit);

        [OperationContract]
        bool AddtoCart(int userID, int bookID, int quantity);

        [OperationContract]
        bool RemoveFromCart(int userID, int bookID);

        [OperationContract]
        List<CartItem> GetCartItems(int userId);

        [OperationContract]
        List<Book> GetBooksByCategory(string category);

        [OperationContract]
        void AddReview(int productID, int UserId, String Comment, int Rating);

        [OperationContract]
        List<Review> GetReviews(int productId);

        // Order and Invoice Related Contracts
        [OperationContract]
        int PlaceOrder(int userID, System.Data.DataTable cart, string shippingAddress, string shippingMethod, decimal shippingCost);

        [OperationContract]
        List<Invoice> GetInvoicesByUser(int userID);

        [OperationContract]
        Invoice GetInvoiceDetails(int invoiceID);

        [OperationContract]
        List<Order> GetOrdersByUser(int userID);

        // Test and Utility Contracts
        [OperationContract]
        string TestDatabaseConnection();

        [OperationContract]
        string TestOrder(int userID);

        [OperationContract]
        SimpleInvoice GetSimpleInvoiceDetails(int invoiceID);

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        List<InvoiceItemDTO> GetInvoiceItems(int invoiceID);
        [OperationContract]
        int GetActiveProductCount();

        [OperationContract]
        bool PlaceOder(int userID, List<int> prodsIds);

        [OperationContract]
        Invoice GetInvoiceWithDetails(int invoiceID);

        [OperationContract]
        List<Book> SortProductsByPrice(String order);

        [OperationContract]
        int GetDistinctProdsSold();

        [OperationContract]
        int InstockProdsSold();

        [OperationContract]
        List<User> RegisteredUsersPerDay();

        [OperationContract]
        decimal TotalRevenue();

        [OperationContract]
        List<Invoice> GetInvoices(DateTime startDate, DateTime endDate);
    }

}