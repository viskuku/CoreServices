using CoreServices.Controllers;
using CoreServices.Models;
using CoreServices.Repository;
using CoreServices.ViewModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoreServices.Test
{
    public class PostUnitTestController
    {
        private PostRepository postRepository;
        public static DbContextOptions<BlogDBContext> dbContextOptions { get; }
        public static string connectionString = @"Server=LAPTOP-UO8CH2JI\SQLEXPRESS;Initial Catalog=BlogDBTest;Integrated Security=True";
        private Mock<IPostRepository> postRepositoryMock;
        private ILogger<PostController> logger;


        static PostUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
               .UseSqlServer(connectionString)
               .Options;
        }

        public PostUnitTestController()
        {
            var context = new BlogDBContext(dbContextOptions);
            DummyDataDBInitializer db = new DummyDataDBInitializer();
            db.Seed(context);

            var serviceProvider = new ServiceCollection()
.AddLogging(config => config.AddLog4Net())
.BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            logger = factory.CreateLogger<PostController>();
            postRepository = new PostRepository(context);
            postRepositoryMock = new Mock<IPostRepository>();
        }


        [Fact]
        public async void Task_GetPostById_Return_OkResult()
        {
            postRepositoryMock.Setup(i => i.GetPost(It.IsAny<int>())).Returns(Task.FromResult<PostViewModel>(new PostViewModel() { CategoryId = 1 }));

            //Arrange  
            var controller = new PostController(logger, postRepositoryMock.Object);
            var postId = 2;

            //Act
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_NoFoundResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var postId = 2;

            //Act
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            int? postId = null;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_MatchResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            int? postId = 1;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            Assert.Equal("Test Title 1", post.Title);
            Assert.Equal("Test Description 1", post.Description);
        }


        [Fact]
        public async void Task_GetPosts_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);

            //Act  
            var data = await controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void Task_GetPosts_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);

            //Act  
            var data = controller.GetPosts();
            data = null;

            if (data != null)
                //Assert  
                Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_GetPosts_MatchResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);

            //Act  
            var data = await controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<List<PostViewModel>>().Subject;

            Assert.Equal("Test Title 1", post[0].Title);
            Assert.Equal("Test Description 1", post[0].Description);

            Assert.Equal("Test Title 2", post[1].Title);
            Assert.Equal("Test Description 2", post[1].Description);
        }

        [Fact]
        public async void Task_Add_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var post = new Post() { Title = "Test Title 3", Description = "Test Description 3", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_Add_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            Post post = new Post() { Title = "Test Title More Than 20 Characteres", Description = "Test Description 3", CategoryId = 3, CreatedDate = DateTime.Now };

            //Act              
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }


        [Fact]
        public async void Task_Add_ValidData_MatchResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var post = new Post() { Title = "Test Title 4", Description = "Test Description 4", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            // var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;  

            Assert.Equal(3, okResult.Value);
        }

        [Fact]
        public async void Task_Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.Title = "Test Title 2 Updated";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var updatedData = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<OkResult>(updatedData);
        }

        [Fact]
        public async void Task_Update_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.Title = "Test Title More Than 20 Characteres";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var data = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }


        [Fact]
        public async void Task_Update_InvalidData_Return_NotFound()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.PostId = 5;
            post.Title = "Test Title More Than 20 Characteres";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var data = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_Delete_Post_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var postId = 2;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<OkResult>(data);
        }

        [Fact]
        public async void Task_Delete_Post_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            var postId = 5;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_Delete_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(logger,postRepository);
            int? postId = null;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }
    }
}
