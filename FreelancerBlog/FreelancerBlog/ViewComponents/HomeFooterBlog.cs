﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreelancerBlog.Areas.Admin.ViewModels.Article;
using FreelancerBlog.AutoMapper;
using FreelancerBlog.Core.Domain;
using FreelancerBlog.Core.Queries.Articles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerBlog.ViewComponents
{
    public class HomeFooterBlog : ViewComponent
    {
        private readonly IMapper _mapper;
        private IMediator _mediator;

        public HomeFooterBlog(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var articles = await _mediator.Send(new GetLatestArticlesQuery {NumberOfArticles = 3});

            var articlesViewModel = _mapper.Map<List<Article>, List<ArticleViewModel>>(articles.ToList());

            return View(articlesViewModel);
        }
    }
}