using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YY.Docker.Http.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController(ApplicationDbContext dbContext)
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var person = await dbContext.Person.FirstAsync();
            return Ok(person);
        }

        [HttpPut]
        public async Task<ActionResult> Put(string name)
        {
            var person = await dbContext.Person.FirstAsync();
            person.Name = name;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}