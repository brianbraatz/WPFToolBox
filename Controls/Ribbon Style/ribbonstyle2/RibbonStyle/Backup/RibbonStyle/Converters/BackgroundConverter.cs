using System;
using System.Windows;
using System.Windows.Data;

namespace RibbonStyle
{
    public class BackgroundConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            String name = value as String;
            switch (name)
            {
                case "ToggleBold":
                    return Application.Current.FindResource("BoldIcon");
                case "ToggleItalic":
                    return Application.Current.FindResource("ItalicIcon");
                case "ToggleUnderline":
                    return Application.Current.FindResource("UnderlineIcon");
                case "AlignLeft":
                    return Application.Current.FindResource("AlignLeftIcon");
                case "AlignCenter":
                    return Application.Current.FindResource("AlignCenterIcon");
                case "AlignRight":
                    return Application.Current.FindResource("AlignRightIcon");
                case "AlignJustify":
                    return Application.Current.FindResource("AlignJustifyIcon");
                case "IncreaseIndentation":
                    return Application.Current.FindResource("IncreaseIndentationIcon");
                case "DecreaseIndentation":
                    return Application.Current.FindResource("DecreaseIndentationIcon");
                case "ToggleBullets":
                    return Application.Current.FindResource("BulletsIcon");
                case "ToggleNumbering":
                    return Application.Current.FindResource("NumberingIcon");
                case "Undo":
                    return Application.Current.FindResource("UndoIcon");
                case "Redo":
                    return Application.Current.FindResource("RedoIcon");
                default:
                    return null;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
