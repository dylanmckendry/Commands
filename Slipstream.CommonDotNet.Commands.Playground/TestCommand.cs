﻿using Microsoft.EntityFrameworkCore;
using Slipstream.CommonDotNet.Commands.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slipstream.CommonDotNet.Commands.Playground
{
    public class TestCommand : ISuccessResult<TestCommand, TestData>, INotFoundResult, IConflictResult
    {
    }

    public class TestCommandHandler : IAsyncCommandHandler<TestCommand>
    {
        private readonly BloggingContext context;

        public TestCommandHandler(BloggingContext context)
        {
            this.context = context;
        }

        public async Task<IResult> ExecuteAsync(TestCommand command)
        {
            await Task.Delay(1);

            if (new Random().Next(0, 20) > 15)
            {
                return new NotFoundException();
            }
            else
            {
                return new TestData();
            }
        }
    }

    public class TestData : IResult
    {
        public int Code { get; set; } = 200;
    }


    public class GetBlogCommand : ISuccessResult<GetBlogCommand, BlogData>, INotFoundResult
    {
        public int BlogId { get; set; }

        public GetBlogCommand(int blogId)
        {
            BlogId = blogId;
        }
    }

    public class BlogData : IResult
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }

    public class GetBlogCommandHandler : IAsyncCommandHandler<GetBlogCommand>
    {
        private readonly BloggingContext context;

        public GetBlogCommandHandler(BloggingContext context)
        {
            this.context = context;
        }

        public async Task<IResult> ExecuteAsync(GetBlogCommand command)
        {
            var blog = await context.Blogs.SingleOrDefaultAsync(b => b.BlogId == command.BlogId);

            if (blog == null)
            {
                return new NotFoundException();
            }

            return new BlogData
            {
                BlogId = blog.BlogId,
                Url = blog.Url
            };
        }
    }

    public class CreatePostCommand : ISuccessResult<CreatePostCommand, PostData>, INotFoundResult, IConflictResult
    {
        public int BlogId { get; set; }

        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public CreatePostCommand(int blogId, string postId, string title, string content)
        {
            BlogId = blogId;
            PostId = postId;
            Title = title;
            Content = content;
        }
    }

    public class PostData : IResult
    {
        public string PostId { get; set; }
    }


    public class CreatePostCommandHandler : IAsyncCommandHandler<CreatePostCommand>
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly BloggingContext context;

        public CreatePostCommandHandler(ICommandProcessor commandProcessor, BloggingContext context)
        {
            this.commandProcessor = commandProcessor;
            this.context = context;
        }

        public async Task<IResult> ExecuteAsync(CreatePostCommand command)
        {
            var getBlogResult = await commandProcessor.ProcessResultAsync(new GetBlogCommand(command.BlogId));
            if (getBlogResult.NotSuccess)
            {
                return getBlogResult.Result;
            }

            if (await context.Posts.AnyAsync(b => b.PostId == command.PostId))
            {
                return new ConflictException();
            }

            context.Posts.Add(new Post
            {
                BlogId = command.BlogId,
                PostId = command.PostId,
                Title = command.Title,
                Content = command.Content
            });

            await context.SaveChangesAsync();

            // return await Processor.For(new GetBlogCommand(command.BlogId))
            //      .When(o => o.NotSuccess()).Return(r => r)
            //      .When(o => o.Success()).Return(r => {
            //          
            //      })

            return new PostData
            {
                PostId = command.PostId
            };
        }
    }
}
