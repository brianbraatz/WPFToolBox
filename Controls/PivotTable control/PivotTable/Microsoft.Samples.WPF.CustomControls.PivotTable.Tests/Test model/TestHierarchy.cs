using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.WPF.CustomControls.PivotTable.Tests
{
    public class TestHierarchy : IHierarchy
    {
        private IDataModel _model;
        private string _uniqueName;
        private string _displayFolder;
        private IFilterAxis _axis;
        private ObservableCollection<ILevel> _levels;
        private ReadOnlyObservableCollection<ILevel> _readOnlyLevelsView;

        public TestHierarchy(string uniqueName)
            : this(uniqueName, null)
        { }

        public TestHierarchy(string uniqueName, IAxis axis)
        {
            _uniqueName = uniqueName;
            _displayFolder = "";
            _axis = axis;
            _levels = new ObservableCollection<ILevel>();
            _readOnlyLevelsView = new ReadOnlyObservableCollection<ILevel>(_levels);
        }

        public string Caption
        {
            get { return _uniqueName; }
        }

        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        public string DisplayFolder
        {
            get { return _displayFolder; }
            set { _displayFolder = value; }
        }

        public IFilterAxis Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }

        public ReadOnlyObservableCollection<ILevel> Levels
        {
            get { return _readOnlyLevelsView; }
        }

        public void AddLevel(ILevel level)
        {
            _levels.Add(level);
        }

        public void RemoveLevel(ILevel level)
        {
            _levels.Remove(level);
        }

        public IDataModel Model 
        {
            get { return _model; }
            set { _model = value; }
        }
    }
}
