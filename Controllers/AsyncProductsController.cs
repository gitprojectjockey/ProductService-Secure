using EDataLayer.Core.Domain;
using EDataLayer.Core.EUnitOfWork.Async.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading.Tasks;

namespace ProductService.Controllers
{

    [EnableCors(origins: "http://localhost:50617", headers: "*", methods: "*")]
    [RoutePrefix("async/api/products")]
    public class AsyncProductsController : ApiController
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        public AsyncProductsController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(System.Threading.CancellationToken cancellationToken)
        {
            IEnumerable<Product> products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, products, System.Net.HttpStatusCode.OK);
        }

        [Route("{id}")]
        [HttpGet]
        [MyFilters.CheckForNullParameter]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            Product product = await _unitOfWork.Products.GetAsync(id);
            if (product != null)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, product, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Guid>(Request, product.ProductId, System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("GetByName/{name}")]
        [HttpGet]
        [MyFilters.CheckForNullParameter]
        public async Task<IHttpActionResult> GetByName(string name)
        {
            var foundProduct = await _unitOfWork.Products.GetProductByNameAsync(name);
            if (foundProduct != null)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, foundProduct, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<string>(Request, name, System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("GetByName")]
        [HttpPost]
        [MyFilters.CheckForNullParameter]
        [MyFilters.ModelStateValidator]
        public async Task<IHttpActionResult> GetByName([FromBody]Product product)
        {
            var foundProduct = await _unitOfWork.Products.GetProductByNameAsync(product.ProductName);
            if (foundProduct != null)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, product, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<string>(Request, product.ProductName, System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("GetByRange")]
        [HttpPost]
        [MyFilters.CheckForNullParameter]
        public async Task<IHttpActionResult> GetByRange([FromBody]IEnumerable<Guid> ids)
        {
            var foundProducts = await _unitOfWork.Products.GetProductsByRangeAsync(ids);

            if (foundProducts.Count() > 0)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, foundProducts, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Guid>>(Request, ids, System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("")]
        [HttpPost]
        [MyFilters.CheckForNullParameter]
        [MyFilters.ModelStateValidator]
        public async Task<IHttpActionResult> Post([FromBody]Product product)
        {
            var productExists = await _unitOfWork.Products.ProductExistsAsync(product);

            if (productExists)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<string>(Request, null, System.Net.HttpStatusCode.Conflict);
            }
            else
            {
                _unitOfWork.Products.Add(product);
                await _unitOfWork.CompleteAsync();
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, product, System.Net.HttpStatusCode.OK);
            }
        }

        [Route("PostRange")]
        [HttpPost]
        [MyFilters.CheckForNullParameter]
        [MyFilters.GenericModelListStateValidator("products", typeof(Product))]
        public async Task<IHttpActionResult> Post([FromBody]IEnumerable<Product> products)
        {
            var productsExist = await _unitOfWork.Products.ProductRangeExistsAsync(products);

            if (productsExist)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<string>(Request, null, System.Net.HttpStatusCode.Conflict);
            }
            else
            {
                _unitOfWork.Products.AddRange(products);
                await _unitOfWork.CompleteAsync();
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, products, System.Net.HttpStatusCode.OK);
            }
        }

        [Route("")]
        [HttpPut]
        [MyFilters.CheckForNullParameter]
        [MyFilters.ModelStateValidator]
        public async Task<IHttpActionResult> Put([FromBody]Product product)
        {
            Product productToUpdate = null;
            var productExists = await _unitOfWork.Products.ProductExistsAsync(product);

            if (productExists)
            {
                productToUpdate = await _unitOfWork.Products.SingleOrDefaultAsync(p => p.ProductId == product.ProductId) as Product;
                productToUpdate.ProductName = product.ProductName;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;
                productToUpdate.CompanyId = product.CompanyId;
                productToUpdate.ProductCatagoryId = product.ProductCatagoryId;

                _unitOfWork.Products.Update(productToUpdate);
                await _unitOfWork.CompleteAsync();
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, productToUpdate, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Guid>(Request, productToUpdate.ProductId, System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("PutRange")]
        [HttpPut]
        [MyFilters.CheckForNullParameter]
        [MyFilters.GenericModelListStateValidator("products", typeof(Product))]
        public async Task<IHttpActionResult> Put([FromBody]IEnumerable<Product> products)
        {
            var allProductsExist = await _unitOfWork.Products.ProductRangeExistsAsync(products);

            if (allProductsExist)
            {
                var productsToUpdate = await _unitOfWork.Products.GetProductsByRangeAsync(products);

                foreach (var productToUpdate in productsToUpdate)
                {
                    productToUpdate.ProductName = products.Single(p => p.ProductId == productToUpdate.ProductId).ProductName;
                    productToUpdate.Description = products.Single(p => p.ProductId == productToUpdate.ProductId).Description;
                    productToUpdate.Price = products.Single(p => p.ProductId == productToUpdate.ProductId).Price;
                    productToUpdate.CompanyId = products.Single(p => p.ProductId == productToUpdate.ProductId).CompanyId;
                    productToUpdate.ProductCatagoryId = products.Single(p => p.ProductId == productToUpdate.ProductId).ProductCatagoryId;
                    _unitOfWork.Products.Update(productToUpdate);
                }

                await _unitOfWork.CompleteAsync();
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, products, System.Net.HttpStatusCode.OK);

            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, products, System.Net.HttpStatusCode.NotFound);
            }
        }

        [Route("{id:guid}")]
        [HttpDelete]
        [MyFilters.CheckForNullParameter]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            var productToDelete = await _unitOfWork.Products.SingleOrDefaultAsync(p => p.ProductId == id) as Product;
            if (productToDelete != null)
            {
                _unitOfWork.Products.Remove(productToDelete);
                await _unitOfWork.CompleteAsync();
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, productToDelete, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Guid>(Request, id, System.Net.HttpStatusCode.NotFound); ;
            }
        }

        [Route("DeleteRange")]
        [HttpDelete]
        [MyFilters.CheckForNullParameter]
        public async Task<IHttpActionResult> DeleteRange([FromBody]IEnumerable<Guid> ids)
        {
            var productsToDelete = await _unitOfWork.Products.GetProductsByRangeAsync(ids);
            if (productsToDelete.Count() > 0)
            {
                _unitOfWork.Products.RemoveRange(productsToDelete);
               await  _unitOfWork.CompleteAsync();
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, productsToDelete, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Guid>>(Request, ids, System.Net.HttpStatusCode.NotFound); ;
            }
        }
    }
}
