using Dapper;
using EcommProject.DataAccess.Repository;
using EcommProject.DataAccess.Repository.IRepository;
using EcommProject.Models;
using EcommProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
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
            //return Json(new { data = _unitOfWork.CoverType.GetAll() }); //GetAll is a Generic Method,rightclick Go To Implementation
            return Json(new { data = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_GetCoverTypes) });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitOfWork.CoverType.Get(id);
            if (coverTypeInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data !" });
            //_unitOfWork.CoverType.Remove(coverTypeInDb);
            //_unitOfWork.Save();
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id);
            _unitOfWork.SP_CALL.Execute(SD.Proc_DeleteCoverType, param);
            return Json(new { success = true, message = "Data successfully deleted !" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null) return View(coverType); //Create
            //Edit
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id.GetValueOrDefault());
            coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
            //coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());  //right click on Get go to implementation
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            DynamicParameters param = new DynamicParameters();
            param.Add("name", coverType.Name);
            if (coverType.Id == 0)
                // _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.SP_CALL.Execute(SD.Proc_CreateCoverType, param);
            else
            {
                //_unitOfWork.CoverType.Update(coverType);
                param.Add("id", coverType.Id);
                _unitOfWork.SP_CALL.Execute(SD.Proc_UpdateCoverType, param);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}