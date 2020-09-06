using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericMockApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericMockApi.Controllers
{
    [Route("/api/mockdata")]
    [ApiController]
    public class RandomMockValuesController : ControllerBase
    {
        private readonly IMockDataRepository _repository;

        public RandomMockValuesController(IMockDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{typeName}")]
        public ActionResult GetMockData(string typeName, int skip, int take)
        {
            var data = _repository.GetMockObjects(typeName, skip, take);
            if (data == null) return BadRequest();
            else return Ok(data);
        }
    }
}
