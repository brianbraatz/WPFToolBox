/*
 * Created by: Peter Weissbrod
 * Created: Friday, January 25, 2008
 */
using System;
using System.Reflection;
using BloggerApp.DAO;
using BloggerApp.Models;
using PostSharp.UserSamples.Validators;

namespace BloggerApp.ViewModels
{
    /// <summary>
    /// A semblance of a BlogEntry business object, created specifically for GUI purposes
    /// </summary>
    public class BlogEntryViewModel : ViewModelBase<BlogEntry>
    {
        #region members
        /// <summary>
        /// This variable is only used for "New" or "Creation" BlogEntries
        /// </summary>
        private readonly Blog m_Blog;
        private string m_Error;
        #endregion

        #region properties
        /// <summary>
        /// expose a bindable, editable name
        /// </summary>
        public string BlogEntryName
        {
            get;
            set;
        }
        /// <summary>
        /// expose a bindable editable content
        /// </summary>
        public string BlogEntryContent
        {
            get;
            set;
        }
        /// <summary>
        /// Any changes to the model should be reflected in the ViewModel
        /// </summary>
        protected override void UpdateViewModelFromBusinessObject()
        {
            if (Model != null)
            {
                BlogEntryName = Model.Name;
                BlogEntryContent = Model.Content;
            }
        }
        ///<summary>
        ///Gets the error message for the property with the given name.
        ///</summary>
        ///
        ///<returns>
        ///The error message for the property. The default is an empty string ("").
        ///</returns>
        ///
        ///<param name="columnName">The name of the property whose error message to get. </param>
        public override string this[string columnName]
        {
            ///this code still bugs me, why should I duplicate my validation efforts here?
            get
            {
                if (columnName == "Name")
                {
                    if (BlogEntryName.Length < 0)
                        return "Name must be specified";
                    if (BlogEntryName.Length > 254)
                        return "Wow that is a long name! shorten it up there!";
                }
                if (columnName == "Content")
                {
                    if (BlogEntryContent.Length < 0)
                        return "A blog with no content? I might as well read myspace!";
                    if (BlogEntryContent.Length > 254)
                        return "This system runs on amiga and only supports very short posts.";
                }
                return string.Empty;
            }
        }
        ///<summary>
        ///Gets an error message indicating what is wrong with this object.
        ///</summary>
        ///
        ///<returns>
        ///An error message indicating what is wrong with this object. The default is an empty string ("").
        ///</returns>
        ///
        public override string Error
        {
            get { return m_Error; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Build a BlogEntryViewModel from a good sound Business object
        /// (this is used when you are editing an existing Business Object)
        /// </summary>
        /// <param name="model"></param>
        public BlogEntryViewModel(BlogEntry model) : base(model)
        {
            UpdateViewModelFromBusinessObject();
        }
        /// <summary>
        /// Build a BlogEntryViewModel from nothing.
        /// In this scenario, we will need to poke in some default values, since we dont have a business object
        /// We will, need a parent object (Blog) to perform the creation
        /// </summary>
        /// <param name="blog"></param>
        public BlogEntryViewModel(Blog blog)
        {
            m_Blog = blog;
            BlogEntryName = "Untitled";
            BlogEntryContent = string.Empty;
        }
        #endregion

        #region private methods
        /// <summary>
        /// This is the callback for editing an existing BlogEntry Business object
        /// </summary>
        protected override void ApplyToModel()
        {
            Model.Name = BlogEntryName;
            Model.Content = BlogEntryContent;
        }
        /// <summary>
        /// This is the callback for creaing a new Business object
        /// (A BlogEntry business object cannot be created without a valid existing Blog business object.)
        /// </summary>
        protected override void CreateModel()
        {
            BlogEntry _blogEntry = new BlogEntry(BlogEntryName, 0, m_Blog, BlogEntryContent);
            //in a real database app, we would save or persist the BlogEntry right around here.
        }
        #endregion
    }
}