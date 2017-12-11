using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InstagramNewsFeed.Models;

namespace InstagramNewsFeed.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NewsController : Controller
    {
        private readonly ApiDbContext _context;

        public NewsController (ApiDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get news feeds from Instagram.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /news
        ///  
        /// </remarks>
        /// <returns>List news</returns>
        /// <response code="200">Returns the newly-created news</response>
        /// <response code="400">If the item is null</response>    
        [HttpGet]
        public OkResult Get()
        {
            return Ok();
        }



    }
}
