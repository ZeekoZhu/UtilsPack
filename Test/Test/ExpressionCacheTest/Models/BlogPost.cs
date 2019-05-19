using System.Collections.Generic;

namespace Test.ExpressionCacheTest.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<Category> Categories { get; set; }
        public bool IsActive { get; set; }
    }
}