using System;
using Xunit;
using Zeeko.UtilsPack;

namespace Test
{
    public class MapperTest
    {
        class Item
        {
            public int A { get; set; }
            public string B;
        }

        struct ItemDto
        {
            public int A { get; set; }
            public string B;
        }
        [Fact]
        public void Test1()
        {
            var src = new Item
            {
                A = 123,
                B = "hello"
            };
            var target = src.Map().To<ItemDto>();
            Assert.Equal(src.A, target.A);
            Assert.Equal(src.B, target.B);
        }
    }
}
