using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
                new Product {ProductID = 4, Name = "P4" },
                new Product {ProductID = 5, Name = "P5" }
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // ActB
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // arrange - define a HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myHelper = null;

            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo { CurrentPage = 2, TotalItems = 28, ItemsPerPage = 10 };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                           + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                           + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());

        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            // Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            // assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            // arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            });

            // Arrange - create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Action
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            // assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
            });

            // Arrange - create the controller
            NavController target = new NavController(mock.Object);

            // Act - get the set of categories
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();


            // Assert
            Assert.AreEqual(results.Length, 3, "Wrong length");
            Assert.AreEqual(results[0], "Apples", "Apples missing");
            Assert.AreEqual(results[1], "Oranges", "Oranges missing");
            Assert.AreEqual(results[2], "Plums", "Plums missing");            
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
            });

            // Arrange
            string categoryToSelect = "Apples";

            // Arrange - create the controller
            NavController target = new NavController(mock.Object);

            // Act - get the set of categories
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;


            // Assert
            Assert.AreEqual(categoryToSelect, result);            
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            });

            // Arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            // Action - test the product counts for different categories
            int resl = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            // assert
            Assert.AreEqual(resl, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void CartTest()
        {
            // arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Arrange - create a new card
            Cart target = new Cart();

            // act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();


            // assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Arrange - create a cart
            Cart target = new Cart();

            // act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.ToArray();

            // assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            // Arrange - create a cart
            Cart target = new Cart();

            // Arrange - add some products to the cart
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // act
            target.RemoveItem(p2);

            // assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            // arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - create a cart
            Cart target = new Cart();


            // act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - create a cart
            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);

            // act
            target.Clear();

            // assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }


        [TestMethod]
        public void Can_Add_To_Cart()
        {
            // arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" },
            }.AsQueryable());


            // Arrange - create a Cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController target = new CartController(mock.Object, null);

            // Act - add a product to the cart
            target.AddToCart(cart, 1, null);


            // assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Summary()
        {
            // arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" },
            }.AsQueryable());


            // Arrange - create a Cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController target = new CartController(mock.Object, null);


            // act - add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            // assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // arrange - create a cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController target = new CartController(null, null);


            // Act - call the index action method
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            // assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // arrange - create a mock order processor
            Mock<IOrderProcess> mock = new Mock<IOrderProcess>();
            // Arrange - create an empty cart
            Cart cart = new Cart();
            // Arrange - create shipping details
            ShippingDetails shippingDetails = new ShippingDetails();
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            // act
            ViewResult result = target.Checkout(cart, shippingDetails);

            // assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);

            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // arrange - create a mock order processing
            Mock<IOrderProcess> mock = new Mock<IOrderProcess>();

            // Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            // Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");


            // Act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());


            // Assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            // Assert - check that the method is returning the default value
            Assert.AreEqual("", result.ViewName);

            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            // arrange - create a mock order processor
            Mock<IOrderProcess> mock = new Mock<IOrderProcess>();

            // Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            // Act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());

            // Assert - check that the method is returning the Completed view
            Assert.AreEqual("Completed", result.ViewName);

            // Assert - check that I am passing a valid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            // Arrange - create a Product with image data
            Product prod = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            // arrange - create a mock order processor
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 3, Name = "P3"}
            }.AsQueryable());

            // Arrange - create the controller
            ProductController target = new ProductController(mock.Object);

            // Act - call the GetImage action method
            ActionResult result = target.GetImage(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
        {
            // arrange - create a mock order processor
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"}
            }.AsQueryable());

            // Arrange - create the controller
            ProductController target = new ProductController(mock.Object);

            // Act - call the GetImage action method
            ActionResult result = target.GetImage(100);

            // Assert 
            Assert.IsNull(result);
        }
    }
}
