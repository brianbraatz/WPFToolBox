//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DragHelper class.
//  Creator: t-tomkm
//  Date Created: 08/30/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Contains logic for dragging items.
    /// </summary>
    internal class DragHelper : IDisposable
    {
        private Control _sourceControl;
        private bool _readyToDrag;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// The helper will attach its own handlers to all events whitch it uses. The control
        /// doesn't have to invoke any method from the helper.
        /// </remarks>
        /// <param name="sourceControl">Control that uses this helper.</param>
        public DragHelper(Control sourceControl)
        {
            Assert.ArgumentNotNull(sourceControl, "sourceControl", "Source control cannot be null.");
            _sourceControl = sourceControl;
            _sourceControl.MouseMove += OnMouseMove;
            _sourceControl.PreviewMouseLeftButtonDown += OnLeftButtonDown;
            _sourceControl.PreviewMouseLeftButtonUp += OnLeftButtonUp;
            _sourceControl.MouseLeave += OnMouseLeave;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_readyToDrag)
            {
                return;
            }

            FrameworkElement sourceElement = e.OriginalSource as FrameworkElement;
            if (sourceElement == null)
            {
                return;
            }

            if (sourceElement.DataContext is IHierarchy)
            {
                StartHierarchyDrag((IHierarchy)sourceElement.DataContext);
            }
            else if (sourceElement.DataContext is ILevel)
            {
                StartHierarchyDrag(((ILevel)sourceElement.DataContext).Hierarchy);
            }
            else if (sourceElement.DataContext is MembersTree)
            {
                StartHierarchyDrag(((MembersTree)sourceElement.DataContext).Level.Hierarchy);
            }
            else if (sourceElement.DataContext is IMember)
            {
                StartMemberDrag((IMember)sourceElement.DataContext);
            }
            else if (sourceElement.DataContext is CollectionViewGroup)
            {
                CollectionViewGroup group = (CollectionViewGroup)sourceElement.DataContext;
                if (group != null && group.Name is TuplesGroup)
                {
                    TuplesGroup tuplesGroup = (TuplesGroup)group.Name;
                    StartMemberDrag(tuplesGroup.Member);
                }
            }
        }

        private void StartHierarchyDrag(IHierarchy hierarchy)
        {
            DataObject dataObject = new DataObject(typeof(IHierarchy), hierarchy);
            DragDrop.DoDragDrop(_sourceControl, dataObject, DragDropEffects.Move);
        }

        private void StartMemberDrag(IMember member)
        {
            DataObject dataObject = new DataObject(typeof(IMember), member);
            DragDrop.DoDragDrop(_sourceControl, dataObject, DragDropEffects.Move);
        }

        private void OnLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _readyToDrag = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _readyToDrag = false;
        }

        private void OnLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _readyToDrag = false;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object and detaches handlers from the source control's events.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sourceControl.MouseMove -= OnMouseMove;
                _sourceControl.MouseDown -= OnLeftButtonDown;
                _sourceControl.MouseLeave -= OnMouseLeave;
                _sourceControl.MouseUp -= OnLeftButtonUp;
            }
        }

        #endregion
    }
}
