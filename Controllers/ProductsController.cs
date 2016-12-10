using EDataLayer.Core.Domain;
using EDataLayer.Core.EUnitOfWork.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ProductService.Controllers
{
    [EnableCors(origins: "http://localhost:50617", headers: "*", methods: "*")]
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var products = _unitOfWork.Products.GetAll();
            return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, products, System.Net.HttpStatusCode.OK);
        }

        [Route("{id}")]
        [HttpGet]
        [MyFilters.CheckForNullParameter]
        public IHttpActionResult Get(Guid id)
        {
            var product = _unitOfWork.Products.Get(id);
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
        public IHttpActionResult GetByName(string name)
        {
            var foundProduct = _unitOfWork.Products.GetProductByName(name);
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
        public IHttpActionResult GetByName([FromBody]Product product)
        {
            var foundProduct = _unitOfWork.Products.GetProductByName(product.ProductName);
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
        public IHttpActionResult GetByRange([FromBody]IEnumerable<Guid> ids)
        {
            // List<Guid> guids = Ids.Where(i => Ids.Contains(i)).Select(i => new Guid(i)).ToList();
            var foundProducts = _unitOfWork.Products.GetProductsByRange(ids);
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
        public IHttpActionResult Post([FromBody]Product product)
        {
            var productExists = _unitOfWork.Products.ProductExists(product);

            if (productExists)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<string>(Request, null, System.Net.HttpStatusCode.Conflict);
            }
            else
            {
                _unitOfWork.Products.Add(product);
                _unitOfWork.Complete();
                return new MyHelpers.ActionResultFactory.CreateActionResult<Product>(Request, product, System.Net.HttpStatusCode.OK);
            }
        }

        [Route("PostRange")]
        [HttpPost]
        [MyFilters.CheckForNullParameter]
        [MyFilters.GenericModelListStateValidator("products", typeof(Product))]
        public IHttpActionResult Post([FromBody]IEnumerable<Product> products)
        {
            var productsExist = _unitOfWork.Products.ProductRangeExists(products);

            if (productsExist)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<string>(Request, null, System.Net.HttpStatusCode.Conflict);
            }
            else
            {
                _unitOfWork.Products.AddRange(products);
                _unitOfWork.Complete();
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, products, System.Net.HttpStatusCode.OK);
            }
        }


        [Route("")]
        [HttpPut]
        [MyFilters.CheckForNullParameter]
        [MyFilters.ModelStateValidator]
        public IHttpActionResult Put([FromBody]Product product)
        {
            Product productToUpdate = null;
            var productExists = _unitOfWork.Products.ProductExists(product);

            if (productExists)
            {
                productToUpdate = _unitOfWork.Products.SingleOrDefault(p => p.ProductId == product.ProductId) as Product;
                productToUpdate.ProductName = product.ProductName;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;
                productToUpdate.CompanyId = product.CompanyId;
                productToUpdate.ProductCatagoryId = product.ProductCatagoryId;

                _unitOfWork.Products.Update(productToUpdate);
                _unitOfWork.Complete();
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
        public IHttpActionResult Put([FromBody]IEnumerable<Product> products)
        {
            var allProductsExist = _unitOfWork.Products.ProductRangeExists(products);

            if (allProductsExist)
            {
                var productsToUpdate = _unitOfWork.Products.GetProductsByRange(products);

                foreach (var productToUpdate in productsToUpdate)
                {
                    productToUpdate.ProductName = products.Single(p => p.ProductId == productToUpdate.ProductId).ProductName;
                    productToUpdate.Description = products.Single(p => p.ProductId == productToUpdate.ProductId).Description;
                    productToUpdate.Price = products.Single(p => p.ProductId == productToUpdate.ProductId).Price;
                    productToUpdate.CompanyId = products.Single(p => p.ProductId == productToUpdate.ProductId).CompanyId;
                    productToUpdate.ProductCatagoryId = products.Single(p => p.ProductId == productToUpdate.ProductId).ProductCatagoryId;
                    _unitOfWork.Products.Update(productToUpdate);
                }

                _unitOfWork.Complete();
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
        public IHttpActionResult Delete(Guid id)
        {
            var productToDelete = _unitOfWork.Products.SingleOrDefault(p => p.ProductId == id) as Product;
            if (productToDelete != null)
            {
                _unitOfWork.Products.Remove(productToDelete);
                _unitOfWork.Complete();
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
        public IHttpActionResult DeleteRange([FromBody]IEnumerable<Guid> ids)
        {

            var productsToDelete = _unitOfWork.Products.GetProductsByRange(ids);
            if (productsToDelete.Count() > 0)
            {
                _unitOfWork.Products.RemoveRange(productsToDelete);
                _unitOfWork.Complete();
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Product>>(Request, productsToDelete, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Guid>>(Request, ids, System.Net.HttpStatusCode.NotFound); ;
            }
        }
    }
}
