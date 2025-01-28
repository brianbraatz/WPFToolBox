using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
namespace XPad
{
    /// <summary>
    /// Provides the services related to file open/save and stuff.
    /// </summary>
    class TextFileService : AbstractService, ITextFileService
    {

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public TextFileService() : base("Textfile service")
        {            
        }
        #endregion
  
        /// <summary>
        /// Opens a text file. 
        /// </summary>
        /// <param name="path">The path to a directory. If null or empty the system default will be used.</param>
        /// <returns></returns>
        public XPadFile OpenFile(string path)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Text file|*.txt|All files|*.*";
            o.Multiselect = false;
            if (!string.IsNullOrEmpty(path))
                o.InitialDirectory = path;
            string content = string.Empty;
            if (o.ShowDialog() == true)
            {
                using (FileStream stream = File.OpenRead(o.FileName))
                {
                    StreamReader reader = new StreamReader(stream);
                    content = reader.ReadToEnd();
                    stream.Close();
                }
                return new XPadFile("No title", o.FileName, content);

            }

            throw new IOException("No file was selected");

        }

        /// <summary>
        /// Saves the given file to disk.
        /// </summary>
        /// <param name="file">The file.</param>
        public void SaveFile(XPadFile file)
        {
            //if path empty allow the user to pick one
            if (string.IsNullOrEmpty(file.Path))
            {
                SaveFileDialog s = new SaveFileDialog();
                s.Filter = "Text file|*.txt";
                s.RestoreDirectory = true;
                if (s.ShowDialog() == true)
                {
                    file.Path = s.FileName;
                }
            }
            //in both cases, save the file
            using (FileStream stream = File.OpenWrite(file.Path))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(file.Content);
                writer.Flush();
                writer.Close();
                stream.Close();
            }
           
        }


    }
}
