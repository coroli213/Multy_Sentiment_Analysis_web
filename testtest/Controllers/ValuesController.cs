using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testtest.Models;

namespace testtest.Models
{
    public class SentyPost
    {
        public long   Id        { get; set; }
        public string Text      { get; set; }
        public string Sentiment { get; set; }
    }
    public class ValuesContext : DbContext
    {
        public ValuesContext(DbContextOptions<ValuesContext> options) : base(options) { }

        public DbSet<SentyPost> SentyItems { get; set; }
    }
}

namespace testtest.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ValuesContext context_;

        public ValuesController(ValuesContext context)
        {
            context_ = context;
            if (!context_.SentyItems.Any())
            {
                context_.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<SentyPost> Get()
        {
           
            return context_.SentyItems.ToList();
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
         
            SentyPost user = context_.SentyItems.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound();
            return new ObjectResult(user);
        }

        // POST api/users
        [HttpPost]
        public IActionResult Post([FromBody]SentyPost user)
        {
            if (user == null){ return BadRequest(); }

            user.Sentiment = Program.UseModelWithSingleItem(user.Text);
            context_.SentyItems.Add(user);
            context_.SaveChanges();
            return Ok(user);
        }

        // PUT api/users/
        [HttpPut]
        public IActionResult Put([FromBody]SentyPost user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!context_.SentyItems.Any(x => x.Id == user.Id))
            {
                return NotFound();
            }

            context_.Update(user);
            context_.SaveChanges();
            return Ok(user);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            SentyPost user = context_.SentyItems.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            context_.SentyItems.Remove(user);
            context_.SaveChanges();
            return Ok(user);
        }
    }
}

