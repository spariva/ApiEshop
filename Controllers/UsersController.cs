using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using Product = ApiEshop.Models.Product;
using ApiEshop.Repositories;
using AutoMapper;

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
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Profile(int id,
            [FromQuery]bool includeStore=true,
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

        //[HttpGet]
        //[Route("[action]/{id}")]
        //public async Task<IActionResult> PurchaseDetails(int id)
        //{
        //    Purchase purchase = await this.repoPay.GetPurchaseByIdAsync(id);
        //    if (purchase == null)
        //    {
        //        return NotFound("Purchase not found =(");
        //    }

        //    foreach (PurchaseItem item in purchase.PurchaseItems)
        //    {
        //        item.Product = await this.repoStores.FindProductAsync(item.ProductId);
        //    }

        //    PurchaseDto purchaseDto = new PurchaseDto
        //    {
        //        Id = purchase.Id,
        //        UserId = purchase.UserId,
        //        TotalPrice = purchase.TotalPrice,
        //        PaymentStatus = purchase.PaymentStatus,
        //        CreatedAt = purchase.CreatedAt,
        //        Items = purchase.PurchaseItems.Select(item => new PurchaseItemDto
        //        {
        //            Id = item.Id,
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity,
        //            Price = item.PriceAtPurchase,
        //            Product = new ProductDto
        //            {
        //                Id = item.Product.Id,
        //                Name = item.Product.Name,
        //                Description = item.Product.Description,
        //                Price = item.Product.Price,
        //                StockQuantity = item.Product.StockQuantity,
        //                ImageUrl = item.Product.ImageUrl
        //            }
        //        }).ToList()

        //    };
        //    // AAAA haz el automapper ya, y estos dtos están mal, pone en purchaseitemdto solo el name de product
        //    return Ok(purchase);
        //}




    }
}
