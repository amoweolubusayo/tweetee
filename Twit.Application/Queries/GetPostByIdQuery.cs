using System.Net.Cache;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Twit.Infrastructure.Data;
using Twit.Application.Models;
using Twit.Core.DTOs.APIResponse;
using Twit.Core.Utils;

namespace Twit.Application.Queries
{
    public class GetPostByIdQuery: IRequest<GenericResponse<PostResponse>>  
    {
         public int Id { get; set; }
    }

     public class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
    {
        public GetPostByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();

        }
    }
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, GenericResponse<PostResponse>>
    {
        private readonly TwitContext _context;
        private readonly ILogger<GetPostByIdQueryHandler> _logger;

        public GetPostByIdQueryHandler(TwitContext context, ILogger<GetPostByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<GenericResponse<PostResponse>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
               PostResponse resp = await (from post in _context.Posts
                                         join user in _context.Users on post.UserId equals user.Id
                                         where post.Id == request.Id
                                         select new PostResponse
                                         {
                                             PostId = post.Id,
                                             Content = post.Content,
                                             IsDeleted = post.IsDeleted,
                                             IsLiked = post.Liked,
                                             PostedBy = user.UserName
                                         }).FirstOrDefaultAsync();
            
            return new GenericResponse<PostResponse>(true, "profile information fetched",resp);
        }

    }
}