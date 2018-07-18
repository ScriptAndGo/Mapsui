using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mapsui.Rendering.Skia
{
    public static class ArrowLineStringRenderer
    {
        public static void Draw(SKCanvas canvas, IViewport viewport, IStyle style, IFeature feature, IGeometry geometry, float opacity)
        {
            var arrowLineString = (ArrowLineString)geometry;
            LineStringRenderer.Draw(canvas, viewport, style, feature, arrowLineString[0], opacity);

            var arrowHead = arrowLineString.ArrowHead as Point;
            var destination = viewport.WorldToScreen(arrowHead);
            var deltaExtremity1 = new Point(arrowLineString.ArrowExtremities[0].X - arrowHead.X, arrowLineString.ArrowExtremities[0].Y - arrowHead.Y);
            var deltaExtremity2 = new Point(arrowLineString.ArrowExtremities[1].X - arrowHead.X, arrowLineString.ArrowExtremities[1].Y - arrowHead.Y);

            DrawArrow(canvas, style, destination, deltaExtremity1, deltaExtremity2, opacity);
        }

        private static void DrawArrow(SKCanvas canvas, IStyle style, Point destination, Point extremity1, Point extremity2, float opacity)
        {
            var vectorStyle = style is VectorStyle ? (VectorStyle)style : new VectorStyle();
            canvas.Save();
            canvas.Translate((float)destination.X, (float)destination.Y);
            
            var path = new SKPath();
            path.MoveTo(0, 0);
            path.LineTo((float)extremity1.X, -1 * (float)extremity1.Y);
            path.MoveTo(0, 0);
            path.LineTo((float)extremity2.X, -1 * (float)extremity2.Y);

            var linePaint = CreateLinePaint(vectorStyle.Line, opacity);
            if ((linePaint != null) && linePaint.Color.Alpha != 0) canvas.DrawPath(path, linePaint);
            canvas.Restore();
        }

        private static SKPaint CreateLinePaint(Pen line, float opacity)
        {
            if (line == null) return null;

            return new SKPaint
            {
                Color = line.Color.ToSkia(opacity),
                StrokeWidth = (float)line.Width,
                StrokeCap = line.PenStrokeCap.ToSkia(),
                PathEffect = line.PenStyle.ToSkia((float)line.Width),
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
        }
    }
}
