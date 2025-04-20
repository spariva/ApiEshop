using ApiEshop.Repositories;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe;

namespace ApiEshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController: ControllerBase
    {
        private RepositoryStores repoStores;
        private RepositoryUsers repoUsers;
        //private HelperPathProvider helperPath;

        public StoresController(RepositoryStores repoStores, RepositoryUsers repoUsers)
        {
            this.repoStores = repoStores;
            this.repoUsers = repoUsers;
        }

        #region Stores CRUD
        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore(int id)
        {
            StoreView store = await this.repoStores.FindStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            StoreDto storeDto = new StoreDto()
            {
                Id = store.Store.Id,
                Name = store.Store.Name,
                Email = store.Store.Email,
                Image = store.Store.Image,
                Category = store.Store.Category,
                UserId = store.Store.UserId,
                StripeId = store.Store.StripeId
            };

            List<ProductDto> productDtos = store.Products.Select(p => new ProductDto
            {
                Id = p.Id,
                StoreId = p.StoreId,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Categories = p.ProdCats.Select(pc => new CategoryDto
                {
                    Id = pc.Category.Id,
                    CategoryName = pc.Category.CategoryName
                }).ToList()
            }).ToList();

            // Create the StoreViewDto
            StoreViewDto storeViewDto = new StoreViewDto()
            {
                Store = storeDto,
                Products = productDtos,
                ProductCategories = store.ProdCategories
            };

            return Ok(storeViewDto);
        }

        [HttpGet]
        [Route("SimpleStore/{id}")]
        public async Task<IActionResult> GetSimpleStore(int id)
        {
            StoreView store = await this.repoStores.FindStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            StoreDto storeDto = new StoreDto()
            {
                Id = store.Store.Id,
                Name = store.Store.Name,
                Email = store.Store.Email,
                Image = store.Store.Image,
                Category = store.Store.Category,
                UserId = store.Store.UserId,
                StripeId = store.Store.StripeId
            };

            return Ok(storeDto);
        }

        ////Store create
        //[HttpPost]
        //[Route("Create")]
        //public async Task<IActionResult> CreateStore([FromBody] StoreDto storeDto)
        //{
        //    if (storeDto == null)
        //    {
        //        return BadRequest();
        //    }

        //    await this.repoStores.CreateStoreAsync(store);
        //    return CreatedAtAction(nameof(GetStore), new { id = store.Id }, store);
        //}


        //[HttpGet("Stores/OnboardingComplete/{id}")]
        //public async Task<IActionResult> OnboardingComplete(int id)
        //{
        //    var store = await this.repoStores.FindSimpleStoreAsync(id);
        //    if (store == null)
        //    {
        //        return NotFound();
        //    }

        //    // Verify this user owns the store
        //    if (store.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
        //    {
        //        return Forbid();
        //    }

        //    // Show success page or redirect to store dashboard
        //    return RedirectToAction("StoreDetails", new { id = store.Id });
        //}

        //[HttpGet("Stores/RefreshOnboarding/{id}")]
        //public async Task<IActionResult> RefreshOnboarding(int id)
        //{
        //    Store store = await this.repoStores.FindSimpleStoreAsync(id);
        //    if (store == null)
        //    {
        //        return NotFound();
        //    }

        //    // Verify this user owns the store
        //    if (store.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
        //    {
        //        return Forbid();
        //    }

        //    // Create a new account link
        //    var accountLinkService = new AccountLinkService();
        //    var accountLink = accountLinkService.Create(new AccountLinkCreateOptions
        //    {
        //        Account = store.StripeId,
        //        ReturnUrl = Url.Action("OnboardingComplete", "Stores", new { id = store.Id }, Request.Scheme),
        //        RefreshUrl = Url.Action("RefreshOnboarding", "Stores", new { id = store.Id }, Request.Scheme),
        //        Type = "account_onboarding",
        //    });

        //    return Redirect(accountLink.Url);
        //}









        #endregion
    }
}
