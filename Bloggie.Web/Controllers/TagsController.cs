using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List
            (
            string? searchQuery,
            string? sort = "Asc",
            int pageNumber = 1
            )
        {

            int pageSize = 3;
            var totalRecords = await _tagRepository.CountAsync();
            var totalPage = Math.Ceiling((decimal)totalRecords / pageSize);


            ViewBag.SearchQuery = searchQuery;
            ViewBag.Sort = sort;
            ViewBag.TotalPages = totalPage;
            ViewBag.CurrentPage = pageNumber;

            var tags = await _tagRepository.GetAllAsync(searchQuery, sort, pageNumber, pageSize);
            return View(tags);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTagViewModel addTagRequest)
        {

            ValidateAddTagRequest(addTagRequest);

            if (ModelState.IsValid == false)
            {
                return View();
            }
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };
            await _tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            if (tag != null)
            {
                var editTagRequest = new EditTagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return View(editTagRequest);

            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagViewModel editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };
            var updatedTag = await _tagRepository.UpdateAsync(tag);
            if (updatedTag != null)
            {

            }
            else
            {

            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedTag = await _tagRepository.DeleteAsync(id);
            if (deletedTag != null)
            {
                return RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        private void ValidateAddTagRequest(AddTagViewModel request)
        {
            if (request.Name is not null && request.DisplayName is not null)
            {
                if (request.Name == request.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name cannot be the same as DisplayName");
                }
            }
        }
    }



}
