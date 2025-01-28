/*
 * Created by: Peter Weissbrod
 * Created: Tuesday, January 22, 2008
 */
using System;
using System.Collections.ObjectModel;
using PostSharp.UserSamples.Validators;

namespace BloggerApp.Models
{
    /// <summary>
    /// business object
    /// </summary>
    public class Blog:BusinessObject
    {
        #region members
        
        #endregion

        #region properties
        /// <summary>
        /// The author cannot be empty or null
        /// </summary>
        [NotNullValidator]
        [StringLengthValidator(MinLength = 1, MaxLength = 255)]
        public string Author
        {
            get;
            set;
        }
        /// <summary>
        /// a reference to the collection of BlogEntries
        /// </summary>
        public ObservableCollection<BlogEntry> BlogEntries
        {
            get;
            private set;
        }
        #endregion

        #region public methods
        /// <summary>
        /// general business object methods
        /// </summary>
        /// <param name="blogEntry"></param>
        public void AddBlogEntry(BlogEntry blogEntry)
        {
            foreach (BlogEntry _blogEntry in BlogEntries)
            {
                if(_blogEntry.Name==blogEntry.Name)
                    throw new Exception("The Name for this blog entry is already taken!");
            }
            BlogEntries.Add(blogEntry);
        }
        public void RemoveBlogEntry(BlogEntry blogEntry)
        {
            if(!BlogEntries.Contains(blogEntry))
                throw new Exception("You cannot remove that which is not there to begin with");
            BlogEntries.Remove(blogEntry);
        }
        /// <summary>
        /// mandatory constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="author"></param>
        public Blog(string name, int id, string author) : base(name, id)
        {
            Author = author;
            BlogEntries = new ObservableCollection<BlogEntry>();
        }
        #endregion

        #region private methods
        #endregion
    }
}