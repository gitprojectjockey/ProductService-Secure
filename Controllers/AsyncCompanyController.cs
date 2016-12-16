using EDataLayer.Core.Domain;
using EDataLayer.Core.EUnitOfWork.Async.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ProductService.Controllers
{
    [Authorize]
    [EnableCors(origins: "http://localhost:50617", headers: "*", methods: "*")]
    [RoutePrefix("async/api/companies")]
    public class AsyncCompaniesController : ApiController
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        public AsyncCompaniesController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(System.Threading.CancellationToken cancellationToken)
        {
            IEnumerable<Company> companies = await _unitOfWork.Companies.GetAllAsync(cancellationToken);
            return new MyHelpers.ActionResultFactory.CreateActionResult<IEnumerable<Company>>(Request, companies, System.Net.HttpStatusCode.OK);
        }

        [Route("{id}")]
        [HttpGet]
        [MyFilters.CheckForNullParameter]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            Company company = await _unitOfWork.Companies.GetAsync(id);
            if (company != null)
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Company>(Request, company, System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new MyHelpers.ActionResultFactory.CreateActionResult<Guid>(Request, company.CompanyId, System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}