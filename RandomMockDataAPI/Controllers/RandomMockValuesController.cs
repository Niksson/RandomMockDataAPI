using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RandomMockDataAPI.Options;
using RandomMockDataAPI.Repositories;

namespace RandomMockDataAPI.Controllers
{
    [Route("/api/mockdata")]
    [ApiController]
    public class RandomMockValuesController : ControllerBase
    {
        private readonly IMockDataRepository _repository;
        private readonly IOptionsSnapshot<MockGeneratorOptions> _options;

        public RandomMockValuesController(IMockDataRepository repository, IOptionsSnapshot<MockGeneratorOptions> options)
        {
            _repository = repository;
            _options = options;
        }

        [HttpGet("{typeName}")]
        public ActionResult GetMockData(string typeName, int skip, int take)
        {
            var objectsPerRequest = _options.Value.ObjectsPerRequest;
            if(take > objectsPerRequest) return BadRequest(new { error = $"The amount of objects to take exceeds limit in appsettings.json: {objectsPerRequest}"});
            var data = _repository.GetMockObjects(typeName, skip, take);
            if (data == null) return BadRequest();
            else return Ok(data);
        }
    }
}
