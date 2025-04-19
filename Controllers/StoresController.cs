using ApiEshop.Repositories;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            StoreViewDto store = await this.repoStores.FindStoreAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }







        #endregion
    }
}
