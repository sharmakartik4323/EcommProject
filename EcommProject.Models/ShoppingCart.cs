using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }//Column Name
        [ForeignKey("ApplicationUserId")]//ApplicationUser Table ke Id Column se foreign key bnegi
        public ApplicationUser ApplicationUser { get; set; }
        public int  ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get; set; }
        [NotMapped]
        public double Price { get; set; } //Price fix nhi hai isliye price ko NotMapped rkha
        public bool IsSelected { get; set; }
        public bool IsRemoveFromCart { get; set; }
    }
}
