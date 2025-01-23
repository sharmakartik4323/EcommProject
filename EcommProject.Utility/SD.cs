using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.Utility
{
    public static class SD //static class means object nhi bnega
    {
        //Cover Type SP
        public const string Proc_GetCoverTypes = "GetCoverTypes";  //Proc_GetCoverTypes is name here , name apni merji se rakh sakte hai, GetCoverTypes is name of Stored procedure
        public const string Proc_GetCoverType = "GetCoverType";
        public const string Proc_CreateCoverType = "CreateCoverType";
        public const string Proc_UpdateCoverType = "UpdateCoverType";
        public const string Proc_DeleteCoverType = "DeleteCoverType";

        //Category SP
        public const string Proc_GetCategories = "GetCategories";
        public const string Proc_GetCategory = "GetCategory";
        public const string Proc_CreateCategory = "CreateCategory";
        public const string Proc_UpdateCategory = "UpdateCategory";
        public const string Proc_DeleteCategory = "DeleteCategory";

        //Roles
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";

        //Session
        public const string Ss_CartSessionCount = "Cart Count Session";

        //*****
        public static double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else return price100;
        }
        //Order Status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProgress = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        //Payment Status
        public static string PaymentStatusPending = "Pending";
        public static string PaymentStatusApproved = "Approved";
        public static string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public static string PaymentStatusRejected = "Rejected";
    }
}
