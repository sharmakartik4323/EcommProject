using Dapper;
using EcommProject.DataAccess.Repository.IRepository;
using EcommProject.Models;
using EcommProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            //return Json(new {data= _unitOfWork.Category.GetAll() }); //GetAll is a Generic Method,rightclick Go To Implementation
            return Json(new { data = _unitOfWork.SP_CALL.List<Category>(SD.Proc_GetCategories) });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if (categoryInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data !" });
            // _unitOfWork.Category.Remove(categoryInDb);
            // _unitOfWork.Save();
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id);
            _unitOfWork.SP_CALL.Execute(SD.Proc_DeleteCategory, param);
            return Json(new { success = true, message = "Data successfully deleted !" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null) return View(category); //Create
            //Edit
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id.GetValueOrDefault());//id parameter kaa name hai, jo stored procedure me diya hai
            category = _unitOfWork.SP_CALL.OneRecord<Category>(SD.Proc_GetCategory, param);
            //category = _unitOfWork.Category.Get(id.GetValueOrDefault());  //right click on Get go to implementation
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null) return NotFound();
            if (!ModelState.IsValid) return View(category);
            DynamicParameters param = new DynamicParameters();
            param.Add("name", category.Name);//category.Name se aaega name yhaa, category View hai or Name property hai
            if (category.Id == 0)
                // _unitOfWork.Category.Add(category);
                _unitOfWork.SP_CALL.Execute(SD.Proc_CreateCategory, param);
            else
            {
                //_unitOfWork.Category.Update(category);
                param.Add("id", category.Id);
                _unitOfWork.SP_CALL.Execute(SD.Proc_UpdateCategory, param);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
