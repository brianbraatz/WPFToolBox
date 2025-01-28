/* Copyright (c) 2007, Dr. WPF
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *   * Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 * 
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 * 
 *   * The name Dr. WPF may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY Dr. WPF 'AS IS' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL Dr. WPF BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DrWPF.Windows.Controls
{
    public static class MenuHelper
    {
        static MenuHelper()
        {
            // get mouse enter/leave events for MenuItem
            EventManager.RegisterClassHandler(typeof(MenuItem),
                MenuItem.MouseEnterEvent, new MouseEventHandler(OnMouseEnterMenuItem), true);

            EventManager.RegisterClassHandler(typeof(MenuItem),
                MenuItem.MouseLeaveEvent, new MouseEventHandler(OnMouseLeaveMenuItem), true);

            // get mouse enter/leave events for Popup
            EventManager.RegisterClassHandler(typeof(Popup),
                MenuItem.MouseEnterEvent, new MouseEventHandler(OnMouseEnterPopup), true);

            EventManager.RegisterClassHandler(typeof(Popup),
                MenuItem.MouseLeaveEvent, new MouseEventHandler(OnMouseLeavePopup), true);

            // listen for the UpdateOverMenuItem event on all MenuItems and Popups
            EventManager.RegisterClassHandler(typeof(MenuItem),
                UpdateOverMenuItemEvent, new RoutedEventHandler(OnUpdateOverMenuItem));

            EventManager.RegisterClassHandler(typeof(Popup),
                UpdateOverMenuItemEvent, new RoutedEventHandler(OnUpdateOverPopup));
        }

        #region IsMouseDirectlyOverMenuItem

        private static readonly DependencyPropertyKey IsMouseDirectlyOverMenuItemPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsMouseDirectlyOverMenuItem", typeof(bool), typeof(MenuHelper),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsMouseDirectlyOverMenuItemProperty 
            = IsMouseDirectlyOverMenuItemPropertyKey.DependencyProperty;

        public static bool GetIsMouseDirectlyOverMenuItem(DependencyObject d)
        {
            return (bool)d.GetValue(IsMouseDirectlyOverMenuItemProperty);
        }

        private static void SetIsMouseDirectlyOverItem(DependencyObject obj, bool value)
        {
            if (obj is MenuItem)
            {
                if (value)
                {
                    if (_currentItem != null)
                    {
                        _currentItem.SetValue(IsMouseDirectlyOverMenuItemPropertyKey, false);
                    }
                    _currentItem = obj as MenuItem;
                }
                else
                {
                    _currentItem = null;
                }
                obj.SetValue(IsMouseDirectlyOverMenuItemPropertyKey, value);
            }
        }

        #endregion

        #region UpdateOverMenuItem event

        // used to find the nearest encapsulating MenuItem to the mouse's current position
        private static readonly RoutedEvent UpdateOverMenuItemEvent = EventManager.RegisterRoutedEvent(
            "UpdateOverMenuItem", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MenuHelper));

        private static void RaiseUpdateOverMenuItem()
        {
            // get the element under the mouse
            IInputElement currentPosition = Mouse.DirectlyOver;

            if (currentPosition != null)
            {
                // raise a bubbled event on the element so it can be handled by an ancestor menu item.
                // if an ancestor menu item receives the event, it will update _currentItem.
                RoutedEventArgs newItemArgs = new RoutedEventArgs(UpdateOverMenuItemEvent);
                currentPosition.RaiseEvent(newItemArgs);
            }
        }

        static void OnUpdateOverMenuItem(object sender, RoutedEventArgs args)
        {
            MenuItem item = sender as MenuItem;

            // the mouse is directly over this item
            SetIsMouseDirectlyOverItem(item, true);

            // prevent this event from routing further
            args.Handled = true;
        }

        static void OnUpdateOverPopup(object sender, RoutedEventArgs args)
        {
            // prevent events from routing from a submenu popup into it's parent menu item
            args.Handled = true;
        }

        #endregion

        #region mouse event handlers for MenuItem

        static void OnMouseEnterMenuItem(object sender, MouseEventArgs args)
        {
            MenuItem item = sender as MenuItem;

            // ignore events that are routed from within the submenu's popup
            if (item.HasItems)
            {
                IInputElement currentElement = Mouse.DirectlyOver;
                Popup popup = item.Template.FindName("PART_Popup", item) as Popup;

                bool isFromPopupChild = (popup != null
                    && currentElement is Visual
                    && (currentElement as Visual).FindCommonVisualAncestor(popup.Child) != null);

                if (isFromPopupChild || (currentElement is Popup))
                    return;
            }

            // the mouse is directly over this item
            SetIsMouseDirectlyOverItem(sender as MenuItem, true);
        }

        static void OnMouseLeaveMenuItem(object sender, MouseEventArgs args)
        {
            // clear property on the current item
            SetIsMouseDirectlyOverItem(sender as MenuItem, false);

            // raise our routed event just in case we're over a new menu item
            RaiseUpdateOverMenuItem();
        }

        #endregion

        #region mouse event handlers for Popup

        static void OnMouseEnterPopup(object sender, MouseEventArgs args)
        {
            // if we were over a menu item, we aren't anymore
            if (_currentItem != null)
            {
                SetIsMouseDirectlyOverItem(_currentItem, false);
            }

            // the popup might have opened underneath the mouse, so raise the 
            // UpdateOverMenuItem event after the next layout
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (ThreadStart)delegate()
            {
                RaiseUpdateOverMenuItem();
            });
        }

        static void OnMouseLeavePopup(object sender, MouseEventArgs args)
        {
            // raise our routed event just in case we're over a new menu item
            RaiseUpdateOverMenuItem();
        }

        #endregion

        // keep a reference to the MenuItem that the mouse is directly over
        private static MenuItem _currentItem = null;
    }
}
