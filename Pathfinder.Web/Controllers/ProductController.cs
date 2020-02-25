using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Core.Paging;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Models;
using System.Net;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [Produces("application/json")]
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        {
            var products = await productService.GetProductList();
            return Ok(products);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<ProductModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IPagedList<ProductModel>>> SearchProducts(PageSearchArgs arg)
        {
            var productPagedList = await productService.SearchProducts(arg);
            return Ok(productPagedList);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(ProductModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModel>> GetProductById(int id)
        {
            var product = await productService.GetProductById(id);
            return Ok(product);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductsByName(string name)
        {
            var products = await productService.GetProductsByName(name);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductsByCategoryId(int categoryId)
        {
            var products = await productService.GetProductsByCategoryId(categoryId);
            return Ok(products);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ProductModel>> CreateProduct(ProductModel product)
        {
            var Result = await productService.CreateProduct(product);
            return Ok(Result);
        }

        [Route("[action]")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateProduct(ProductModel product)
        {
            await productService.UpdateProduct(product);
            return Ok();
        }

        [Route("[action]")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteProductById(ProductModel product)
        {
            await productService.DeleteProductById(product.Id);
            return Ok();
        }
    }
}