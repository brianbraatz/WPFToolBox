using Microsoft.Samples.WPF.CustomControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    [TestClass()]
    public class ExpandedMembersSetTest
    {
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void CollapseMemberTest()
        {
            object target = Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor accessor = new Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor(target);
            TestMember member1 = new TestMember("member1");
            TestMember member2 = new TestMember("member2");
            TestMember member3 = new TestMember("member3");
            member1.AddChild(member3);
            member3.ParentMember = member1;
            
            accessor.ExpandMember(member1);
            accessor.CollapseMember(member2);
            Assert.AreEqual(MemberState.Expanded, accessor.GetMemberState(member1));
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member2));
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member3));

            accessor.CollapseMember(member1);
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member1));
            Assert.AreEqual(MemberState.Hidden, accessor.GetMemberState(member3));
        }

        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void ExpandMemberTest()
        {
            object target = Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor accessor = new Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor(target);
            TestMember member1 = new TestMember("member1");
            TestMember member2 = new TestMember("member2");
            TestMember member3 = new TestMember("member3");
            member2.AddChild(member3);
            member3.ParentMember = member2;

            accessor.ExpandMember(member1);
            accessor.CollapseMember(member2);
            Assert.AreEqual(MemberState.Expanded, accessor.GetMemberState(member1));
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member2));
            Assert.AreEqual(MemberState.Hidden, accessor.GetMemberState(member3));

            accessor.ExpandMember(member2);
            Assert.AreEqual(MemberState.Expanded, accessor.GetMemberState(member2));
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member3));
        }

        /// <summary>
        /// A test for OnHierarchiesCleared() method.
        /// </summary>
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void OnHierarchiesClearedTest()
        {
            object target = Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor accessor = new Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor(target);
            TestMember member1 = new TestMember("member1");

            accessor.ExpandMember(member1);
            accessor.OnHierarchiesCleared();
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member1));
        }

        /// <summary>
        /// A test for OnHierarchyRemoved(IHierarchy) method.
        /// </summary>
        [DeploymentItem("Microsoft.Samples.WPF.CustomControls.PivotTable.dll")]
        [TestMethod()]
        public void OnHierarchyRemovedTest()
        {
            object target = Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor.CreatePrivate();
            Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor accessor = new Microsoft.Samples.WPF.CustomControls.PivotTable.Tests.Microsoft_Samples_WPF_CustomControls_DataModel_ExpandedMembersSetAccessor(target);
            TestHierarchy hierarchy1 = new TestHierarchy("hierarchy1");
            TestHierarchy hierarchy2 = new TestHierarchy("hierarchy2");
            TestLevel level1 = new TestLevel("level1");
            level1.Hierarchy = hierarchy1;
            TestLevel level2 = new TestLevel("level2");
            level2.Hierarchy = hierarchy2;
            TestMember member1 = new TestMember("member1");
            member1.Level = level1;
            TestMember member2 = new TestMember("member2");
            member2.Level = level2;

            accessor.ExpandMember(member1);
            accessor.ExpandMember(member2);
            accessor.OnHierarchyRemoved(hierarchy1);
            Assert.AreEqual(MemberState.Collapsed, accessor.GetMemberState(member1));
            Assert.AreEqual(MemberState.Expanded, accessor.GetMemberState(member2));
        }
    }
}
