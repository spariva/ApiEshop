using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using Product = ApiEshop.Models.Product;
using ApiEshop.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using System.Security.Claims;
using AutoMapper;


namespace ApiEshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController: ControllerBase
    {
        private RepositoryStores repoStores;
        private RepositoryUsers repoUsers;
        private readonly IMapper mapper;
        //private HelperPathProvider helperPath;

        public ProductsController(RepositoryStores repoStores, RepositoryUsers repoUsers, IMapper mapper)
        {
            this.repoStores = repoStores;
            this.repoUsers = repoUsers;
            this.mapper = mapper;
            //this.helperPath = helperPath;
        }


        #region Products CRUD
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            List<Product> products = await this.repoStores.GetAllProductsAsync();
            List<ProductDto> productsDto = this.mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            Product product = await this.repoStores.FindProductAsync(id);
            ProductDto p = this.mapper.Map<ProductDto>(product);
            return Ok(p);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Categories()
        {
            List<Category> categories = await this.repoStores.GetAllCategoriesAsync();
            List<CategoryDto> c = this.mapper.Map<List<CategoryDto>>(categories);
            return Ok(categories);  
        }

        [HttpPost]
        [Route("[action]/{id}")
        public async Task<IActionResult> Create(ProductDto p)
        {
            Product product = await this.repoStores.CreateProductAsync(p.Name, p.StoreId, p.Description, p.Image, p.Price, p.StockQuantity, p.Categories);

        }

        //[AuthorizeUser]
        //public async Task<IActionResult> ProductCreate()
        //{
        //    List<Category> categories = await this.repoStores.GetAllCategoriesAsync();
        //    ViewBag.Productcategories = categories.Select(c => new SelectListItem
        //    {
        //        Value = c.Id.ToString(),
        //        Text = c.CategoryName
        //    }).ToList();

        //    return View();
        //}

        //[HttpPost]
        //[AuthorizeUser]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ProductCreate(string name, string description, IFormFile image, decimal price, int stockQuantity, List<int> selectedCategories, string newCategories)
        //{
        //    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    Store store = await this.repoUsers.FindStoreByUserIdAsync(userId);

        //    if (store == null)
        //    {
        //        TempData["Message"] = "Create a store before!";
        //        return RedirectToAction("Profile", "Users");
        //    }

        //    int storeId = store.Id;

        //    if (ModelState.IsValid)
        //    {
        //        // Save the image 
        //        string fileName = image.FileName;
        //        string path = this.helperPath.MapPath(fileName, Folder.Products);
        //        using (Stream stream = new FileStream(path, FileMode.Create))
        //        {
        //            await image.CopyToAsync(stream);
        //        }

        //        // Handle new categories
        //        if (!string.IsNullOrEmpty(newCategories))
        //        {
        //            var newCategoryNames = newCategories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim().ToUpper()).ToList();
        //            foreach (var categoryName in newCategoryNames)
        //            {
        //                var category = await this.repoStores.FindOrCreateCategoryAsync(categoryName);
        //                selectedCategories.Add(category.Id);
        //            }
        //        }

        //        // Insert the product
        //        var product = await this.repoStores.CreateProductAsync(name, storeId, description, fileName, price, stockQuantity, selectedCategories);

        //        return RedirectToAction("ProductDetails", new { id = product.Id });
        //    }


        //    // If we got this far, something failed; re-populate the categories
        //    var categories = await this.repoStores.GetAllCategoriesAsync();
        //    ViewBag.Productcategories = categories.Select(c => new SelectListItem
        //    {
        //        Value = c.Id.ToString(),
        //        Text = c.CategoryName
        //    }).ToList();
        //    ViewBag.Mensaje = "Error en el formulario model state is not valid";

        //    return View();
        //}

        //[AuthorizeUser]
        //public async Task<IActionResult> ProductEdit(int id)
        //{
        //    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    Store store = await this.repoUsers.FindStoreByUserIdAsync(userId);

        //    if (store == null)
        //    {
        //        TempData["Message"] = "Create a store before!";
        //        return RedirectToAction("Profile", "Users");
        //    }

        //    Product product = await this.repoStores.FindProductAsync(id);

        //    if (product.StoreId != store.Id)
        //    {
        //        TempData["Message"] = "That was not your product to Edit";
        //        return RedirectToAction("Profile", "Users");
        //    }

        //    List<Category> categories = await this.repoStores.GetAllCategoriesAsync();
        //    ViewBag.Productcategories = categories.Select(c => new SelectListItem
        //    {
        //        Value = c.Id.ToString(),
        //        Text = c.CategoryName
        //    }).ToList();

        //    return View(product);
        //}

        //[HttpPost]
        //[AuthorizeUser]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ProductEdit(int id, string name, string description, IFormFile image, string oldimage, decimal price, int stockQuantity, List<int> selectedCategories, string newCategories)
        //{
        //    if (!string.IsNullOrEmpty(newCategories))
        //    {
        //        var newCategoryNames = newCategories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim().ToUpper()).ToList();
        //        foreach (var categoryName in newCategoryNames)
        //        {
        //            var category = await this.repoStores.FindOrCreateCategoryAsync(categoryName);
        //            selectedCategories.Add(category.Id);
        //        }
        //    }

        //    if (image != null)
        //    {
        //        string fileName = image.FileName;
        //        string path = this.helperPath.MapPath(fileName, Folder.Products);
        //        using (Stream stream = new FileStream(path, FileMode.Create))
        //        {
        //            await image.CopyToAsync(stream);
        //        }

        //        await this.repoStores.UpdateProductAsync(id, name, description, fileName, price, stockQuantity, selectedCategories);
        //    }
        //    else
        //    {
        //        await this.repoStores.UpdateProductAsync(id, name, description, oldimage, price, stockQuantity, selectedCategories);
        //    }
        //    return RedirectToAction("ProductDetails", new { id = id });


        //    // If we got this far, something failed; re-populate the categories? TODO
        //}

        ////First I find the product to get the id, so I pass the Product to not call twice the database
        //[AuthorizeUser]
        //public async Task<IActionResult> ProductDelete(int id)
        //{
        //    Product p = await this.repoStores.FindProductAsync(id);
        //    int storeId = p.StoreId;

        //    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    Store store = await this.repoUsers.FindStoreByUserIdAsync(userId);

        //    if (store == null)
        //    {
        //        TempData["Message"] = "Create a store before!";
        //        return RedirectToAction("Profile", "User");
        //    }

        //    if (p.StoreId != store.Id)
        //    {
        //        TempData["Message"] = "That was not your product to Delete";
        //        return RedirectToAction("Profile", "Users");
        //    }

        //    await this.repoStores.DeleteProductAsync(p);
        //    return RedirectToAction("StoreDetails", storeId);
        //}



        #endregion
    }
}
