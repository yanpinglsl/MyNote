using Microsoft.AspNetCore.Mvc;

namespace SwaggerDemo.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("v2/api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V2))]
    public class OrdersV2Controller : ControllerBase
    {
        /// <summary>
        /// 获取数据列表。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        /// <summary>
        /// 获取主键所对应的数据。
        /// </summary>
        /// <param name="id">查询的主键。</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        /// <summary>
        /// 增加数据。
        /// </summary>
        /// <param name="value">参数</param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        /// <summary>
        /// 修改数据。
        /// </summary>
        /// <param name="id">查询主键。</param>
        /// <param name="value">要修改的值。</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        /// <summary>
        /// 删除数据。
        /// </summary>
        /// <param name="id">要删除的主键。</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
