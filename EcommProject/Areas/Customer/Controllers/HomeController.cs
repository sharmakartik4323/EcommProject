//using EcommProject.DataAccess.Repository;
//using EcommProject.DataAccess.Repository.IRepository;
//using EcommProject.Models;
//using EcommProject.Models.ViewModels;
//using EcommProject.Utility;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using System.Diagnostics;
//using System.Diagnostics.Eventing.Reader;
//using System.Security.Claims;
//using System.Security.Cryptography.X509Certificates;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//namespace EcommProject.Areas.Customer.Controllers
//{
//    [Area("Customer")]
//    //[AllowAnonymous]
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly IUnitOfWork _unitOfWork;

//        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
//        {
//            _logger = logger;
//            _unitOfWork = unitOfWork;
//        }

//        public IActionResult Index(string search, string searchTxt)
//        {
//            //Session Count
//            var claimsIdentity = (ClaimsIdentity)User.Identity;
//            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
//            if (claims != null)
//            {
//                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
//                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
//            }

//            //****
//            //var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType"); 
//            //return View(productList);

//            if (search == "Title")
//            {
//                var productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType").Where(m => EF.Functions.Like(m.Title, $"%{searchTxt}%")).ToList();
//                return View(productList);
//            }
//            else if (search == "Author")
//            {
//                var productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType").Where(m => EF.Functions.Like(m.Author, $"%{searchTxt}%")).ToList();
//                return View(productList);
//            }
//            else
//            {
//                var productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType");
//                return View(productList);
//            }
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//        public IActionResult Details(int id)
//        {
//            //Session Count
//            var claimsIdentity = (ClaimsIdentity)User.Identity;
//            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
//            if (claims != null)
//            {
//                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
//                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
//            }
//            //****
//            var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");//In FirstOrDefault go to implementation
//            if (productInDb == null) return NotFound();
//            var shoppingCart = new ShoppingCart()
//            {
//                Product = productInDb,
//                ProductId = productInDb.Id
//            };
//            return View(shoppingCart);
//        }
//        [Authorize]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Details(ShoppingCart shoppingCart)
//        {
//            shoppingCart.Id = 0;
//            if (ModelState.IsValid)
//            {
//                var claimsIdentity = (ClaimsIdentity)User.Identity;
//                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
//                if (claims == null) return NotFound();
//                shoppingCart.ApplicationUserId = claims.Value;

//                var shoppingCartInDb = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.ApplicationUserId == claims.Value && sc.ProductId == shoppingCart.ProductId);
//                if (shoppingCartInDb == null)
//                    _unitOfWork.ShoppingCart.Add(shoppingCart);
//                else
//                    shoppingCartInDb.Count += shoppingCart.Count;
//                _unitOfWork.Save();
//                return RedirectToAction("Index");
//            }
//            else
//            {
//                var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingCart.Id, includeProperties: "Category,CoverType");
//                if (productInDb == null) return NotFound();
//                var shoppingCartEdit = new ShoppingCart()
//                {
//                    Product = productInDb,
//                    ProductId = productInDb.Id
//                };
//                return View(shoppingCart);
//            }
//        }
//    }
//}


using EcommProject.Models.ViewModels;
using EcommProject.DataAccess.Repository;
using EcommProject.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using EcommProject.Models;
using System.Security.Claims;

namespace EcommProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Main Index Action to Handle Search and Offer Logic
        public IActionResult Index(string search, string searchTxt)
        {
            // Get products based on search criteria
            IQueryable<Product> productList;

            if (search == "Title")
            {
                productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType")
                    .Where(m => EF.Functions.Like(m.Title, $"%{searchTxt}%"));
            }
            else if (search == "Author")
            {
                productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType")
                    .Where(m => EF.Functions.Like(m.Author, $"%{searchTxt}%"));
            }
            else
            {
                productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType");
            }

            // Map products to the ViewModel with Remaining Time calculated
            var productWithTimes = productList.Select(product => new ProductWithTimeViewModel
            {
                Product = product,
                RemainingTime = product.OfferExpirationTime.HasValue
                    ? product.OfferExpirationTime.Value - DateTime.Now
                    : TimeSpan.Zero // If no expiration time, set to zero
            }).ToList();

            return View(productWithTimes); // Pass the ViewModel to the view
        }

        public IActionResult Science()
        {
            var productList = _unitOfWork.Product
                .GetAlls(includeProperties: "Category,CoverType")
                .Where(m => m.Title == "Science")
                .ToList();
            return View(productList);
        }

        public IActionResult Writing()
        {
            var productList = _unitOfWork.Product.GetAlls(includeProperties: "Category,CoverType")
                .Where(m => m.Title == "Writing").ToList();
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");
            if (productInDb == null) return NotFound();

            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };

            return View(shoppingCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claims == null) return NotFound();

                shoppingCart.ApplicationUserId = claims.Value;
                shoppingCart.Id = 0;

                var shoppingCartInDb = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.ApplicationUserId == claims.Value && sc.ProductId == shoppingCart.ProductId);
                if (shoppingCartInDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartInDb.Count += shoppingCart.Count;

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingCart.ProductId, includeProperties: "Category,CoverType");
                if (productInDb == null) return NotFound();

                shoppingCart.Product = productInDb;
                return View(shoppingCart);
            }
        }
    }
}
