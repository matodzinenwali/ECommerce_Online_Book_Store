using HashPass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace BookNexusWebService
{
  
    public class Service1 : IService1
    {
        DatabaseDataContext db = new DatabaseDataContext();

        public bool AddProduct(Book book)
        {
            var b = db.Books.FirstOrDefault(p => p.title == book.title && p.author == book.author &&
                                        p.Image == book.Image && p.description == book.description && p.genre == book.genre
                                        && p.quantity == book.quantity && p.language == book.language && p.price == book.price);

            if (b == null)
            {
                var Newbook = new Book
                {
                    title = book.title,
                    author = book.author,
                    Image = book.Image,
                    description = book.description,
                    genre = book.genre,
                    quantity = book.quantity,
                    language = book.language,
                    isActive = 1,
                    price = book.price
                };

                db.Books.InsertOnSubmit(Newbook);

                try
                {
                    db.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<Book> GetBooksByCategory(string category)
        {
            List<Book> books = new List<Book>();

            dynamic products = (from b in db.Books
                                where b.isActive.Equals(1) &&
                                b.genre.Equals(category)
                                select b);
            if (products != null)
            {
                foreach (Book b in products)
                {
                    var book = new Book
                    {
                        book_ID = b.book_ID,
                        title = b.title,
                        Image = b.Image,
                        author = b.author,
                        description = b.description,
                        genre = b.genre,
                        quantity = b.quantity,
                        language = b.language,
                        price = b.price
                    };
                    books.Add(book);
                }
                return books;
            }
            else
            {
                return null;
            }
        }

        public bool AddtoCart(int userID, int bookID, int quantity)
        {
            var existingItem = db.ShoppingCarts.FirstOrDefault(c => c.user_Id == userID && c.book_ID == bookID);

            if (existingItem != null)
            {
                existingItem.quantity++;
            }
            else
            {
                var newCartItem = new ShoppingCart
                {
                    user_Id = userID,
                    book_ID = bookID,
                    quantity = quantity
                };
                db.ShoppingCarts.InsertOnSubmit(newCartItem);
            }

            try
            {
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClearCart(int userId)
        {
            try
            {
                var cartItems = db.ShoppingCarts.Where(c => c.user_Id == userId);
                db.ShoppingCarts.DeleteAllOnSubmit(cartItems);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUserProfile(User user)
        {
            try
            {
                var delete = db.Users.FirstOrDefault(u => u.email == user.email);

                if (delete != null && (delete.isActive == 1) == true)
                {
                    db.Users.DeleteOnSubmit(delete);
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ForgotPassword(string email, string newPassword)
        {
            var fp = db.Users.FirstOrDefault(u => u.email == email);
            bool changed = false;
            if (fp != null)
            {
                try
                {
                    fp.password = Secrecy.HashPassword(newPassword);
                    db.SubmitChanges();
                    changed = true;
                }
                catch (Exception e)
                {
                    e.GetBaseException();
                }
            }
            return changed;
        }

        public int GetActiveProductCount()
        {
            return db.Books.Count(p => p.isActive == 1);
        }

        public List<User> GetAllUsers()
        {
            var users = db.Users.Select(u =>
             new User
             {
                 user_ID = u.user_ID,
                 firstName = u.firstName,
                 lastName = u.lastName,
                 email = u.email,
                 isActive = u.isActive,
                 isLoggedin = u.isLoggedin,
                 type = u.type
             }).ToList();
            return users;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public List<CartItem> GetCartItems(int userId)
        {
            var cartItems = (from ci in db.ShoppingCarts
                             join b in db.Books on ci.book_ID equals b.book_ID
                             where ci.user_Id == userId
                             select new CartItem
                             {
                                 Title = b.title,
                                 Price = b.price,
                                 Quantity = ci.quantity
                             }).ToList();
            return cartItems;
        }

        public Book GetProduct(int ID)
        {
            var prod = db.Books.FirstOrDefault(u => u.book_ID == ID);

            if (prod != null)
            {
                return new Book
                {
                    book_ID = prod.book_ID,
                    title = prod.title,
                    Image = prod.Image,
                    author = prod.author,
                    description = prod.description,
                    genre = prod.genre,
                    quantity = prod.quantity,
                    language = prod.language,
                    isActive = 1,
                    price = prod.price
                };
            }
            else
            {
                return null;
            }
        }

        public int GetProductCount()
        {
            return db.Books.Count();
        }

        public List<Book> GetProducts()
        {
            List<Book> books = new List<Book>();
            dynamic prods = (from b in db.Books
                             where b.isActive.Equals(1)
                             select b).DefaultIfEmpty();
            if (prods == null)
            {
                return null;
            }
            else
            {
                foreach (Book p in prods)
                {
                    var objBook = new Book();
                    objBook.book_ID = p.book_ID;
                    objBook.title = p.title;
                    objBook.description = p.description;
                    objBook.genre = p.genre;
                    objBook.Image = p.Image;
                    objBook.language = p.language;
                    objBook.quantity = p.quantity;
                    objBook.price = p.price;
                    books.Add(objBook);
                }
                return books;
            }
        }

        public User GetUser(int u_ID)
        {
            try
            {
                var user = (db.Users.FirstOrDefault(u => u.user_ID == u_ID));

                if (user == null)
                    return null;

                return new User
                {
                    user_ID = user.user_ID,
                    firstName = user.firstName,
                    lastName = user.lastName,
                    email = user.email,
                    isActive = user.isActive,
                    type = user.type,
                    isLoggedin = user.isLoggedin
                };
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public int GetUserCount()
        {
            return db.Users.Count();
        }

        public char GetUserType(User details)
        {
            var user = db.Users.FirstOrDefault(u => u.email == details.email && u.password == details.password);

            if (user != null)
            {
                return user.type;
            }
            return 'X';
        }

        public char Login(string email, string password)
        {
            var hashedPassword = Secrecy.HashPassword(password);
            var tempUser = (from u in db.Users
                            where u.email.Equals(email) &&
                            u.password.Equals(hashedPassword)
                            select u).FirstOrDefault();
            if (tempUser == null)
            {
                return '\0';
            }
            else
            {
                return tempUser.type;
            }
        }

        public User LoginUser(string email, string password)
        {
            var hashedPassword = Secrecy.HashPassword(password);
            var tempUser = (from u in db.Users
                            where u.email.Equals(email) &&
                            u.password.Equals(hashedPassword)
                            select u).FirstOrDefault();

            if (tempUser == null)
            {
                return null;
            }
            else
            {
                tempUser.isLoggedin = 'Y';
                db.SubmitChanges();

                return new User
                {
                    user_ID = tempUser.user_ID,
                    firstName = tempUser.firstName,
                    type = tempUser.type,
                    isLoggedin = tempUser.isLoggedin
                };
            }
        }

        public bool LogOut(string Email)
        {
            var status = db.Users.FirstOrDefault(u => u.email == Email);

            if (status == null)
            {
                return false;
            }
            else
            {
                status.isLoggedin = 'N';
                db.SubmitChanges();
                return true;
            }
        }

        public bool PlaceOder(int userID, List<int> prodsIds)
        {
            var order = (from o in db.Orders
                         where o.user_id.Equals(userID)
                         select o).FirstOrDefault();

            if (order != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int Register(string firstName, string lastName, string Email, string password, char userType)
        {
            var tempUser = (from u in db.Users
                            where u.email.Equals(Email)
                            select u).FirstOrDefault();

            if (tempUser == null)
            {
                var hashedPassword = Secrecy.HashPassword(password);
                var objUser = new User();
                objUser.firstName = firstName;
                objUser.lastName = lastName;
                objUser.email = Email;
                objUser.password = hashedPassword;
                objUser.isActive = 'Y';
                objUser.type = userType;
                objUser.isLoggedin = 'N';
                objUser.DateRegistered = DateTime.Now;
                db.Users.InsertOnSubmit(objUser);
                try
                {
                    db.SubmitChanges();
                    return 0;
                }
                catch (Exception e)
                {
                    e.GetBaseException();
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }

        public bool RemoveFromCart(int userID, int bookID)
        {
            try
            {
                var existingItem = db.ShoppingCarts.FirstOrDefault(c => c.user_Id == userID && c.book_ID == bookID);

                if (existingItem != null)
                {
                    db.ShoppingCarts.DeleteOnSubmit(existingItem);
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveProductByID(int ID, int quantity)
        {
            try
            {
                var product = db.Books.FirstOrDefault(p => p.book_ID == ID);

                if (product != null)
                {
                    product.quantity = quantity;
                    product.isActive = 0;
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ResetUserDetails(string email, string oldpassword, string newPassword)
        {
            try
            {
                var hashedOldPassword = Secrecy.HashPassword(oldpassword);
                var hashedNewPassword = Secrecy.HashPassword(newPassword);

                var Resetuser = db.Users.FirstOrDefault(u => u.email == email && u.password == hashedOldPassword);

                if (Resetuser == null)
                    return false;

                Resetuser.password = hashedNewPassword;
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return false;
            }
        }

        public List<Book> SearchProducts(string keyword)
        {
            List<Book> prods = new List<Book>();
            var prod = (from b in db.Books where b.title.ToLower().Contains(keyword.ToLower()) && b.isActive.Equals(1) select b);

            foreach (Book b in prod)
            {
                var book = new Book();
                book.book_ID = b.book_ID;
                book.title = b.title;
                book.description = b.description;
                book.genre = b.genre;
                book.Image = b.Image;
                book.language = b.language;
                book.author = b.author;
                book.price = b.price;
                prods.Add(book);
            }
            return prods;
        }

        public bool UpdateProduct(Book book)
        {
            try
            {
                var update = db.Books.FirstOrDefault(p => p.book_ID == book.book_ID);

                if (update != null)
                {
                    update.title = book.title;
                    update.description = book.description;
                    update.quantity = book.quantity;
                    update.isActive = book.isActive;
                    update.price = book.price;
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void AddReview(int productID, int userId, string _Comment, int Rating)
        {
            var user = (from u in db.Users where u.user_ID.Equals(userId) select u).FirstOrDefault();
            var product = (from p in db.Books where p.book_ID.Equals(productID) select p).FirstOrDefault();

            if (user != null && product != null)
            {
                var review = new Review();
                review.book_ID = productID;
                review.user_ID = userId;
                review.comment = _Comment;
                review.rating = Rating;

                db.Reviews.InsertOnSubmit(review);
                db.SubmitChanges();
            }
        }

        public List<Review> GetReviews(int productId)
        {
            List<Review> reviews = new List<Review>();

            var revs = (from r in db.Reviews
                        where r.book_ID.Equals(productId)
                        select r);

            foreach (Review review in revs)
            {
                var tempReview = new Review();
                tempReview.book_ID = review.book_ID;
                tempReview.user_ID = review.user_ID;
                tempReview.comment = review.comment;
                tempReview.rating = review.rating;
                reviews.Add(tempReview);
            }
            return reviews;
        }

        public List<Order> GetOrdersByUser(int userID)
        {
            try
            {
                var orders = (from o in db.Orders
                              where o.user_id == userID
                              orderby o.date descending
                              select o).ToList();

                return orders;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting orders: {ex.Message}");
                return new List<Order>();
            }
        }

        public Invoice GetInvoiceWithDetails(int invoiceID)
        {
            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<Invoice>(i => i.Invoice_Lines);
            options.LoadWith<Invoice_Line>(il => il.Book);
            db.LoadOptions = options;

            return db.Invoices.FirstOrDefault(i => i.invoice_ID == invoiceID);
        }

 
        public SimpleInvoice GetSimpleInvoiceDetails(int invoiceID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetSimpleInvoiceDetails called for: {invoiceID}");

                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    var invoice = db.Invoices.FirstOrDefault(i => i.invoice_ID == invoiceID);

                    if (invoice == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Invoice not found");
                        return null;
                    }

                    return new SimpleInvoice
                    {
                        invoice_ID = invoice.invoice_ID,
                        user_ID = invoice.user_ID,
                        issueDate = invoice.issueDate,
                        subtotal = invoice.subtotal,
                        tax = invoice.tax,
                        total = invoice.total,
                        status = "Completed"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSimpleInvoiceDetails error: {ex.Message}");
                return null;
            }
        }

        public string TestDatabaseConnection()
        {
            try
            {
                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    var userCount = db.Users.Count();
                    var bookCount = db.Books.Count();
                    var invoiceCount = db.Invoices.Count();

                    return $"Database OK - Users: {userCount}, Books: {bookCount}, Invoices: {invoiceCount}";
                }
            }
            catch (Exception ex)
            {
                return $"Database connection failed: {ex.Message}";
            }
        }

        public string TestOrder(int userID)
        {
            try
            {
                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    Invoice invoice = new Invoice
                    {
                        user_ID = userID,
                        issueDate = DateTime.Now,
                        subtotal = 100.00m,
                        tax = 15.00m,
                        total = 115.00m
                    };

                    db.Invoices.InsertOnSubmit(invoice);
                    db.SubmitChanges();

                    return $"Test order successful - Invoice ID: {invoice.invoice_ID}";
                }
            }
            catch (Exception ex)
            {
                return $"Test order failed: {ex.Message}";
            }
        }


  
        public List<InvoiceItemDTO> GetInvoiceItems(int invoiceID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetInvoiceItems called for invoice: {invoiceID}");

                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    var items = (from il in db.Invoice_Lines
                                 join b in db.Books on il.book_ID equals b.book_ID
                                 where il.invoice_ID == invoiceID
                                 select new InvoiceItemDTO
                                 {
                                     Title = b.title,
                                     Quantity = il.quantity,
                                     UnitPrice = il.unit_price,
                                     LineTotal = il.line_total
                                 }).ToList();

                    System.Diagnostics.Debug.WriteLine($"Found {items.Count} items for invoice {invoiceID}");
                    return items;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetInvoiceItems error: {ex.Message}");
                return new List<InvoiceItemDTO>();
            }
        }

        public InvoiceWithItems GetInvoiceWithItems(int invoiceID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetInvoiceWithItems called for invoice: {invoiceID}");

                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    var invoice = db.Invoices.FirstOrDefault(i => i.invoice_ID == invoiceID);
                    if (invoice == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Invoice not found");
                        return null;
                    }

                    var items = GetInvoiceItems(invoiceID);

                    return new InvoiceWithItems
                    {
                        Invoice = invoice,
                        Items = items
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetInvoiceWithItems error: {ex.Message}");
                return null;
            }
        }

        public List<OrderHistoryDTO> GetOrderHistory(int userID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetOrderHistory called for user: {userID}");

                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    var orders = (from o in db.Orders
                                  join i in db.Invoices on o.Invoice_ID equals i.invoice_ID
                                  where o.user_id == userID
                                  orderby o.date descending
                                  select new OrderHistoryDTO
                                  {
                                      OrderId = o.order_Id,
                                      OrderDate = o.date,
                                      Status = o.status,
                                      Total = o.total,
                                      InvoiceId = i.invoice_ID
                                  }).ToList();

                    System.Diagnostics.Debug.WriteLine($"Found {orders.Count} orders for user {userID}");
                    return orders;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetOrderHistory error: {ex.Message}");
                return new List<OrderHistoryDTO>();
            }
        }

        public List<Invoice> GetInvoicesByUser(int userID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetInvoicesByUser called for user: {userID}");

                var invoices = (from inv in db.Invoices
                                where inv.user_ID == userID
                                orderby inv.issueDate descending
                                select inv).ToList();

                System.Diagnostics.Debug.WriteLine($"Found {invoices.Count} invoices for user {userID}");
                return invoices;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting invoices: {ex.Message}");
                return new List<Invoice>();
            }
        }

        public Invoice GetInvoiceDetails(int invoiceID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetInvoiceDetails called for invoice: {invoiceID}");

                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    var invoice = db.Invoices.FirstOrDefault(i => i.invoice_ID == invoiceID);

                    if (invoice != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Invoice found: {invoice.invoice_ID}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Invoice not found");
                    }

                    return invoice;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting invoice details: {ex.Message}");
                return null;
            }
        }

        public string GetInvoiceSummary(int invoiceID)
        {
            try
            {
                var invoiceWithItems = GetInvoiceWithItems(invoiceID);
                if (invoiceWithItems == null)
                    return "Invoice not found";

                var invoice = invoiceWithItems.Invoice;
                var items = invoiceWithItems.Items;

                StringBuilder summary = new StringBuilder();
                summary.AppendLine($"Invoice #: {invoice.invoice_ID}");
                summary.AppendLine($"Date: {invoice.issueDate:dd MMM yyyy}");
                summary.AppendLine($"Total: R{invoice.total:N2}");
                summary.AppendLine("Items:");

                foreach (var item in items)
                {
                    summary.AppendLine($"- {item.Title} x {item.Quantity} = R{item.LineTotal:N2}");
                }

                return summary.ToString();
            }
            catch (Exception ex)
            {
                return $"Error generating invoice summary: {ex.Message}";
            }
        }

       
        public int PlaceOrder(int userID, DataTable cart, string shippingAddress, string shippingMethod, decimal shippingCost)
        {
            using (DatabaseDataContext db = new DatabaseDataContext())
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"=== PlaceOrder Started ===");
                    System.Diagnostics.Debug.WriteLine($"UserID: {userID}, Cart Rows: {cart?.Rows?.Count}");

                    if (cart == null || cart.Rows.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Cart is empty or null");
                        return -1;
                    }

                    // Calculate totals
                    decimal subtotal = 0;
                    foreach (DataRow row in cart.Rows)
                    {
                        if (row["Price"] != DBNull.Value && row["Quantity"] != DBNull.Value)
                        {
                            decimal price = Convert.ToDecimal(row["Price"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            subtotal += price * quantity;
                        }
                    }

                    decimal discount = subtotal >= 1000 ? subtotal * 0.15m : 0;
                    decimal discountedSubtotal = subtotal - discount;
                    decimal tax = discountedSubtotal * 0.15m;
                    decimal total = discountedSubtotal + tax + shippingCost;

                    System.Diagnostics.Debug.WriteLine($"Subtotal: {subtotal}, Discount: {discount}, Tax: {tax}, Total: {total}");

                    // 1. Create Invoice
                    Invoice invoice = new Invoice
                    {
                        user_ID = userID,
                        issueDate = DateTime.Now,
                        subtotal = subtotal,
                        tax = tax,
                        total = total
                    };

                    db.Invoices.InsertOnSubmit(invoice);
                    db.SubmitChanges();
                    System.Diagnostics.Debug.WriteLine($"Invoice created: {invoice.invoice_ID}");

                    // 2. Create Order
                    Order order = new Order
                    {
                        date = DateTime.Now,
                        status = "Completed",
                        user_id = userID,
                        total = total,
                        Invoice_ID = invoice.invoice_ID
                    };

                    db.Orders.InsertOnSubmit(order);
                    db.SubmitChanges();
                    System.Diagnostics.Debug.WriteLine($"Order created: {order.order_Id}");

                    // 3. Create Order Lines (additional tracking)
                    foreach (DataRow row in cart.Rows)
                    {
                        if (row["BookID"] != DBNull.Value && row["Quantity"] != DBNull.Value && row["Price"] != DBNull.Value)
                        {
                            int bookID = Convert.ToInt32(row["BookID"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            decimal unitPrice = Convert.ToDecimal(row["Price"]);

                            var book = db.Books.FirstOrDefault(b => b.book_ID == bookID);
                            if (book == null)
                            {
                                System.Diagnostics.Debug.WriteLine($"Book {bookID} not found, skipping...");
                                continue;
                            }

                            // Create Order Line
                            OrderLine orderLine = new OrderLine
                            {
                                oderId = order.order_Id,
                                book_ID = bookID,
                                quantity = quantity,
                                priceAtOder = unitPrice
                            };
                            db.OrderLines.InsertOnSubmit(orderLine);

                            // Create Invoice Line
                            Invoice_Line invoiceLine = new Invoice_Line
                            {
                                invoice_ID = invoice.invoice_ID,
                                book_ID = bookID,
                                quantity = quantity,
                                unit_price = unitPrice,
                                line_total = unitPrice * quantity
                            };
                            db.Invoice_Lines.InsertOnSubmit(invoiceLine);

                            // Update inventory
                            book.quantity -= quantity;
                            if (book.quantity < 0) book.quantity = 0;
                        }
                    }

                    // 4. Clear cart
                    var cartItems = db.ShoppingCarts.Where(c => c.user_Id == userID);
                    db.ShoppingCarts.DeleteAllOnSubmit(cartItems);

                    db.SubmitChanges();
                    System.Diagnostics.Debug.WriteLine($"Order completed successfully. Invoice ID: {invoice.invoice_ID}");

                    return invoice.invoice_ID;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"=== ERROR in PlaceOrder ===");
                    System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }

                    return -1;
                }
            }


        }

        public List<Book> SortProductsByPrice(string orderby)
        {
            List<Book> books = new List<Book>();

            var book = (from b in db.Books where b.isActive.Equals(1) select b);

            if (orderby.Equals("asc"))
            {
                book = book.OrderBy(asc => asc.price);
            }
            else if (orderby.Equals("desc"))
            {
                book = book.OrderByDescending(p => p.price);
            }

            if (book != null)
            {
                foreach (Book b in book)
                {
                    var objBook = new Book();
                    objBook.book_ID = b.book_ID;
                    objBook.title = b.title;
                    objBook.author = b.author;
                    objBook.Image = b.Image;
                    objBook.description = b.description;
                    objBook.genre = b.genre;
                    objBook.language = b.language;
                    objBook.price = b.price;
                    books.Add(objBook);
                }

                return books;
            }
            else
            {
                return null;
            }

        }

        public int GetDistinctProdsSold()
        {
            var count = (from b in db.OrderLines select b.book_ID).Distinct().Count(); //Replace db.Books with db.Orders

            return count;

        }

        //Number of Products on Hand for Each Product Sold {in stock}
        public int InstockProdsSold()
        {
            var prodsActive = (from b in db.Books where b.isActive.Equals(1) select b.quantity).Sum();
            return prodsActive;
        }

        //Number of Registered Users per Day
        public List<User> RegisteredUsersPerDay()
        {
            List<User> users = new List<User>();

            var query = db.Users
              .GroupBy(u => u.DateRegistered.Date)
              .Select(g => new
              {
                  Date = g.Key,
                  Count = g.Count()  // count of User IDs for that date
              })
              .OrderBy(x => x.Date)
              .ToList();

            foreach (var item in query)
            {
                var u = new User();
                u.DateRegistered = item.Date;

                u.firstName = item.Count.ToString();
                users.Add(u);
            }

            return users;
        }

        //Total Revenue-> sum of all order amounts.
        public decimal TotalRevenue()
        {
            var revenue = (from o in db.OrderLines select o.quantity * o.priceAtOder).Sum(); //replace db.Book with Oders/OrderDetails

            return revenue;
        }

        public List<Invoice> GetInvoices(DateTime startDate, DateTime endDate)
        {
            var invoices = (from i in db.Invoices
                            where i.issueDate >= startDate && i.issueDate <= endDate
                            select i).ToList();

            return invoices;
        }

        public List<Book> GetMostPurchased(int limit)
        {
            if (limit <= 0) limit = 5;

            try
            {
                var books = db.OrderLines
                              .GroupBy(o => o.Book)        
                              .OrderByDescending(g => g.Sum(x => x.quantity))
                              .Take(limit)
                              .Select(g => g.Key)                   
                              .ToList();

                return books;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return new List<Book>();
            }
        }

    }
    [DataContract]
    public class SimpleInvoice
    {
        [DataMember]
        public int invoice_ID { get; set; }

        [DataMember]
        public int user_ID { get; set; }

        [DataMember]
        public DateTime issueDate { get; set; }

        [DataMember]
        public decimal subtotal { get; set; }

        [DataMember]
        public decimal tax { get; set; }

        [DataMember]
        public decimal total { get; set; }

        [DataMember]
        public string status { get; set; }
    }
    [DataContract]
    public class InvoiceItemDTO
    {
        [DataMember] public string Title { get; set; }
        [DataMember] public int Quantity { get; set; }
        [DataMember] public decimal UnitPrice { get; set; }
        [DataMember] public decimal LineTotal { get; set; }
    }
    [DataContract]
    public class InvoiceWithItems
    {
        [DataMember] public Invoice Invoice { get; set; }
        [DataMember] public List<InvoiceItemDTO> Items { get; set; }
    }
    [DataContract]
    public class OrderHistoryDTO
    {
        [DataMember] public int OrderId { get; set; }
        [DataMember] public DateTime OrderDate { get; set; }
        [DataMember] public string Status { get; set; }
        [DataMember] public decimal Total { get; set; }
        [DataMember] public int InvoiceId { get; set; }
    }
}