using System;
using System.Windows;
using System.Windows.Controls;
using BloggerApp.DAO;
using BloggerApp.Models;
using BloggerApp.ViewModels;

namespace BloggerApp
{
    /// <summary>
    /// Interaction logic for MainUI.xaml
    /// </summary>
    public partial class MainUI : Window
    {
        private BlogEntryEditor m_BlogEntryEditor;
        public Blog Blog
        {
            get { return DataContext as Blog; }
        }
        public MainUI()
        {
            DataContext = new DummyBlogDAO().GetByID(0);
            InitializeComponent();
        }
        private void btnNewPost_Click(object sender, RoutedEventArgs e)
        {
            m_BlogEntryEditor = new BlogEntryEditor();
            m_BlogEntryEditor.Blog = Blog;
            m_BlogEntryEditor.BlogEntryViewModel = new BlogEntryViewModel(Blog);
            m_BlogEntryEditor.ShowDialog();
        }
        private void btnEditPost_Click(object sender, RoutedEventArgs e)
        {
            if (lstBlogEntries.SelectedValue != null)
            {
                m_BlogEntryEditor = new BlogEntryEditor();
                m_BlogEntryEditor.Blog = DataContext as Blog;
                m_BlogEntryEditor.BlogEntryViewModel = new BlogEntryViewModel((BlogEntry) lstBlogEntries.SelectedValue);
                m_BlogEntryEditor.ShowDialog();
            }
            else
            {
                MessageBox.Show("select something to edit you silly goose");
            }
        }
        private void btnRemovePost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Blog.RemoveBlogEntry((BlogEntry) lstBlogEntries.SelectedValue);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}