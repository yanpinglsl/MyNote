using Consul;
using Microsoft.AspNetCore.Mvc;

namespace YY.ConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController(IConsulClient consulClient) : ControllerBase
    {
        /// <summary>
        /// 创建锁
        /// </summary>
        /// <param name="key">共享资源的键值</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(string key)
        {
            var sessionEntry = new SessionEntry
            {
                //过期时间
                TTL = TimeSpan.FromMinutes(1),
                LockDelay = TimeSpan.FromSeconds(1),//默认15s
                //如果使用了release，任何与该session相关的锁都会被释放，并且持有该锁的key的ModifyIndex也会递增
                //如果使用了delete，持有该锁的KEY将会被删除。
                Behavior = SessionBehavior.Release
            };
            var sessionRequest = await consulClient.Session.Create(sessionEntry);
            var sessionId = sessionRequest.Response;

            var acquireLock = await consulClient.KV.Acquire(new KVPair(key)
            {
                Session = sessionId
            });

            return Ok(new {sessionId, acquireLock = acquireLock.Response});
        }


        /// <summary>
        /// 获取Session列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var sessionRequest = await consulClient.Session.List();
            return Ok(sessionRequest.Response);
        }


        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(string key, string sessionId)
        {
            var released = await consulClient.KV.Release(new KVPair(key)
            {
                Session = sessionId
            });
            return Ok(released .Response);
        }
    }
}
