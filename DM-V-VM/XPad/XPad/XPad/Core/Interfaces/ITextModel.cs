using System;
namespace XPad
{
    public interface ITextModel
    {
        string CurrentFileName { get; set; }
        bool IsSaved { get; set; }
        string Text { get; set; }
    }
}
