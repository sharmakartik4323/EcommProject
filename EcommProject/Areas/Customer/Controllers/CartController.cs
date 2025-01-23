using EcommProject.DataAccess.Repository.IRepository;
using EcommProject.Models;
using EcommProject.Models.ViewModels;
using EcommProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISMSService _sMSService;
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager, ISMSService sMSService)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _sMSService = sMSService;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
                }
            }
            //Email
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent kindly verify your email!";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must be confirm for authorize customer!";
                ViewBag.EmailCSS = "text-danger";
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email Empty!!!");
            }
            else
            {
                //Email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                   "/Account/ConfirmEmail",
                   pageHandler: null,
                   values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //**
                isEmailConfirm = true;
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.Get(id);
            if (cart == null) return NotFound();
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.Get(id);
            if (cart == null) return NotFound();
            if (cart.Count == 1)
                cart.Count = 1;
            else
            {
                cart.Count -= 1;
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.Get(id);
            if (cart == null) return NotFound();
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            //Session Count
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            //****
            return RedirectToAction("Index");
        }
        public IActionResult Summary(string checkBoxInput)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var checkedBoxes = checkBoxInput.Split(',', StringSplitOptions.RemoveEmptyEntries);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value && checkedBoxes.Contains(sc.Id.ToString()), includeProperties: "Product"),
                //****
                Addresses = _unitOfWork.Address.GetAll(a => a.ApplicationUserId == claims.Value).Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Name}, {a.StreetAddress}, {a.City}, {a.State}, {a.PostalCode}, {a.PhoneNumber}"
                }).ToList(),
                //****
                OrderHeader = new OrderHeader()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
                }
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            //****
            //if (shoppingCartVM.SelectedAddressId != null)
            //{
            //    var selectedAddress = _unitOfWork.Address.Get(shoppingCartVM.SelectedAddressId);

            //    shoppingCartVM.OrderHeader.Name = selectedAddress.Name;
            //    shoppingCartVM.OrderHeader.StreetAddress = selectedAddress.StreetAddress;
            //    shoppingCartVM.OrderHeader.City = selectedAddress.City;
            //    shoppingCartVM.OrderHeader.State = selectedAddress.State;
            //    shoppingCartVM.OrderHeader.PostalCode = selectedAddress.PostalCode;
            //    shoppingCartVM.OrderHeader.PhoneNumber = selectedAddress.PhoneNumber;
            //}

            //****

            return View(ShoppingCartVM);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("Summary")]
        //public IActionResult SummaryPost(string stripeToken)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        //    ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);

        //    ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product");

        //    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
        //    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        //    ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;
        //    _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        //    _unitOfWork.Save();

        //    foreach (var list in ShoppingCartVM.ListCart)
        //    {
        //        list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
        //        OrderDetails orderDetails = new OrderDetails()
        //        {
        //            ProductId = list.ProductId,
        //            OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
        //            Price = list.Price,
        //            Count = list.Count,
        //        };
        //        ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
        //        _unitOfWork.OrderDetails.Add(orderDetails);
        //        _unitOfWork.Save();
        //    }
        //    _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
        //    _unitOfWork.Save();
        //    //Session Count
        //    HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);

        //    //Stripe Payment
        //    if (stripeToken == null)
        //    {
        //        ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        //        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
        //        ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
        //    }
        //    else
        //    {
        //        var options = new ChargeCreateOptions()
        //        {
        //            Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
        //            Currency = "usd",
        //            Description = "Order Id : " + ShoppingCartVM.OrderHeader.Id.ToString(),
        //            Source = stripeToken
        //        };
        //        var service = new ChargeService();
        //        Charge charge = service.Create(options);
        //        if (charge.BalanceTransactionId == null)
        //            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
        //        else
        //            ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
        //        if (charge.Status.ToLower() == "succeeded")
        //        {
        //            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
        //            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
        //            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        //        }
        //        _unitOfWork.Save();
        //    }

        //    //****
        //    return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken, string checkBoxInput, int selectedAddressId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product");

            //****
            if (selectedAddressId != 0)
            {
                var selectedAddress = _unitOfWork.Address.Get(selectedAddressId);

                ShoppingCartVM.OrderHeader.Name = selectedAddress.Name;
                ShoppingCartVM.OrderHeader.StreetAddress = selectedAddress.StreetAddress;
                ShoppingCartVM.OrderHeader.City = selectedAddress.City;
                ShoppingCartVM.OrderHeader.State = selectedAddress.State;
                ShoppingCartVM.OrderHeader.PostalCode = selectedAddress.PostalCode;
                ShoppingCartVM.OrderHeader.PhoneNumber = selectedAddress.PhoneNumber;
                //return RedirectToAction("Summary");
            }
            else
            {
                //****

                var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
                var address = new Models.Address
                {
                    Name = ShoppingCartVM.OrderHeader.Name,
                    StreetAddress = ShoppingCartVM.OrderHeader.StreetAddress,
                    State = ShoppingCartVM.OrderHeader.State,
                    City = ShoppingCartVM.OrderHeader.City,
                    PostalCode = ShoppingCartVM.OrderHeader.PostalCode,
                    PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber,
                    ApplicationUserId = user.Id
                };

                var existingAddress = _unitOfWork.Address.FirstOrDefault(a => a.ApplicationUserId == address.ApplicationUserId && a.StreetAddress == address.StreetAddress && a.City == address.City && a.State == address.State && a.PostalCode == address.PostalCode);

                if (existingAddress == null)
                {
                    _unitOfWork.Address.Add(address);
                    _unitOfWork.Save();
                }
                //****

                var checkedBoxes = checkBoxInput.Split(',', StringSplitOptions.RemoveEmptyEntries);

                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;
                _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
                _unitOfWork.Save();

                foreach (var cartItem in ShoppingCartVM.ListCart)
                {
                    if (checkedBoxes.Contains(cartItem.Id.ToString()))
                    {
                        cartItem.Price = SD.GetPriceBasedOnQuantity(cartItem.Count, cartItem.Product.Price, cartItem.Product.Price50, cartItem.Product.Price100);

                        OrderDetails orderDetails = new OrderDetails()
                        {
                            ProductId = cartItem.ProductId,
                            OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                            Price = cartItem.Price,
                            Count = cartItem.Count,
                        };

                        ShoppingCartVM.OrderHeader.OrderTotal += (cartItem.Price * cartItem.Count);

                        _unitOfWork.OrderDetails.Add(orderDetails);
                        _unitOfWork.Save();
                    }
                }
                var itemsToRemove = ShoppingCartVM.ListCart
                    .Where(cartItem => checkedBoxes.Contains(cartItem.Id.ToString()))
                    .ToList();

                _unitOfWork.ShoppingCart.RemoveRange(itemsToRemove);
                _unitOfWork.Save();
                //Session Count
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
                //Stripe Payment
                if (stripeToken == null)
                {
                    ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                }
                else
                {
                    var options = new ChargeCreateOptions()
                    {
                        Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                        Currency = "usd",
                        Description = "Order Id : " + ShoppingCartVM.OrderHeader.Id.ToString(),
                        Source = stripeToken
                    };
                    var service = new ChargeService();
                    Charge charge = service.Create(options);
                    if (charge.BalanceTransactionId == null)
                        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                    else
                        ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                    if (charge.Status.ToLower() == "succeeded")
                    {
                        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                        //ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                        ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
                        ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                    }
                    _unitOfWork.Save();
                }
            }

            //****
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == Claims.Value);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is empty");
            }
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                //$"Your order has been placed successfully. Thank you for shopping with us. Your order ID is #{id}");

                //SMS SENDER
                //string sMSMessage = $"Your order #{id} has been placed successfully. Thank you for shopping with us!";
                //await _sMSService.SendAsync(user.PhoneNumber, sMSMessage);

                //Call
                //await _sMSService.SendCallAsync(user.PhoneNumber,"Your order has been placed successfully. Thank you for shopping with us.");
            }
            return View(id);
        }
    }
}