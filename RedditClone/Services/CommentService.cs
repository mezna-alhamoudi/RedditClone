using RedditClone.Models;
using RedditClone.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditClone.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;
        
        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        
        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            return await _commentRepository.GetCommentsAsync();
        }
        
        // Implement other service methods as needed
    }
}
