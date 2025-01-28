using System;

namespace WpfBootcampDemo
{
    /// <summary>
    /// Represents an object that displays the
    /// image associated with an Xray.
    /// </summary>
    public interface IXrayImageViewer
    {
        void ShowImage(Uri imageLocation);
    }
}