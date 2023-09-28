using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedditClone.Data;
using RedditClone.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RedditClone.Controllers
{
    [ApiController]
    [Route("api/post/{postId}/comment")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly RedditCloneContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(RedditCloneContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, Comment comment)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found");
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            comment.UserId = user.Id;
            comment.PostId = postId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { postId, commentId = comment.Id }, comment);
        }


        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int postId, int commentId, Comment updatedComment)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            if (comment.PostId != postId)
            {
                return BadRequest("Comment does not belong to the specified post");
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.Id != comment.UserId)
            {
                return Forbid("You are not authorized to update this comment");
            }

            comment.Content = updatedComment.Content;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            if (comment.PostId != postId)
            {
                return BadRequest("Comment does not belong to the specified post");
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.Id != comment.UserId)
            {
                return Forbid("You are not authorized to delete this comment");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{commentId}")]
        public async Task<ActionResult<Comment>> GetComment(int postId, int commentId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            if (comment.PostId != postId)
            {
                return BadRequest("Comment does not belong to the specified post");
            }

            return comment;
        }
    }
}
