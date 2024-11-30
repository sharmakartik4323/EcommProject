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
        public IEnumerable<ShoppingCart> ListCart { get; set; } 
        public OrderHeader OrderHeader { get; set; }

        //****
        public IEnumerable<SelectListItem> Addresses { get; set; }
        public int SelectedAddressId { get; set; }
    }
}
