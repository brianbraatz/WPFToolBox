/*
 * Created by: Peter Weissbrod
 * Created: 01/25/2008
 */
using BloggerApp.DAO;
using BloggerApp.Models;
using BloggerApp.ViewModels;
using NUnit.Framework;

namespace BloggerApp.Tests.ViewModelTests
{
    [TestFixture]
    public class BlogEntryViewModelTests
    {
        #region setup/teardown
        [TestFixtureSetUp]
        public void Setup()
        {
        }
        [TestFixtureTearDown]
        public void TearDown()
        {
        }
        #endregion

        #region tests
        [Test]
        [ExpectedException]
        public void TestInvalidCreate()
        {
            Blog _blog = new DummyBlogDAO().GetByID(0);
            BlogEntryViewModel _blogEntryViewModel = new BlogEntryViewModel(_blog);
            _blogEntryViewModel.BlogEntryName = string.Empty;
            _blogEntryViewModel.ApplyChanges();
        }
        [Test]
        public void TestValidCreate()
        {
            Blog _blog = new DummyBlogDAO().GetByID(0);
            BlogEntryViewModel _blogEntryViewModel = new BlogEntryViewModel(_blog);
            _blogEntryViewModel.BlogEntryName = "Foo";
            _blogEntryViewModel.BlogEntryContent = "bar";
            _blogEntryViewModel.ApplyChanges();
        }
        [Test]
        [ExpectedException]
        public void TestInavlidEdit()
        {
            Blog _blog = new DummyBlogDAO().GetByID(0);
            BlogEntryViewModel _blogEntryViewModel = new BlogEntryViewModel(_blog.BlogEntries[0]);
            _blogEntryViewModel.BlogEntryName = string.Empty;
            _blogEntryViewModel.ApplyChanges();
        }
        [Test]
        public void TestValidEdit()
        {
            Blog _blog = new DummyBlogDAO().GetByID(0);
            BlogEntryViewModel _blogEntryViewModel = new BlogEntryViewModel(_blog.BlogEntries[0]);
            _blogEntryViewModel.BlogEntryName = "Foo";
            _blogEntryViewModel.BlogEntryContent = "bar";
            _blogEntryViewModel.ApplyChanges();
        }
        #endregion
    }
}