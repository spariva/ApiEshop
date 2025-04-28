using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using Product = ApiEshop.Models.Product;
using ApiEshop.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ApiEshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private RepositoryStores repoStores;
        private RepositoryUsers repoUsers;
        private RepositoryPayments repoPay;
        private readonly IMapper mapper;


        public UsersController(RepositoryStores repoStores, RepositoryUsers repoUsers, RepositoryPayments repoPay, IMapper mapper)
        {
            this.repoStores = repoStores;
            this.repoUsers = repoUsers;
            this.repoPay = repoPay;
            this.mapper = mapper;
        }

        //Podría hacer que el dto use el dto user etc y empezar a ocultar info
        [Authorize]
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> Profile(int id,
            [FromQuery] bool includeStore = true,
            [FromQuery] bool includePurchases = true)
        {
            User user = await this.repoUsers.FindUserAsync(id);
            if (user == null)
            {
                return NotFound("User not found =(");
            }

            ProfileDto profileDto = new ProfileDto { User = user };

            if (includeStore)
            {
                profileDto.Store = await this.repoUsers.FindStoreByUserIdAsync(id);

            }

            if (includePurchases)
            {
                var purchases = await this.repoPay.GetPurchasesByUserIdAsync(id);
                if (purchases.Count > 0)
                {
                    profileDto.Purchases = purchases;
                }
            }

            return Ok(profileDto);
        }


        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> FindStoreByUser(int id)
        {
            Store store = await this.repoUsers.FindStoreByUserIdAsync(id);
            if (store == null)
            {
                return NotFound("Store not found =(");
            }
            StoreDto storeDto = this.mapper.Map<StoreDto>(store);
            return Ok(storeDto);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> PurchaseDetails(int id)
        {
            Purchase purchase = await this.repoPay.GetPurchaseByIdAsync(id);
            if (purchase == null)
            {
                return NotFound("Purchase not found =(");
            }

            foreach (PurchaseItem item in purchase.PurchaseItems)
            {
                item.Product = await this.repoStores.FindProductAsync(item.ProductId);
            }

            PurchaseDto p = this.mapper.Map<PurchaseDto>(purchase);
            return Ok(p);
        }


    }
}
