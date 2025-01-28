using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Documents;

namespace RibbonStyle
{
    public class CommandConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            String name = value as String;
            switch (name)
            {
                case "ToggleBold":
                    return EditingCommands.ToggleBold;
                case "ToggleItalic":
                    return EditingCommands.ToggleItalic;
                case "ToggleUnderline":
                    return EditingCommands.ToggleUnderline;
                case "AlignLeft":
                    return EditingCommands.AlignLeft;
                case "AlignCenter":
                    return EditingCommands.AlignCenter;
                case "AlignRight":
                    return EditingCommands.AlignRight;
                case "AlignJustify":
                    return EditingCommands.AlignJustify;
                case "IncreaseIndentation":
                    return EditingCommands.IncreaseIndentation;
                case "DecreaseIndentation":
                    return EditingCommands.DecreaseIndentation;
                case "ToggleBullets":
                    return EditingCommands.ToggleBullets;
                case "ToggleNumbering":
                    return EditingCommands.ToggleNumbering;
                case "Undo":
                    return ApplicationCommands.Undo;
                case "Redo":
                    return ApplicationCommands.Redo;
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
