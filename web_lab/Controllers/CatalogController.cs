using Auth.Service;
using DAL;
using Domain.Enum;
using Domain.Helpers;
using Domain.Models;
using Domain.Services;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using web_lab.Models;
using Web_lab.Models;

namespace web_lab.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly AccountService _accountService;
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        public CatalogController(ICatalogRepository catalogRepository, AccountService accountService, IFileService fileService, ILogService logService)
        {
            _catalogRepository = catalogRepository;
            _accountService = accountService;
            _fileService = fileService;
            _logService = logService;
        }
        public IActionResult Index()
        {
            CatalogFilterModel model = new CatalogFilterModel
            {
                CitiesList = EnumHelper.ToSelectList<CityEnum>(),
                BeachTypesList = EnumHelper.ToSelectList<BeachTypeEnum>()
            };

            return View(model);
        }

        public JsonResult GetItems(CatalogFilterModel? filter)
        {
            var items = _catalogRepository.Select(new DAL.Filters.CatalogItemFilter
            {
                City = filter.City,
                BeachType = filter.BeachType,
                StarRating = filter.StarRating,
                IsAllInclusive = filter.IsClearFilter ? null : filter.IsAllInclusive
            });

            return Json(items);
        }

        [HttpGet]
        public IActionResult InsertGet()
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();

            var model = new CatalogItemCreateGetModel
            {
                CitiesList = EnumHelper.ToSelectList<CityEnum>(),
                BeachTypesList = EnumHelper.ToSelectList<BeachTypeEnum>()
            };

            return View("~/Views/Catalog/CreateItem.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertPost(CatalogItemCreatePostModel model)
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();

            var fileName = await _fileService.SaveCatalogItemImageAsync(model.Image);
            if (fileName == null)
                return Json(new { Success = false, Message = "Произошла ошибка при сохранении файла." });

            var item = new CatalogItem
            {
                Name = model.Name,
                Description = model.Description,
                City = model.City,
                BeachType = model.BeachType,
                StarRating = model.StarRating,
                IsAllInclusive = model.IsAllInclusive,
                Url = $"/assets/{fileName}"
            };

            try
            {
                _catalogRepository.Insert(item);
            }
            catch (Exception ex)
            {
                _logService.Log("InsertPost", "Catalog", $"Ошибка при добавлении элемента каталога {item.Name} в базу данных: {ex.Message}", LoggingLevel.Error.ToString(), token);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("InsertPost", "Catalog", $"Новый элемент каталога {item.Name} добавлен в базу данных", LoggingLevel.Success.ToString(), token);
            return Json(new { Success = true });
        }

        [HttpGet]
        public IActionResult EditGet(int itemId)
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();

            var item = _catalogRepository.Select(new DAL.Filters.CatalogItemFilter { Id = itemId }).FirstOrDefault();
            if (item == null)
                return NotFound();

            var model = new CatalogItemEditGetModel
            {
                Id = itemId,
                Name = item.Name,
                Description = item.Description,
                City = item.City,
                BeachType = item.BeachType,
                StarRating = item.StarRating,
                IsAllInclusive = item.IsAllInclusive == null ? false : (bool)item.IsAllInclusive,
                CitiesList = EnumHelper.ToSelectList<CityEnum>(),
                BeachTypesList = EnumHelper.ToSelectList<BeachTypeEnum>()
            };

            return View("~/Views/Catalog/EditItem.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(CatalogItemEditPostModel model)
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();

            string? fileName = null;
            if (model.Image != null)
            {
                fileName = await _fileService.SaveCatalogItemImageAsync(model.Image);
                if (fileName == null)
                    return Json(new { Success = false, Message = "Произошла ошибка при сохранении файла." });
            }

            var item = _catalogRepository.Select(new DAL.Filters.CatalogItemFilter { Id = model.Id }).FirstOrDefault();
            if (item == null)
                return NotFound();

            var editItem = new CatalogItem
            {
                Id = model.Id,
                Name = model.Name != null ? model.Name : item.Name,
                Description = model.Description != null ? model.Description : item.Description,
                City = model.City != null ? model.City : item.City,
                BeachType = model.BeachType != null ? model.BeachType : item.BeachType,
                StarRating = model.StarRating != null ? model.StarRating : item.StarRating,
                IsAllInclusive = model.IsAllInclusive != null ? model.IsAllInclusive : item.IsAllInclusive,
                Url = fileName != null ? $"/assets/{fileName}" : item.Url,
            };

            try
            {
                _catalogRepository.Edit(editItem);
            }
            catch (Exception ex)
            {
                _logService.Log("EditPost", "Catalog", $"Ошибка при изменении элемента каталога с ID {model.Id} в базе данных: {ex.Message}", LoggingLevel.Error.ToString(), token);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("EditPost", "Catalog", $"Элемент каталога с ID {model.Id} успешно изменен в базе данных", LoggingLevel.Success.ToString(), token);
            return Json(new { Success = true });
        }

        [HttpDelete]
        public IActionResult Delete(int itemId)
        {
            var token = Request.Cookies["authToken"];
            if (token == null)
                return Unauthorized();

            var userAccount = _accountService.GetAccount(token);
            if (userAccount == null || userAccount.IsAdmin == false)
                return NotFound();

            var item = _catalogRepository.Select(new DAL.Filters.CatalogItemFilter { Id = itemId }).FirstOrDefault();
            if (item == null)
                return NotFound();

            try
            {
                _catalogRepository.Delete(itemId);
            }
            catch (Exception ex)
            {
                _logService.Log("Delete", "Catalog", $"Ошибка при удалении элемента каталога с ID {itemId} из базы данных: {ex.Message}", LoggingLevel.Error.ToString(), token);
                return Json(new { Success = false, Message = ex.Message });
            }

            _logService.Log("Delete", "Catalog", $"Элемент каталога с ID {itemId} успешно удалён из базы данных", LoggingLevel.Success.ToString(), token);
            return Json(new { Success = true });
        }
    }
}
