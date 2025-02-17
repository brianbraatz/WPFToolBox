//---------------------------------------------------------------------------
//
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.
//
// This file is part of the 3D Tools for Windows Presentation Foundation
// project.  For more information, see:
// 
// http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PhotoBrowser.FlickrApi;

namespace PhotoBrowser
{
    /// <summary>
    /// Interaction logic for BlogVisual.xaml
    /// </summary>

    public partial class BlogVisual : System.Windows.Controls.Border
    {
        public BlogVisual()
        {
            InitializeComponent();

            if (Flickr.CurrAuthorizedUser == null)
            {
                submitButton.IsEnabled = false;
            }
        }
    }
}