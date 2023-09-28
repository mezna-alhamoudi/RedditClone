namespace RedditClone.Models
{
    public class Post
    {
        public Post()
        {
            Title = string.Empty;
            Content = string.Empty;
            UserId = string.Empty;
            Comments = new List<Comment>();
            // If ApplicationUser can be null, initialize it here or handle it appropriately.
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; } 
        public List<Comment> Comments { get; set; }
    }
}
