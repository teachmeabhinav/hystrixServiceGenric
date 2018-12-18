using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hystrix.Dotnet;
using Microsoft.AspNetCore.Mvc;

namespace hystrixServiceGenric.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        private readonly IHystrixCommand hystrixCommand;

        public ValuesController(IHystrixCommandFactory hystrixCommandFactory)
        {
            hystrixCommand = hystrixCommandFactory.GetHystrixCommand("TestGroup", "TestCommand");
        }

        [HttpGet("getstring")]
        public async Task<IActionResult> Get()
        {
            var result = await hystrixCommand.ExecuteAsync(
                async () =>
                {
                    // Here we could do a potentially failing operation, like calling an external service.
                    var rnd = new Random();
                    await Task.Delay(rnd.Next(500, 1000));

                    if (rnd.Next(4) == 0)
                    {
                        throw new Exception("Test exception. Hystrix will catch this and return the FallbackResult instead.");
                    }

                    return "ExpensiveResult";
                },
                () => Task.FromResult("FallbackResult"),
                new CancellationTokenSource());

            return Ok($"Hello! Result: {result}");
        }

        // GET api/values/5
        [HttpGet("getOther")]
        public List<string> other()
        {
            var result = hystrixCommand.Execute<List<string>>(
                 () =>
                {
                    // Here we could do a potentially failing operation, like calling an external service.
                    var rnd = new Random();
                    Task.Delay(rnd.Next(500, 1000));
                    List<string> tempResult = new List<string>();
                    if (rnd.Next(4) == 0)
                    {
                        throw new Exception("Test exception. Hystrix will catch this and return the FallbackResult instead.");                  

                    }
                    tempResult.Add("Pearson");
                    tempResult.Add("Datacard");
                    tempResult.Add("DC");
                    tempResult.Add("Hilti");
                    return tempResult;
                },
                () =>
                {
                    return new List<string>() { "Error Collection" };
                },
                new CancellationTokenSource());

            return result.ToList();
        }
    }
}
