using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork  //Interface is public so members will be public byDefault
    {
        ICategoryRepository Category { get; } //This is property, Category naam rakh diya
        ICoverTypeRepository CoverType { get; }
        ISP_CALL SP_CALL { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IAddressRepository Address { get; }
        void Save();
    }
}
