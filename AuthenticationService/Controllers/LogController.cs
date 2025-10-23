// using AuthenticationService.Database.EntityFramework.Context;
// using AuthenticationService.Domain.Log;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Package.Database.EntityFramework.Interfaces;

// namespace AuthenticationService.Controllers
// {
//     [ApiController]
//     [Route("v1/log")]
//     public class LogController : ControllerBase
//     {
//         private readonly ILogger<LogController> _logger;
//         private readonly IRepository<LogDto, AuthContext> _logDb;
//         public LogController(IRepository<LogDto, AuthContext> logDb)
//         {
//             _logDb = logDb;
//         }

//         [AllowAnonymous]
//         [HttpPost()]
//         public async Task<IActionResult> CreateLog([FromBody] LogDto log)
//         {
//             if (!ModelState.IsValid) return null;
//             try
//             {
//                 _logDb.Insert(log);
//                 _logDb.Save();
//                 return Ok(log);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogInformation("Error creating user: " + ex.Message);

//             }

//             return NotFound(null);
//         }

//         [HttpGet()]
//         public async Task<IActionResult> GetLog()
//         {
//             return Ok(_logDb.Get());
//         }
//     }
// }
