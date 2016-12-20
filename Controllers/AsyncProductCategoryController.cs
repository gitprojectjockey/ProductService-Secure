using EDataLayer.Core.Domain;
using EDataLayer.Core.EUnitOfWork.Async.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProductService.Controllers
{
    [Authorize]
    [RoutePrefix("async/api/productCategories")]
    public class AsyncProductCategoryController : ApiController
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        public AsyncProductCategoryController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(System.Threading.CancellationToken cancellationToken)
        {
            IEnumerable<ProductCategory> productCategories = await _unitOfWork.ProductCategories.GetAllAsync(cancellationToken);
            return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<ProductCategory>>(Request, productCategories, System.Net.HttpStatusCode.OK);
        }
    }
}