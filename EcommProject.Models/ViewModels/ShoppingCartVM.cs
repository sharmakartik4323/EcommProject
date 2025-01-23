using EcommProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; } //Jis user ne Login kiya uske Cart me kon kon se items hai,Example Vikas ne login kiya uske Cart me 3 items hai toh isme 3 record aaege
        public OrderHeader OrderHeader { get; set; }

        //****
        public IEnumerable<SelectListItem> Addresses { get; set; }
        public int SelectedAddressId { get; set; }
    }
}