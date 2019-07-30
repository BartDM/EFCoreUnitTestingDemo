using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Application
{
    [TestFixture]
    public class QueryTests
    {
        [Test]
        public void GetAllBlogs_orders_by_name()
        {
            var data = new List<Blog>
            {
                new Blog { Name = "BBB" },
                new Blog { Name = "ZZZ" },
                new Blog { Name = "AAA" },
            }.AsQueryable();

            var mockSet = A.Fake<DbSet<Blog>>(d=>d.Implements<IQueryable<Blog>>());
            A.CallTo(() => ((IQueryable<Blog>) mockSet).Provider).Returns(data.Provider);
            A.CallTo(() => ((IQueryable<Blog>) mockSet).Expression).Returns(data.Expression);
            A.CallTo(() => ((IQueryable<Blog>) mockSet).ElementType).Returns(data.ElementType);
            A.CallTo(() => ((IQueryable<Blog>) mockSet).GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = A.Fake<BloggingContext>();
            A.CallTo(() => mockContext.Blogs).Returns(mockSet);

            var service = new BlogService(mockContext);
            var blogs = service.GetAllBlogs();

            Assert.AreEqual(3, blogs.Count);
            Assert.AreEqual("AAA", blogs[0].Name);
            Assert.AreEqual("BBB", blogs[1].Name);
            Assert.AreEqual("ZZZ", blogs[2].Name);
        }
    }
}
