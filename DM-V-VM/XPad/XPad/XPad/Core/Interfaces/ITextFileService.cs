using System;
namespace XPad
{
    interface ITextFileService
    {
        XPadFile OpenFile(string path);
        void SaveFile(XPadFile file);
    }
}
