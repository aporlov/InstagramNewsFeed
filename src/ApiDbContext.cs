﻿using Microsoft.EntityFrameworkCore;
using InstagramNewsFeed.Models;

namespace InstagramNewsFeed
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<NewsFeed> NewsFeed { get; set; }

    }
}
