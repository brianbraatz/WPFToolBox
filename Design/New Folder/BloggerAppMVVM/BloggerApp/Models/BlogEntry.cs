/*
 * Created by: Peter Weissbrod
 * Created: Tuesday, January 22, 2008
 */
using System;
using System.ComponentModel;
using PostSharp.UserSamples.Validators;

namespace BloggerApp.Models
{
    /// <summary>
    /// business object
    /// </summary>
    public class BlogEntry:BusinessObject
    {
        #region members
        
        #endregion

        #region properties
        /// <summary>
        /// The content cant be empt or null
        /// </summary>
        [StringLengthValidator(MinLength = 1, MaxLength = 255)]
        [NotNullValidator]
        public string Content
        {
            get;
            set;
        }
        /// <summary>
        /// a reference to the parent Blog
        /// </summary>
        [NotNullValidator]
        public Blog Blog
        {
            get;
            private set;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Mandatory constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="blog"></param>
        /// <param name="content"></param>
        public BlogEntry(string name, int id, Blog blog, string content) : base(name, id)
        {
            Blog = blog;
            Content = content;
            Blog.AddBlogEntry(this);
        }
        #endregion

        #region private methods
        #endregion
    }
}