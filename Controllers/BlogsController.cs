using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using DotNetCoreCodeFirst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreCodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BloggingContext _context;
        
        public BlogsController(BloggingContext context)
        {
            _context = context;
            
            if (!_context.Blogs.Any())
            {
                _context.Blogs.Add(new Blog{Url = "abc.xyz", Name = "Abc"});
                _context.SaveChanges();
            }
        }
        
        // GET api/blogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            return await _context.Blogs.ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blogItem = await _context.Blogs.FindAsync(id);

            if (blogItem == null)
            {
                return NotFound();
            }

            return blogItem;
        }

        //Return list of blog by name
        [HttpGet("name/{name}")]
        public ActionResult<Blog> GetBlogByName(string name)
        {
            var blog = _context.Blogs.Where(x => x.Name.Equals(name)).ToList();
            
            if (blog.Count == 0)
            {
                return NotFound();
            }

            return Ok(blog);
        }
        
        // POST api/values
        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof (GetBlog), new {id = blog.BlogId}, blog) ;
        }

        // PUT api/blogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlog([FromRoute] int id, [FromBody] Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (id != blog.BlogId)
            {
                return BadRequest();
            }
            
            _context.Entry(blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!ExistBlog(id))
                {
                    return NotFound();
                }
                throw;
            }
            

            return Ok("Success");
        }

        private bool ExistBlog(int id)
        {
            return _context.Blogs.Any(x => x.BlogId == id);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBlog(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blog = await _context.Blogs.FindAsync(id);

            if (!ExistBlog(id))
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }
    }
}