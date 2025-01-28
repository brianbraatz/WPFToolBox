//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: TuplesGroupDescription class.
//  Creator: t-tomkm
//  Date Created: 08/16/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Class that returns group descriptions for tuples. It is used by the MembersDisplayControls
    /// to display tuples in a tree view.
    /// </summary>
    internal class TuplesGroupDescription : GroupDescription
    {
        /// <summary>
        /// Returns object that describes tuples group.
        /// </summary>
        /// <param name="item">Tuple to return the group for.</param>
        /// <param name="treeLevel">Level into the tree.</param>
        /// <param name="culture">Information about the current culture.</param>
        /// <returns>Returns TuplesGroup for the given tuple.</returns>
        public override object GroupNameFromItem(object item, int treeLevel, CultureInfo culture)
        {
            Tuple tuple = (Tuple)item;
            IFilterAxis axis = tuple[0].Level.Hierarchy.Axis;
            int hierarchyNumber = GetHierarchyNumberForTreeLevel(axis, treeLevel);
            if (hierarchyNumber == -1)
            {
                return null;
            }
            int totalLevelsCount = GetTotalLevelsCount(axis, axis.Hierarchies[hierarchyNumber]);
            int levelWithinHierarchy = axis.Hierarchies[hierarchyNumber].Levels.Count - totalLevelsCount + treeLevel;
            IMember member = GetMemberByLevelNumber(tuple[hierarchyNumber], levelWithinHierarchy);
            
            return new TuplesGroup(axis.Hierarchies[hierarchyNumber].Levels[levelWithinHierarchy], member);
        }

        private int GetHierarchyNumberForTreeLevel(IFilterAxis axis, int treeLevel)
        {
            int currentLevel = 0;
            for (int i = 0; i < axis.Hierarchies.Count; i++)
            {
                IHierarchy hierarchy = axis.Hierarchies[i];
                currentLevel += hierarchy.Levels.Count;
                if (currentLevel > treeLevel)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetTotalLevelsCount(IFilterAxis axis, IHierarchy lastHierarchy)
        {
            int result = 0;

            foreach (IHierarchy hierarchy in axis.Hierarchies)
            {
                result += hierarchy.Levels.Count;
                if (hierarchy == lastHierarchy)
                {
                    break;
                }
            }
            return result;
        }

        private IMember GetMemberByLevelNumber(IMember memberOnPath, int levelNumber)
        {
            IMember currentMember = memberOnPath;
            while (currentMember.Level.LevelNumber > levelNumber)
            {
                currentMember = currentMember.ParentMember;
            }
            return currentMember;
        }
    }
}
