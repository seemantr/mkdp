using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
namespace MarkdownPad2.SpellCheck
{
	public class SpellCheckBackgroundRenderer : IBackgroundRenderer
	{
		public TextSegmentCollection<TextSegment> ErrorSegments
		{
			get;
			private set;
		}
		public KnownLayer Layer
		{
			get
			{
				return KnownLayer.Selection;
			}
		}
		public SpellCheckBackgroundRenderer()
		{
			this.ErrorSegments = new TextSegmentCollection<TextSegment>();
		}
		private System.Collections.Generic.IEnumerable<System.Windows.Point> CreatePoints(System.Windows.Point start, System.Windows.Point end, double offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new System.Windows.Point(start.X + (double)i * offset, start.Y - (((i + 1) % 2 == 0) ? offset : 0.0));
			}
			yield break;
		}
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			foreach (TextSegment current in this.ErrorSegments)
			{
				foreach (System.Windows.Rect current2 in BackgroundGeometryBuilder.GetRectsForSegment(textView, current, false))
				{
					System.Windows.Point bottomLeft = current2.BottomLeft;
					System.Windows.Point bottomRight = current2.BottomRight;
					Pen pen = new Pen(new SolidColorBrush(Colors.Red), 1.0);
					pen.Freeze();
					double num = 2.5;
					int count = System.Math.Max((int)((bottomRight.X - bottomLeft.X) / num) + 1, 4);
					StreamGeometry streamGeometry = new StreamGeometry();
					using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
					{
						streamGeometryContext.BeginFigure(bottomLeft, false, false);
						streamGeometryContext.PolyLineTo(this.CreatePoints(bottomLeft, bottomRight, num, count).ToArray<System.Windows.Point>(), true, false);
					}
					streamGeometry.Freeze();
					drawingContext.DrawGeometry(Brushes.Transparent, pen, streamGeometry);
				}
			}
		}
	}
}
