using System;
using System.Windows;
using BloggerApp.Models;
using BloggerApp.ViewModels;

namespace BloggerApp
{
    /// <summary>
    /// Interaction logic for BlogEntryEditor.xaml
    /// </summary>
    public partial class BlogEntryEditor : Window
    {
        #region properties
        /// <summary>
        /// If we want controls directly bound to a property in code behind, we need (at the very least) to register this property as a DependencyProperty
        /// </summary>
        public static readonly DependencyProperty BlogEntryViewModelProperty =
            DependencyProperty.Register("BlogEntryViewModel", typeof (BlogEntryViewModel), typeof (BlogEntryEditor),
                                        new PropertyMetadata(null));
        public BlogEntryViewModel BlogEntryViewModel
        {
            get { return ((BlogEntryViewModel) GetValue(BlogEntryViewModelProperty)); }
            set { SetValue(BlogEntryViewModelProperty, value); }
        }
        public Blog Blog
        {
            get;
            set;
        }
        #endregion

        public BlogEntryEditor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// when the user clicks apply, build event args and send them to the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BlogEntryViewModel.ApplyChanges();
                Close();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        /// <summary>
        /// when the user clicks cancel, close the UI, do nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}