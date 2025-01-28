using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.InteropServices;

namespace WpfApplication1
{

    static class DoubleUtil
    {
        // Fields
        internal const double DBL_EPSILON = 2.2204460492503131E-16;
        internal const float FLT_MIN = 1.175494E-38f;

        // Methods
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }

        public static bool AreClose(Point point1, Point point2)
        {
            return (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));
        }

        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return (((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width));
        }

        public static bool AreClose(Size size1, Size size2)
        {
            return (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));
        }

        public static bool AreClose(Vector vector1, Vector vector2)
        {
            return (AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y));
        }

        public static int DoubleToInt(double val)
        {
            if (0.0 >= val)
            {
                return (int)(val - 0.5);
            }
            return (int)(val + 0.5);
        }

        public static bool GreaterThan(double value1, double value2)
        {
            return ((value1 > value2) && !AreClose(value1, value2));
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0.0) && LessThanOrClose(val, 1.0));
        }

        public static bool IsNaN(double value)
        {
            NanUnion union = new NanUnion();
            union.DoubleValue = value;
            ulong num = union.UintValue & 18442240474082181120L;
            ulong num2 = union.UintValue & ((ulong)0xfffffffffffffL);
            if ((num != 0x7ff0000000000000L) && (num != 18442240474082181120L))
            {
                return false;
            }
            return (num2 != 0L);
        }

        public static bool IsOne(double value)
        {
            return (Math.Abs((double)(value - 1.0)) < 2.2204460492503131E-15);
        }

        public static bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }

        public static bool LessThan(double value1, double value2)
        {
            return ((value1 < value2) && !AreClose(value1, value2));
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool RectHasNaN(Rect r)
        {
            if ((!IsNaN(r.X) && !IsNaN(r.Y)) && (!IsNaN(r.Height) && !IsNaN(r.Width)))
            {
                return false;
            }
            return true;
        }

        // Nested Types
        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            // Fields
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }
    }


    public class InverseWrapPanel : WrapPanel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            int start = 0;
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            double v = 0.0;
            double itemU = (this.Orientation == Orientation.Horizontal) ? itemWidth : itemHeight;
            UVSize size = new UVSize(this.Orientation);
            UVSize size2 = new UVSize(this.Orientation, finalSize.Width, finalSize.Height);
            bool flag = !DoubleUtil.IsNaN(itemWidth);
            bool flag2 = !DoubleUtil.IsNaN(itemHeight);
            bool useItemU = (this.Orientation == Orientation.Horizontal) ? flag : flag2;
            UIElementCollection internalChildren = base.InternalChildren;
            int end = 0;
            int count = internalChildren.Count;
            while (end < count)
            {
                UIElement element = internalChildren[end];
                if (element != null)
                {
                    UVSize size3 = new UVSize(this.Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(size.U + size3.U, size2.U))
                    {
                        this.arrangeLine(v, size.V, start, end, useItemU, itemU);
                        v += size.V;
                        size = size3;
                        if (DoubleUtil.GreaterThan(size3.U, size2.U))
                        {
                            this.arrangeLine(v, size3.V, end, ++end, useItemU, itemU);
                            v += size3.V;
                            size = new UVSize(this.Orientation);
                        }
                        start = end;
                    }
                    else
                    {
                        size.U += size3.U;
                        size.V = Math.Max(size3.V, size.V);
                    }
                }
                end++;
            }
            if (start < internalChildren.Count)
            {
                this.arrangeLine(v, size.V, start, internalChildren.Count, useItemU, itemU);
            }
            return finalSize;

        }

        private void arrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            double num = 0.0;
            bool flag = this.Orientation == Orientation.Horizontal;
            UIElementCollection internalChildren = base.InternalChildren;
            for (int i = end-1; i >= start; i--)//Changed here to arrange the items inverted
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    UVSize size = new UVSize(this.Orientation, element.DesiredSize.Width, element.DesiredSize.Height);
                    double num3 = useItemU ? itemU : size.U;
                    element.Arrange(new Rect(flag ? num : v, flag ? v : num, flag ? num3 : lineV, flag ? lineV : num3));
                    num += num3;
                }
            }
        }

 
        [StructLayout(LayoutKind.Sequential)]
        private struct UVSize
        {
            internal double U;
            internal double V;
            private Orientation _orientation;
            internal UVSize(Orientation orientation, double width, double height)
            {
                this.U = this.V = 0.0;
                this._orientation = orientation;
                this.Width = width;
                this.Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                this.U = this.V = 0.0;
                this._orientation = orientation;
            }

            internal double Width
            {
                get
                {
                    if (this._orientation != Orientation.Horizontal)
                    {
                        return this.V;
                    }
                    return this.U;
                }
                set
                {
                    if (this._orientation == Orientation.Horizontal)
                    {
                        this.U = value;
                    }
                    else
                    {
                        this.V = value;
                    }
                }
            }
            internal double Height
            {
                get
                {
                    if (this._orientation != Orientation.Horizontal)
                    {
                        return this.U;
                    }
                    return this.V;
                }
                set
                {
                    if (this._orientation == Orientation.Horizontal)
                    {
                        this.V = value;
                    }
                    else
                    {
                        this.U = value;
                    }
                }
            }
        }
    }
}
