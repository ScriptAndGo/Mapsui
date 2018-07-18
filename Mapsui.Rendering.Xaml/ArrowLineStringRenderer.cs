using Mapsui.Geometries;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamlMedia = System.Windows.Media;
using System.Windows.Shapes;
using XamlPoint = System.Windows.Point;
using XamlColors = System.Windows.Media.Colors;

namespace Mapsui.Rendering.Xaml
{
    public static class ArrowLineStringRenderer
    {
        public static Shape Render(ArrowLineString arrowLineString, IStyle style, IViewport viewport)
        {
            var vectorStyle = style is VectorStyle ? (VectorStyle)style : new VectorStyle();
            Path path = LineStringRenderer.CreateLineStringPath(vectorStyle);
            path.StrokeThickness = vectorStyle.Line.Width;

            var arrowHead = arrowLineString[1] as Point;
            var deltaExtremity1 = new Point(arrowLineString.ArrowExtremities[0].X - arrowHead.X, arrowLineString.ArrowExtremities[0].Y - arrowHead.Y);
            var deltaExtremity2 = new Point(arrowLineString.ArrowExtremities[1].X - arrowHead.X, arrowLineString.ArrowExtremities[1].Y - arrowHead.Y);

            var group = new XamlMedia.GeometryGroup();
            XamlMedia.Geometry line = (arrowLineString[0] as LineString).ToXaml();
            group.Children.Add(line);
            XamlMedia.Geometry arrow = CreateArrow(deltaExtremity1, deltaExtremity2);
            group.Children.Add(arrow);

            var arrowMatrix = XamlMedia.Matrix.Identity;
            MatrixHelper.ScaleAt(ref arrowMatrix, viewport.Resolution, viewport.Resolution);
            MatrixHelper.RotateAt(ref arrowMatrix, viewport.Rotation);
            MatrixHelper.Append(ref arrowMatrix, GeometryRenderer.CreateTransformMatrix(arrowHead, viewport));
            arrow.Transform = new XamlMedia.MatrixTransform { Matrix = arrowMatrix };
            line.Transform = new XamlMedia.MatrixTransform { Matrix = GeometryRenderer.CreateTransformMatrix1(viewport) };

            path.Data = group;
            return path;
        }
   
        private static XamlMedia.PathGeometry CreateArrow(Point extremity1, Point extremity2)
        {
            var head = new XamlPoint(0,0);

            var segment1 = new XamlMedia.LineSegment(new XamlPoint(extremity1.X, extremity1.Y), true);
            var segment2 = new XamlMedia.LineSegment(new XamlPoint(extremity2.X, extremity2.Y), true);
            var figure1 = new XamlMedia.PathFigure(head, new XamlMedia.PathSegmentCollection { segment1 }, false);
            var figure2 = new XamlMedia.PathFigure(head, new XamlMedia.PathSegmentCollection { segment2 }, false);
            var figures = new XamlMedia.PathFigureCollection();
            figures.Add(figure1);
            figures.Add(figure2);

            return new XamlMedia.PathGeometry
            {
                Figures = figures
            };
        }

        public static void PositionArrowLineString(Shape renderedGeometry, ArrowLineString arrowLineString, IViewport viewport)
        {
            Path path = renderedGeometry as Path;
            XamlMedia.GeometryGroup data = path.Data as XamlMedia.GeometryGroup;
            XamlMedia.Geometry line = data.Children[0];
            XamlMedia.Geometry arrow = data.Children[1];
            Point arrowHead = arrowLineString[1] as Point;

            var arrowMatrix = XamlMedia.Matrix.Identity;
            MatrixHelper.ScaleAt(ref arrowMatrix, viewport.Resolution, viewport.Resolution);
            MatrixHelper.RotateAt(ref arrowMatrix, viewport.Rotation);
            MatrixHelper.Append(ref arrowMatrix, GeometryRenderer.CreateTransformMatrix(arrowHead, viewport));

            arrow.Transform = new XamlMedia.MatrixTransform { Matrix = arrowMatrix };
            line.Transform = new XamlMedia.MatrixTransform { Matrix = GeometryRenderer.CreateTransformMatrix1(viewport) };
        }
    }
}

