using Consul;
using Microsoft.AspNetCore.Mvc;

namespace YY.ConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockController(IConsulClient consulClient, IHostEnvironment hostEnvironment) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string key)
        {
            var lockKey = $"{hostEnvironment.ApplicationName}/lock/{key}";

            var myLock = consulClient.CreateLock(new LockOptions(lockKey)
            {
                SessionTTL = TimeSpan.FromSeconds(60),
                LockTryOnce = false,
                LockWaitTime = TimeSpan.FromSeconds(2),
                LockRetryTime = TimeSpan.FromSeconds(1),
                MonitorRetryTime = TimeSpan.FromSeconds(2)
            });

            await myLock.Acquire();

            while (myLock.IsHeld)
            {
                try
                {
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    await myLock.Release();
                }
            }

            return Ok($"{DateTime.Now}");
        }
    }
}
