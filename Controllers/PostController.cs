using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedditClone.Data;
using RedditClone.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedditClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly RedditCloneContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(RedditCloneContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost(Post post)
        {
            if (ModelState.IsValid)
            {
                // Get the currently authenticated user
                var username = User.FindFirst(ClaimTypes.Name)?.Value; 
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("User not authenticated");
                }

                // Find the user by username
                var user = await _userManager.FindByNameAsync(username);
                
                if (user == null)
                {
                    return NotFound("User not found");
                }
                
                var userPrimKey = user.Id;
                
                // Set the UserId for the post
                post.UserId = userPrimKey;

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Post created successfully", PostId = post.Id });

            }

            return BadRequest("Invalid data");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, Post updatedPost)
        {
            var existingPost = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (existingPost == null)
            {
                return NotFound("Post not found");
            }

            if (ModelState.IsValid)
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value; 
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("User not authenticated");
                }

                // Find the user by username
                var user = await _userManager.FindByNameAsync(username);
                
                if (user == null)
                {
                    return NotFound("User not found");
                }
                
                if (user.Id != existingPost.UserId.ToString())
                {
                    return Forbid("You are not authorized to update this post");
                }

                existingPost.Title = updatedPost.Title;
                existingPost.Content = updatedPost.Content;

                _context.Posts.Update(existingPost);
                await _context.SaveChangesAsync();
                return Ok("Post updated successfully");
            }

            return BadRequest("Invalid data");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            return Ok(post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            // Remove the post from the DbContext
            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            return Ok("Post deleted successfully");
        }


    }
}
