using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GR.Core.Tests.Extensions
{
    [Author("Lupei Nicolae")]
    [TestFixture]
    public class DbContextExtensionTexts
    {
        private const string ConString = "Host=109.185.158.154;Port=5432;Username=postgres;Password=1111;Persist Security Info=true;Database=ISODMS.PROD;MaxPoolSize=1000;";

        private DbContext _context;

        [SetUp]
        public void SetUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MockDbContext>();
            optionsBuilder.UseNpgsql(ConString);
            _context = new MockDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Is disposed 
        /// </summary>
        [Test]
        public void IsDbContextDisposedTest()
        {
            Assert.IsFalse(_context.IsDisposed());
            _context.Dispose();
            Assert.IsFalse(_context.IsDisposed());//Need to fix
        }

        /// <summary>
        /// Save
        /// </summary>
        [Test]
        public void SaveSynchronouslyTest()
        {
            var saveResult = _context.Save();
            Assert.IsTrue(saveResult.IsSuccess);

            var iContext = (IDbContext)_context;
            var iSaveResult = iContext.Push();
            Assert.IsTrue(iSaveResult.IsSuccess);
        }

        /// <summary>
        /// Save
        /// </summary>
        [Test]
        public async Task SaveAsyncTest()
        {
            var saveResult = await _context.SaveAsync();
            Assert.IsTrue(saveResult.IsSuccess);

            var iContext = (IDbContext)_context;
            var iSaveResult = await iContext.PushAsync();
            Assert.IsTrue(iSaveResult.IsSuccess);
        }
    }
}
