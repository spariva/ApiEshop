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

        #region Categories
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> Categories()
        {
            List<Category> categories = await this.repoStores.GetAllCategoriesAsync();
            List<CategoryDto> c = this.mapper.Map<List<CategoryDto>>(categories);
            return Ok(c);
        }

        //Returns a new/existent category, given the name of the category
        [HttpPost]
        [Route("[action]/{name}")]
        public async Task<ActionResult> FindorCreateCategory(string name)
        {
            Category category = await this.repoStores.FindOrCreateCategoryAsync(name);
            CategoryDto c = this.mapper.Map<CategoryDto>(category);
            return Ok(c);
        }
        #endregion

        #region Products CRUD
        [HttpGet]
        public async Task<ActionResult> ProductList()
        {
            List<Product> products = await this.repoStores.GetAllProductsAsync();
            List<ProductDto> productsDto = this.mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<ActionResult> ProductDetails(int id)
        {
            Product product = await this.repoStores.FindProductAsync(id);
            ProductDto p = this.mapper.Map<ProductDto>(product);
            return Ok(p);
        }

        [HttpPost]
        [Route("[action]/{id}")]
        public async Task<ActionResult> Create(ProductDto p)
        {
            List<int> c = p.Categories.Select(c => c.Id).ToList();
            Product product = await this.repoStores.CreateProductAsync(p.Name, p.StoreId, p.Description, p.Image, p.Price, p.StockQuantity, c);
            ProductDto pDto = this.mapper.Map<ProductDto>(product);
            return Ok(pDto);
        }

        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<ActionResult> Update(int id, ProductDto p)
        {
            List<int> c = p.Categories.Select(c => c.Id).ToList();
            Product product = await this.repoStores.UpdateProductAsync(id, p.Name, p.Description, p.Image, p.Price, p.StockQuantity, c);
            ProductDto pDto = this.mapper.Map<ProductDto>(product);
            return Ok(pDto);
        }

        [HttpDelete]
        [Route("[action]/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Product p = await this.repoStores.FindProductAsync(id);
            if(p == null)
            {
                return NotFound("Product not found");
            }
            await this.repoStores.DeleteProductAsync(p);
            return Ok();
        }

        #endregion
    }
}
