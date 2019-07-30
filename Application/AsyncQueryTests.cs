using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Application
{
    [TestFixture]
    public class AsyncQueryTests
    {
        [Test]
        public async Task GetAllBlogsAsync_orders_by_name()
        {

            var data = new List<Blog>
            {
                new Blog { Name = "BBB" },
                new Blog { Name = "ZZZ" },
                new Blog { Name = "AAA" },
            }.AsQueryable();

            var mockSet = A.Fake<DbSet<Blog>>(d => d.Implements<IQueryable<Blog>>().Implements<IAsyncEnumerable<Blog>>());
            A.CallTo(() => ((IQueryable<Blog>)mockSet).Provider).Returns(new TestAsyncQueryProvider<Blog>(data.Provider));
            A.CallTo(() => ((IQueryable<Blog>)mockSet).Expression).Returns(data.Expression);
            A.CallTo(() => ((IQueryable<Blog>)mockSet).ElementType).Returns(data.ElementType);
            A.CallTo(() => ((IAsyncEnumerable<Blog>)mockSet).GetEnumerator()).Returns(new TestAsyncEnumerator<Blog>(data.GetEnumerator()));
            
            var mockContext = A.Fake<BloggingContext>();
            A.CallTo(() => mockContext.Blogs).Returns(mockSet);
            
            var service = new BlogService(mockContext);
            var blogs = await service.GetAllBlogsAsync();

            Assert.AreEqual(3, blogs.Count);
            Assert.AreEqual("AAA", blogs[0].Name);
            Assert.AreEqual("BBB", blogs[1].Name);
            Assert.AreEqual("ZZZ", blogs[2].Name);
        }
    }
}
