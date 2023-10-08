using Microsoft.EntityFrameworkCore;
using RedditClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedditClone.Data;

namespace RedditClone.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RedditCloneContext _context;
        
        public CommentRepository(RedditCloneContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
