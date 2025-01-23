using EcommProject.DataAccess.Data;
using EcommProject.DataAccess.Repository.IRepository;
using EcommProject.Models.ViewModels;
using EcommProject.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using EcommProject.DataAccess.Repository;
using EcommProject.Utility;
using Stripe.Billing;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Stripe.Climate;

namespace EcommProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly ISMSService _sMSService;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderManagementController(IUnitOfWork unitOfWork, IEmailSender emailSender, ISMSService sMSService, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _sMSService = sMSService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll()
        {
            var orderlist = _unitOfWork.OrderHeader.GetAll();
            return Json(new { Data = orderlist });
        }
        public IActionResult PendingOrders()
        {
            var pendingOrders = _unitOfWork.OrderHeader.GetAll(
             or => or.OrderStatus == SD.OrderStatusPending);
            return Json(new { Data = pendingOrders });
        }
        public IActionResult Pending()
        {
            return View();
        }
        public IActionResult ApprovedOrders()
        {
            var approvedOrders = _unitOfWork.OrderHeader.GetAll(
                or => or.OrderStatus == SD.PaymentStatusApproved);
            return Json(new { Data = approvedOrders });
        }
        public IActionResult Approved()
        {
            return View();
        }
        public IActionResult CancelledOrders()
        {
            var cancelled = _unitOfWork.OrderHeader.GetAll(os => os.OrderStatus == SD.OrderStatusCancelled);
            return Json(new { Data = cancelled });
        }
        public IActionResult Cancelled()
        {
            return View();
        }
        public IActionResult RefundedOrders()
        {
            var refunded = _unitOfWork.OrderHeader.GetAll(os => os.OrderStatus == SD.OrderStatusRefunded);
            return Json(new { Data = refunded });
        }
        public IActionResult Refunded()
        {
            return View();
        }
        public IActionResult ProcessingOrders()
        {
            var processing = _unitOfWork.OrderHeader.GetAll(os => os.OrderStatus == SD.OrderStatusInProgress);
            return Json(new { Data = processing });
        }
        public IActionResult Processing()
        {
            return View();
        }
        public IActionResult ShippedOrders()
        {
            var shipped = _unitOfWork.OrderHeader.GetAll(os => os.OrderStatus == SD.OrderStatusShipped);
            return Json(new { Data = shipped });
        }
        public IActionResult Shipped()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderDetails orderDetails = new OrderDetails();
            orderDetails = _unitOfWork.OrderDetails.FirstOrDefault(or => or.OrderHeaderId == id, includeProperties: "Product,OrderHeader.ApplicationUser,OrderHeader,Product.CoverType,Product.Category");
            if (orderDetails == null) return NotFound();
            return View(orderDetails);
        }
        public IActionResult ByDateTime(DateTime startDate, DateTime endDate)
        {
            var filter = _unitOfWork.OrderHeader.GetAll(or => or.OrderDate >= startDate && or.OrderDate <= endDate).ToList();
            return View(filter);
        }
        public async Task<IActionResult> OrdersConfirmation(string id)
        {
            //var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            //var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == id.ToString());

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
                //    $"Your confirmation messages send successfully. Thank you for shopping with us.");

                ////SMS SENDER
                //string sMSMessage = $"Your confirmation messages has been sent successfully. Thank you for shopping with us!";
                //await _sMSService.SendAsync(user.PhoneNumber, sMSMessage);

                ////Call
                //await _sMSService.SendCallAsync(user.PhoneNumber, "Your confirmation messages has been sent successfully. Thank you for shopping with us.");

            }
            return View();
        }

        //--------------
        public IActionResult ApproveOrder(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.FirstOrDefault(o => o.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            // Update order status to Approved
            orderHeader.OrderStatus = SD.PaymentStatusApproved;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }
        
        public IActionResult CancelOrder(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.FirstOrDefault(o => o.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            // Update order status to Cancelled
            orderHeader.OrderStatus = SD.OrderStatusCancelled;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }
    }
}