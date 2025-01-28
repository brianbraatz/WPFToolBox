/*
 * Created by: Peter Weissbrod
 * Created: Tuesday, January 22, 2008
 */
using System;
using System.Collections.ObjectModel;
using BloggerApp.Models;

namespace BloggerApp.DAO
{
    /// <summary>
    /// dummy DAO implementation... no database
    /// Probably a good crosscut for IOC right here
    /// </summary>
    public class DummyBlogDAO:IDAO<Blog>
    {
        public ObservableCollection<Blog> GetAll()
        {
            throw new NotImplementedException();
        }
        public void Save(Blog model)
        {
            throw new NotImplementedException();
        }
        public Blog GetByID(int id)
        {
            return CreateDummyBlog();
        }
        private Blog CreateDummyBlog()
        {
            Blog _blog = new Blog("Pete's Blog",0,"Pete");
            BlogEntry _blogEntry = new BlogEntry("This entry came from the DB",0,_blog,"This is some dummy content");
            return _blog;
        }
    }
}