using System;
using System.Collections.Generic;
using System.Text;

namespace MyAdvancedListbox
{
    public class StudentResult
    {
        // Fields
        private string _id;
        private string _name;      
        private int _result;      
        private string _comment;
        private string _picture;

        // Properties
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }

        // Constructors
        public StudentResult() { }

        public StudentResult(string id, string name, int result, string comment, string picture)
        {
            this.Id = Id;
            this.Name = name;
            this.Result = result;
            this.Comment = comment;
            this.Picture = picture;
        }

        // Methods
        public override string ToString()
        {
            return this.Name;
        }
    }
}
