using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreServices.Models;
using CoreServices.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace CoreServices.Repository
{
    public class PostRepository : IPostRepository
    {
        BlogDBContext blogDBContext;
        public PostRepository(BlogDBContext blogDBContext)
        {
            this.blogDBContext = blogDBContext;
        }
        public async Task<int> AddPost(Post post)
        {
            if (blogDBContext != null)
            {
                await blogDBContext.Post.AddAsync(post);
                await blogDBContext.SaveChangesAsync();

                return post.PostId;
            }
            return 0;
        }

        public async Task<int> DeletePost(int? postId)
        {
            int result = 0;
            var post = await blogDBContext?.Post.FirstOrDefaultAsync(x => x.PostId == postId);
            if (post != null)
            {

                blogDBContext?.Post.Remove(post);
                result = await blogDBContext.SaveChangesAsync();
            }
            return result;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await blogDBContext?.Category.ToListAsync() ?? null;

        }

        public async Task<PostViewModel> GetPost(int? postId)
        {
            if (blogDBContext != null)
            {
                var post = from p in blogDBContext.Post
                           from c in blogDBContext.Category
                           where p.PostId == postId
                           select new PostViewModel
                           {
                               PostId = p.PostId,
                               Title = p.Title,
                               Description = p.Description,
                               CategoryId = p.CategoryId,
                               CategoryName = c.Name,
                               CreatedDate = p.CreatedDate
                           };
                return await post.FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<PostViewModel>> GetPosts()
        {
            if (blogDBContext != null)
            {
                var listOfPost = from p in blogDBContext.Post
                                 from c in blogDBContext.Category
                                 where p.CategoryId == c.Id
                                 select new PostViewModel
                                 {
                                     PostId = p.PostId,
                                     Title = p.Title,
                                     Description = p.Description,
                                     CategoryId = p.CategoryId,
                                     CategoryName = c.Name,
                                     CreatedDate = p.CreatedDate
                                 };
                return await listOfPost.ToListAsync();
            }
            else
            {
                return null;
            }

        }

        public async Task UpdatePost(Post post)
        {
            if (blogDBContext != null)
            {
                blogDBContext.Post.Update(post);

                //Commit the transaction
                await blogDBContext.SaveChangesAsync();
            }
        }
    }
}
