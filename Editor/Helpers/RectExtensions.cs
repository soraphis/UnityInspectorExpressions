using System;
using UnityEngine;

namespace Editor.Helpers
{
    public static class RectExtensions
    {
        public static Rect Shift(this Rect self, float x, float y)
        {
            var output = new Rect(self);
            output.x += x;
            output.y += y;
            return output;
        }


        /// <returns>A new Rect that has "padding" space around the initial rect</returns>
        public static Rect Padding(this Rect self, float padding)
        {
            var output = new Rect(self);
            output.xMin += padding;
            output.yMin += padding;
            output.xMax -= padding;
            output.yMax -= padding;
            return output;
        }

        /// <returns>A new Rect that has "padding" space around the initial rect</returns>
        public static Rect Padding(this Rect self, float paddingX, float paddingY)
        {
            var output = new Rect(self);
            output.xMin += paddingX;
            output.yMin += paddingY;
            output.xMax -= paddingX;
            output.yMax -= paddingY;
            return output;
        }

        /// <returns>A new Rect that has "padding" space around the initial rect</returns>
        public static Rect Padding(this Rect self, float paddingLeft, float paddingRight, float paddingTop, float paddingBottom)
        {
            var output = new Rect(self);
            output.xMin += paddingLeft;
            output.yMin += paddingTop;
            output.xMax -= paddingRight;
            output.yMax -= paddingBottom;
            return output;
        }

        public static Rect RowPrepend(this Rect self, float left)
        {
            return new Rect(self) { xMin = self.xMin - left, xMax = self.xMin };
        }

        public static Rect ColPostpend(this Rect self, float bottom)
        {
            return new Rect(self) { yMin = self.yMax, yMax = self.yMax + bottom };
        }

        public static Rect CutLeft(this Rect self, float leftWidth, out Rect right)
        {
            right = new Rect(self) { xMin = self.xMin + leftWidth };
            return new Rect(self) { xMax = self.xMin + leftWidth };
        }

        public static Rect CutBottom(this Rect self, float bottomHeight, out Rect top)
        {
            top = new Rect(self) { yMax = self.yMax - bottomHeight };
            return new Rect(self) { yMin = self.yMax - bottomHeight };
        }

        public static Rect CutTop(this Rect self, float topHeight, out Rect bottom)
        {
            bottom = new Rect(self) { yMin = self.yMin + topHeight };
            return new Rect(self) { yMax = self.yMin + topHeight };
        }


        public static Rect WithHeight(this Rect self, float height)
        {
            return new Rect(self) { height = height };
        }

        // //////////////////////////////

        private static RectLayoutBuild s_LayoutBuilder = new ();
        // public static void Row(this Rect self, Action<IRectLayoutBuilder> setup, Span<Rect> output) // TODO: instead of lambda, return a disposable builder object ... for performance reasons ... but should not be tooo bad for the editor any ways.
        // {
        // 	s_LayoutBuilder.Clear(self);
        // 	setup?.Invoke(s_LayoutBuilder);
        // 	s_LayoutBuilder.GetRects(output);
        // }

        public static RectLayout Row(this Rect self, Span<Rect> output)
        {
            s_LayoutBuilder.Clear(self);
            return new RectLayout(s_LayoutBuilder, output);
        }
    }
}