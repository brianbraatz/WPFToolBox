using System;
using System.ComponentModel;

namespace WpfBootcampDemo.Model
{
    /// <summary>
    /// Represnts an x-ray and information about it.
    /// </summary>
    public class Xray : INotifyPropertyChanged
    {
        #region Data

        readonly DateTime _creationDate;
        readonly string _fileName;
        bool _hasBeenViewed;
        readonly XraySide _side;

        #endregion // Data

        #region Constructor
        
        public Xray(DateTime creationDate, string fileName, XraySide side)
        {
            _creationDate = creationDate;
            _fileName = fileName;
            _side = side;
        }

        #endregion // Constructor

        #region Properties

        public DateTime CreationDate
        {
            get { return _creationDate; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public bool HasBeenViewed
        {
            get { return _hasBeenViewed; }
            set 
            {
                if (value == _hasBeenViewed)
                    return;

                _hasBeenViewed = value;

                this.OnPropertyChanged("HasBeenViewed");
            }
        }

        public XraySide Side
        {
            get { return _side; }
        }

        #endregion // Properties

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if(handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public enum XraySide
    {
        Left,
        Front,
        Right,
        Back
    }
}