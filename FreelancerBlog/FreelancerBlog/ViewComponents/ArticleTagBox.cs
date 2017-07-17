﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreelancerBlog.Areas.Admin.ViewModels.Article;
using FreelancerBlog.Core.Domain;
using FreelancerBlog.Core.Queries.ArticleTags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerBlog.ViewComponents
{
    public class ArticleTagBox : ViewComponent
    {
        private readonly IMapper _mapper;
        private IMediator _context;

        public ArticleTagBox(IMapper mapper, IMediator context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var articleTags = await _context.Send(new GetAllArticleTagsQuery());

            var articleTagViewModel = _mapper.Map<List<ArticleTag>, List<ArticleTagViewModel>>(articleTags.ToList());

            return View(articleTagViewModel);
        }
    }
}