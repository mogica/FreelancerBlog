﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreelancerBlog.Areas.Admin.ViewModels.Article;
using FreelancerBlog.AutoMapper;
using FreelancerBlog.Core.Commands.ArticleComments;
using FreelancerBlog.Core.Commands.Articles;
using FreelancerBlog.Core.Domain;
using FreelancerBlog.Core.Queries.Article;
using FreelancerBlog.Core.Queries.Articles;
using FreelancerBlog.Core.Queries.ArticleTags;
using FreelancerBlog.Core.Services.Shared;
using FreelancerBlog.Core.Types;
using FreelancerBlog.ViewModels.Article;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FreelancerBlog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IMapper _mapper;
        private IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private ICaptchaValidator _captchaValidator;
        private IConfiguration _configuration;

        public ArticleController(UserManager<ApplicationUser> userManager, ICaptchaValidator captchaValidator, IConfiguration configuration, IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
            _userManager = userManager;
            _captchaValidator = captchaValidator;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = await _mediator.Send(new GetAriclesQuery());

            var articlesViewModel = _mapper.Map<List<Article>, List<ArticleViewModel>>(articles.ToList());

            return View(articlesViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Tag(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var articles = await _mediator.Send(new ArticlesByTagQuery { TagId = id });

            var articlesViewModel = _mapper.Map<List<Article>, List<ArticleViewModel>>(articles.ToList());

            return View(articlesViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var article = await _mediator.Send(new ArticleByArticleIdQuery { ArticleId = 4 });

            if (article == null)
            {
                return NotFound();
            }

            await _mediator.Send(new IncreaseArticleViewCountCommand { ArticleId = id });

            var articleViewModel = _mapper.Map<Article, ArticleViewModel>(article);
            articleViewModel.ArticleTags = await _mediator.Send(new TagsByArticleIdQuery { ArticleId = article.ArticleId }); ;
            articleViewModel.ArticleTagsList = await _mediator.Send(new GetCurrentArticleTagsQuery { ArticleId = article.ArticleId });
            articleViewModel.SumOfRating = articleViewModel.ArticleRatings.Sum(a => a.ArticleRatingScore) / articleViewModel.ArticleRatings.Count;
            articleViewModel.CurrentUserRating = articleViewModel.ArticleRatings.SingleOrDefault(a => a.UserIDfk == _userManager.GetUserId(User));

            return View(articleViewModel);
        }

        public async Task<JsonResult> RateArticle(int id, double rating)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Status = "YouMustLogin" });
            }

            var userId = _userManager.GetUserId(User);
            var rateBefore = await _mediator.Send(new ArticleRatedBeforeQuery { ArticleId = id, UserId = userId });

            if (rateBefore)
            {
                await _mediator.Send(new UpdateArticleRatingCommand { ArticleId = id, ArticleRating = rating, UserId = userId });

                return Json(new { Status = "UpdatedSuccessfully" });
            }

            await _mediator.Send(new AddRatingToArticleCommand { ArticleId = id, ArticleRating = rating, UserId = userId });

            return Json(new { Status = "Success" });
        }

        [HttpPost]
        public async Task<JsonResult> SubmitComment(ArticleCommentViewModel viewModel)
        {
            CaptchaResponse captchaResult = await _captchaValidator.ValidateCaptchaAsync(_configuration.GetValue<string>("reChaptchaSecret:server-secret"));

            if (captchaResult.Success != "true")
            {
                return Json(new { status = "FailedTheCaptchaValidation" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { Status = "CannotHaveEmptyArgument" });
            }

            var articleComment = _mapper.Map<ArticleCommentViewModel, ArticleComment>(viewModel);

            await _mediator.Send(new AddCommentToArticleCommand { ArticleComment = articleComment });

            return Json(new { Status = "Success" });
        }
    }
}