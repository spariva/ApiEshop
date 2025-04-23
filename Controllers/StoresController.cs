using ApiEshop.Repositories;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe;
using AutoMapper;

namespace ApiEshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController: ControllerBase
    {
        private RepositoryStores repoStores;
        private RepositoryUsers repoUsers;
        private readonly IMapper mapper;
        //private HelperPathProvider helperPath;

        public StoresController(RepositoryStores repoStores, RepositoryUsers repoUsers, IMapper mapper)
        {
            this.repoStores = repoStores;
            this.repoUsers = repoUsers;
            this.mapper = mapper;
        }

        #region Stores CRUD
        [HttpGet]
        public async Task<ActionResult> GetStores()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            List<StoreDto> storesDto = this.mapper.Map<List<StoreDto>>(stores);
            return Ok(storesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetStore(int id)
        {
            StoreView store = await this.repoStores.FindStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            StoreViewDto storeViewDto = new StoreViewDto
            {
                Store = this.mapper.Map<StoreDto>(store.Store),
                Products = this.mapper.Map<List<ProductDto>>(store.Products),
                ProductCategories = store.ProdCategories
            };

            return Ok(storeViewDto);
        }

        [HttpGet]
        [Route("SimpleStore/{id}")]
        public async Task<ActionResult> GetSimpleStore(int id)
        {
            Store store = await this.repoStores.FindSimpleStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            StoreDto storeDto = this.mapper.Map<StoreDto>(store);

            return Ok(storeDto);
        }

        //Store create, stripe etc handled on mvc so I send a model instead a dto
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> CreateStore(Store store)
        {
            Store s = await this.repoStores.CreateStoreAsync(store.Name, store.Email, store.Image, store.Category, store.UserId, store.StripeId);
            return Ok(s);
        }


        [HttpGet]
        [Route("OnboardingComplete/{id}")]
        public async Task<ActionResult> OnboardingComplete(int id)
        {
            var store = await this.repoStores.FindSimpleStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            // Verify this user owns the store
            if (store.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Forbid();
            }

            // Show success page or redirect to store dashboard
            return RedirectToAction("StoreDetails", new { id = store.Id });
        }

        [HttpGet]
        [Route("Stores/RefreshOnboarding/{id}")]
        public async Task<ActionResult> RefreshOnboarding(int id)
        {
            Store store = await this.repoStores.FindSimpleStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            // Verify this user owns the store
            if (store.UserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Forbid();
            }

            // Create a new account link
            var accountLinkService = new AccountLinkService();
            var accountLink = accountLinkService.Create(new AccountLinkCreateOptions
            {
                Account = store.StripeId,
                ReturnUrl = Url.Action("OnboardingComplete", "Stores", new { id = store.Id }, Request.Scheme),
                RefreshUrl = Url.Action("RefreshOnboarding", "Stores", new { id = store.Id }, Request.Scheme),
                Type = "account_onboarding",
            });

            return Redirect(accountLink.Url);
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<ActionResult> UpdateStore(int id, StoreDto s)
        {

            var result = await this.repoStores.UpdateStoreAsync(id, s.Name, s.Email, s.Image, s.Category);

            if (result == null)
            {
                return NotFound();
            }

            StoreDto store = this.mapper.Map<StoreDto>(s);

            return Ok(store);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> DeleteStore(int id)
        {
            await this.repoStores.DeleteStoreAsync(id);

            return Ok();
        }









        #endregion
    }
}
