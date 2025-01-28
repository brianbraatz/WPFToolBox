//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: DragHelper, DropHelperWithoutMeasures and DropHelperWithMeasures classes.
//  Creator: t-tomkm
//  Date Created: 08/30/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Contains logic for dropping items.
    /// </summary>
    internal abstract class DropHelper : IDisposable
    {
        /// <summary>
        /// Factory method that creates new DropHelper.
        /// </summary>
        /// <param name="sourceControl">Control that will use the helper.</param>
        /// <param name="destinationAxis">Axis associated with the control.</param>
        /// <param name="supportsMeasures">Indicates whether the axis accepts measures.</param>
        /// <returns>
        /// New DropHelper object.
        /// </returns>
        /// <remarks>
        /// The helper object will automatically attach its handlers to all needed control's
        /// events.
        /// </remarks>
        public static DropHelper CreateDropHelper(Control sourceControl, IFilterAxis destinationAxis, bool supportsMeasures)
        {
            Assert.ArgumentNotNull(sourceControl, "sourceControl", "Source control cannot be null.");
            if (supportsMeasures)
            {
                return new DropHelperWithMeasures(sourceControl, destinationAxis);
            }
            else
            {
                return new DropHelperWithoutMeasures(sourceControl, destinationAxis);
            }
        }

        private IFilterAxis _destinationAxis;
        private Control _sourceControl;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceControl">Control that will use the helper.</param>
        /// <param name="destinationAxis">Axis associated with the control.</param>
        protected DropHelper(Control sourceControl, IFilterAxis destinationAxis)
        {
            _destinationAxis = destinationAxis;
            _sourceControl = sourceControl;
            _sourceControl.Drop += OnDrop;
        }

        /// <summary>
        /// Gets the destination axis. It can be null.
        /// </summary>
        protected IFilterAxis DestinationAxis
        {
            get { return _destinationAxis; }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IHierarchy)))
            {
                IHierarchy hierarchy = (IHierarchy)e.Data.GetData(typeof(IHierarchy));
                OnHierarchyDrop(hierarchy);
            }
            else
            {
                IMember member = (IMember)e.Data.GetData(typeof(IMember));
                OnMemberDrop(member);
            }
        }

        /// <summary>
        /// Drops a hierarchy. It is a template method.
        /// </summary>
        /// <param name="hierarchy">The hierarchy to drop.</param>
        private void OnHierarchyDrop(IHierarchy hierarchy)
        {
            if (hierarchy.Axis != _destinationAxis)
            {
                if (hierarchy == hierarchy.Model.MeasuresHierarchy)
                {
                    OnMeasureHierarchyDrop(hierarchy);
                }
                else
                {
                    OnDimensionHierarchyDrop(hierarchy);
                }
            }
        }

        /// <summary>
        /// Drops a member. It is a template method.
        /// </summary>
        /// <param name="member">The member to drop.</param>
        private void OnMemberDrop(IMember member)
        {
            if (member is IMeasuresMember)
            {
                OnMeasureMemberDrop((IMeasuresMember)member);
            }
            else
            {
                OnDimensionMemberDrop(member);
            }
        }

        /// <summary>
        /// Drops a dimension hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy to drop.</param>
        protected virtual void OnDimensionHierarchyDrop(IHierarchy hierarchy)
        {
            if (hierarchy.Axis != null)
            {
                hierarchy.Axis.Hierarchies.Remove(hierarchy);
            }
            if (_destinationAxis != null)
            {
                _destinationAxis.Hierarchies.Add(hierarchy);
            }
        }

        /// <summary>
        /// Drops a measures hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy to drop.</param>
        protected abstract void OnMeasureHierarchyDrop(IHierarchy hierarchy);

        /// <summary>
        /// Drops a dimension member. It is used by the OnHierarchyDrop template method.
        /// </summary>
        /// <param name="member">The member to drop.</param>
        protected virtual void OnDimensionMemberDrop(IMember member)
        {
            if (member.Level.Hierarchy.Axis != _destinationAxis)
            {
                OnDimensionHierarchyDrop(member.Level.Hierarchy);
            }
        }

        /// <summary>
        /// Drops a measures member. It is used by the OnMemberDrop template method.
        /// </summary>
        /// <param name="member">The member to drop.</param>
        protected abstract void OnMeasureMemberDrop(IMeasuresMember member);

        #region IDisposable Members

        /// <summary>
        /// Disposes the object and detaches all handlers from the control's events.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indicates whether this method was called from
        /// the IDisposable.Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sourceControl.Drop -= OnDrop;
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains logic for dropping items to a control that doesn't accept measures.
    /// </summary>
    internal class DropHelperWithoutMeasures : DropHelper
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceControl">Control that will use the helper.</param>
        /// <param name="destinationAxis">Axis associated with the control.</param>
        public DropHelperWithoutMeasures(Control sourceControl, IFilterAxis destinationAxis)
            : base(sourceControl, destinationAxis)
        { }

        /// <summary>
        /// Drops a measures hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy to drop.</param>
        protected override void OnMeasureHierarchyDrop(IHierarchy hierarchy)
        { }

        /// <summary>
        /// Drops a measures member.
        /// </summary>
        /// <param name="member">The member to drop.</param>
        protected override void OnMeasureMemberDrop(IMeasuresMember member)
        { }
    }

    /// <summary>
    /// Contains logic for dropping items to a control that accepts measures.
    /// </summary>
    internal class DropHelperWithMeasures : DropHelper
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceControl">Control that will use the helper.</param>
        /// <param name="destinationAxis">Axis associated with the control.</param>
        public DropHelperWithMeasures(Control sourceControl, IFilterAxis destinationAxis)
            : base(sourceControl, destinationAxis)
        { }

        /// <summary>
        /// Drops a measures hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy to drop.</param>
        protected override void OnMeasureHierarchyDrop(IHierarchy hierarchy)
        {
            IFilterAxis axis = hierarchy.Axis;
            if (DestinationAxis != null)
            {
                IList<IMember> filteredMembers;
                IList<IMember> notFilteredMembers;
                if (axis != null)
                {
                    filteredMembers = GetFilteredMembers(hierarchy.Levels[0]);
                    notFilteredMembers = GetNotFilteredMembers(hierarchy.Levels[0]);
                    axis.Hierarchies.Remove(hierarchy);
                }
                else
                {
                    filteredMembers = new List<IMember>(hierarchy.Levels[0].Members);
                    notFilteredMembers = new List<IMember>();
                }
                DestinationAxis.Hierarchies.Add(hierarchy);
                DestinationAxis.FilterMembers(notFilteredMembers, filteredMembers);
            }
            else
            {
                axis.Hierarchies.Remove(hierarchy);
            }
        }

        private IList<IMember> GetFilteredMembers(ILevel level)
        {
            List<IMember> result = new List<IMember>();
            IFilterAxis axis = level.Hierarchy.Axis;
            foreach (IMember member in level.Members)
            {
                if (axis.IsMemberFiltered(member))
                {
                    result.Add(member);
                }
            }
            return result;
        }

        private IList<IMember> GetNotFilteredMembers(ILevel level)
        {
            List<IMember> result = new List<IMember>();
            IFilterAxis axis = level.Hierarchy.Axis;
            foreach (IMember member in level.Members)
            {
                if (!axis.IsMemberFiltered(member))
                {
                    result.Add(member);
                }
            }
            return result;
        }

        /// <summary>
        /// Drops a measures member.
        /// </summary>
        /// <param name="member">The member to drop.</param>
        protected override void OnMeasureMemberDrop(IMeasuresMember member)
        {
            if (DestinationAxis == null)
            {
                RemoveMeasureMember(member);
            }
            else if (member.Level.Hierarchy.Axis == DestinationAxis)
            {
                AddMeasureMember(member);
            }
            else
            {
                OnMeasureHierarchyDrop(member.Level.Hierarchy);
                AddMeasureMember(member);
            }
        }

        private void RemoveMeasureMember(IMeasuresMember member)
        {
            ILevel level = member.Level;
            IFilterAxis axis = level.Hierarchy.Axis;
            if (axis != null)
            {
                axis.FilterMembers(null, new IMember[] { member });
                if (GetNumberOfNotFilteredMembers(axis, level) == 0)
                {
                    axis.Hierarchies.Remove(level.Hierarchy);
                }
            }
        }

        private void AddMeasureMember(IMeasuresMember member)
        {
            ILevel level = member.Level;
            IFilterAxis axis = level.Hierarchy.Axis;
            if (axis != null)
            {
                axis.FilterMembers(new IMember[] { member }, null);
            }
        }

        private int GetNumberOfNotFilteredMembers(IFilterAxis axis, ILevel level)
        {
            int result = 0;
            foreach (IMember member in level.Members)
            {
                if (!axis.IsMemberFiltered(member))
                {
                    result++;
                }
            }
            return result;
        }
    }
}
