using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstagramNewsFeed.Models
{
    public class NewsFeed
    {
        public Guid Id { get; set; }

        public string Data { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }
}
