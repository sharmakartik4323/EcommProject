using EcommProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository:IRepository<Category> //IRepository generic repository hai usme pass kiya Category
    {
    }
}
