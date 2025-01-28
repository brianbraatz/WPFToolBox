/*
 * Created by: Peter Weissbrod
 * Created: Tuesday, January 22, 2008
 */
using System;
using System.ComponentModel;
using PostSharp.UserSamples.Validators;

namespace BloggerApp.Models
{
    /// <summary>
    /// generic model definition
    /// </summary>
    public abstract class BusinessObject:INotifyPropertyChanged
    {
        #region members
        private readonly int m_ID;
        private string m_Name;
        #endregion

        #region properties
        [IntegerRangeValidator(MinVal = 0,MaxVal = int.MaxValue)]
        public int ID
        {
            get { return m_ID; }
        }
        [NotNullValidator]
        [StringLengthValidator(MinLength = 1,MaxLength = 254)]
        public string Name
        {
            get { return m_Name; }
            set {
                m_Name = value;
                if(PropertyChanged!=null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }
        #endregion

        #region public methods
        public BusinessObject(string name, int id)
        {
            if(name==string.Empty)
                throw new Exception("Name cannot be empty");
            m_Name = name;
            m_ID = id;
        }
        #endregion

        #region private methods
        #endregion

        #region INotifyPropertyChanged Members
        ///<summary>
        ///Occurs when a property value changes.
        ///</summary>
        ///
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}