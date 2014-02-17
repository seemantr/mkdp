using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace MarkdownPad2.UserControls
{
	public class ScrollableTabPanel : Panel, IScrollInfo, INotifyPropertyChanged
	{
		private ScrollViewer _svOwningScrollViewer;
		private bool _fCanScroll_H = true;
		private System.Windows.Size _szControlExtent = new System.Windows.Size(0.0, 0.0);
		private System.Windows.Size _szViewport = new System.Windows.Size(0.0, 0.0);
		private System.Windows.Vector _vOffset;
		private static GradientStopCollection _gscOpacityMaskStops_TransparentOnLeftAndRight = new GradientStopCollection
		{
			new GradientStop(Colors.Transparent, 0.0),
			new GradientStop(Colors.Black, 0.2),
			new GradientStop(Colors.Black, 0.8),
			new GradientStop(Colors.Transparent, 1.0)
		};
		private static GradientStopCollection _gscOpacityMaskStops_TransparentOnLeft = new GradientStopCollection
		{
			new GradientStop(Colors.Transparent, 0.0),
			new GradientStop(Colors.Black, 0.5)
		};
		private static GradientStopCollection _gscOpacityMaskStops_TransparentOnRight = new GradientStopCollection
		{
			new GradientStop(Colors.Black, 0.5),
			new GradientStop(Colors.Transparent, 1.0)
		};
		private TranslateTransform _ttScrollTransform = new TranslateTransform();
		public static readonly System.Windows.DependencyProperty RightOverflowMarginProperty = System.Windows.DependencyProperty.Register("RightOverflowMargin", typeof(int), typeof(ScrollableTabPanel), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly System.Windows.DependencyProperty AnimationTimeSpanProperty = System.Windows.DependencyProperty.Register("AnimationTimeSpanProperty", typeof(System.TimeSpan), typeof(ScrollableTabPanel), new FrameworkPropertyMetadata(new System.TimeSpan(0, 0, 0, 0, 100), FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly System.Windows.DependencyProperty LineScrollPixelCountProperty = System.Windows.DependencyProperty.Register("LineScrollPixelCount", typeof(int), typeof(ScrollableTabPanel), new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsRender));
		public event PropertyChangedEventHandler PropertyChanged;
		public bool CanHorizontallyScroll
		{
			get
			{
				return this._fCanScroll_H;
			}
			set
			{
				this._fCanScroll_H = value;
			}
		}
		public bool CanVerticallyScroll
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		public double ExtentHeight
		{
			get
			{
				return this.Extent.Height;
			}
		}
		public double ExtentWidth
		{
			get
			{
				return this.Extent.Width;
			}
		}
		public double HorizontalOffset
		{
			get
			{
				return this._vOffset.X;
			}
			private set
			{
				this._vOffset.X = value;
			}
		}
		public ScrollViewer ScrollOwner
		{
			get
			{
				return this._svOwningScrollViewer;
			}
			set
			{
				this._svOwningScrollViewer = value;
				if (this._svOwningScrollViewer != null)
				{
					this.ScrollOwner.Loaded += new RoutedEventHandler(this.ScrollOwner_Loaded);
					return;
				}
				this.ScrollOwner.Loaded -= new RoutedEventHandler(this.ScrollOwner_Loaded);
			}
		}
		public double VerticalOffset
		{
			get
			{
				return 0.0;
			}
		}
		public double ViewportHeight
		{
			get
			{
				return this.Viewport.Height;
			}
		}
		public double ViewportWidth
		{
			get
			{
				return this.Viewport.Width;
			}
		}
		public System.Windows.Size Extent
		{
			get
			{
				return this._szControlExtent;
			}
			private set
			{
				this._szControlExtent = value;
			}
		}
		public System.Windows.Size Viewport
		{
			get
			{
				return this._szViewport;
			}
			private set
			{
				this._szViewport = value;
			}
		}
		public bool IsOnFarLeft
		{
			get
			{
				return this.HorizontalOffset == 0.0;
			}
		}
		public bool IsOnFarRight
		{
			get
			{
				return this.HorizontalOffset + this.Viewport.Width == this.ExtentWidth;
			}
		}
		public bool CanScroll
		{
			get
			{
				return this.ExtentWidth > this.Viewport.Width;
			}
		}
		public bool CanScrollLeft
		{
			get
			{
				return this.CanScroll && !this.IsOnFarLeft;
			}
		}
		public bool CanScrollRight
		{
			get
			{
				return this.CanScroll && !this.IsOnFarRight;
			}
		}
		public int RightOverflowMargin
		{
			get
			{
				return (int)base.GetValue(ScrollableTabPanel.RightOverflowMarginProperty);
			}
			set
			{
				base.SetValue(ScrollableTabPanel.RightOverflowMarginProperty, value);
			}
		}
		public System.TimeSpan AnimationTimeSpan
		{
			get
			{
				return (System.TimeSpan)base.GetValue(ScrollableTabPanel.AnimationTimeSpanProperty);
			}
			set
			{
				base.SetValue(ScrollableTabPanel.AnimationTimeSpanProperty, value);
			}
		}
		public int LineScrollPixelCount
		{
			get
			{
				return (int)base.GetValue(ScrollableTabPanel.LineScrollPixelCountProperty);
			}
			set
			{
				base.SetValue(ScrollableTabPanel.LineScrollPixelCountProperty, value);
			}
		}
		public ScrollableTabPanel()
		{
			base.RenderTransform = this._ttScrollTransform;
			base.SizeChanged += new SizeChangedEventHandler(this.ScrollableTabPanel_SizeChanged);
		}
		private static double CalculateNewScrollOffset(double dblViewport_Left, double dblViewport_Right, double dblChild_Left, double dblChild_Right)
		{
			bool flag = dblChild_Left < dblViewport_Left && dblChild_Right < dblViewport_Right;
			bool flag2 = dblChild_Right > dblViewport_Right && dblChild_Left > dblViewport_Left;
			bool flag3 = dblChild_Right - dblChild_Left > dblViewport_Right - dblViewport_Left;
			if (!flag2 && !flag)
			{
				return dblViewport_Left;
			}
			if (flag && !flag3)
			{
				return dblChild_Left;
			}
			return dblChild_Right - (dblViewport_Right - dblViewport_Left);
		}
		private void UpdateMembers(System.Windows.Size szExtent, System.Windows.Size szViewportSize)
		{
			if (szExtent != this.Extent)
			{
				this.Extent = szExtent;
				if (this.ScrollOwner != null)
				{
					this.ScrollOwner.InvalidateScrollInfo();
				}
			}
			if (szViewportSize != this.Viewport)
			{
				this.Viewport = szViewportSize;
				if (this.ScrollOwner != null)
				{
					this.ScrollOwner.InvalidateScrollInfo();
				}
			}
			if (this.HorizontalOffset + this.Viewport.Width + (double)this.RightOverflowMargin > this.ExtentWidth)
			{
				this.SetHorizontalOffset(this.HorizontalOffset + this.Viewport.Width + (double)this.RightOverflowMargin);
			}
			this.NotifyPropertyChanged("CanScroll");
			this.NotifyPropertyChanged("CanScrollLeft");
			this.NotifyPropertyChanged("CanScrollRight");
		}
		private double getLeftEdge(UIElement uieChild)
		{
			double num = 0.0;
			foreach (UIElement uIElement in base.InternalChildren)
			{
				double width = uIElement.DesiredSize.Width;
				if (uieChild != null && uieChild == uIElement)
				{
					return num;
				}
				num += width;
			}
			return num;
		}
		public bool IsPartlyVisible(UIElement uieChild)
		{
			System.Windows.Rect intersectionRectangle = this.GetIntersectionRectangle(uieChild);
			return !(intersectionRectangle == System.Windows.Rect.Empty);
		}
		public double PartlyVisiblePortion_OverflowToRight(UIElement uieChild)
		{
			System.Windows.Rect intersectionRectangle = this.GetIntersectionRectangle(uieChild);
			double result = 1.0;
			if (!(intersectionRectangle == System.Windows.Rect.Empty) && this.CanScrollRight && intersectionRectangle.Width < uieChild.DesiredSize.Width && intersectionRectangle.X > 0.0)
			{
				result = intersectionRectangle.Width / uieChild.DesiredSize.Width;
			}
			return result;
		}
		public double PartlyVisiblePortion_OverflowToLeft(UIElement uieChild)
		{
			System.Windows.Rect intersectionRectangle = this.GetIntersectionRectangle(uieChild);
			double result = 1.0;
			if (!(intersectionRectangle == System.Windows.Rect.Empty) && this.CanScrollLeft && intersectionRectangle.Width < uieChild.DesiredSize.Width && intersectionRectangle.X == 0.0)
			{
				result = intersectionRectangle.Width / uieChild.DesiredSize.Width;
			}
			return result;
		}
		private System.Windows.Rect GetScrollViewerRectangle()
		{
			return new System.Windows.Rect(new System.Windows.Point(0.0, 0.0), this.ScrollOwner.RenderSize);
		}
		private System.Windows.Rect GetChildRectangle(UIElement uieChild)
		{
			GeneralTransform generalTransform = uieChild.TransformToAncestor(this.ScrollOwner);
			return generalTransform.TransformBounds(new System.Windows.Rect(new System.Windows.Point(0.0, 0.0), uieChild.RenderSize));
		}
		private System.Windows.Rect GetIntersectionRectangle(UIElement uieChild)
		{
			System.Windows.Rect scrollViewerRectangle = this.GetScrollViewerRectangle();
			System.Windows.Rect childRectangle = this.GetChildRectangle(uieChild);
			return System.Windows.Rect.Intersect(scrollViewerRectangle, childRectangle);
		}
		private void RemoveOpacityMasks()
		{
			foreach (UIElement uieChild in base.Children)
			{
				this.RemoveOpacityMask(uieChild);
			}
		}
		private void RemoveOpacityMask(UIElement uieChild)
		{
			uieChild.OpacityMask = null;
		}
		private void UpdateOpacityMasks()
		{
			foreach (UIElement uieChild in base.Children)
			{
				this.UpdateOpacityMask(uieChild);
			}
		}
		private void UpdateOpacityMask(UIElement uieChild)
		{
			if (uieChild == null)
			{
				return;
			}
			System.Windows.Rect scrollViewerRectangle = this.GetScrollViewerRectangle();
			if (scrollViewerRectangle == System.Windows.Rect.Empty)
			{
				return;
			}
			System.Windows.Rect childRectangle = this.GetChildRectangle(uieChild);
			if (scrollViewerRectangle.Contains(childRectangle))
			{
				uieChild.OpacityMask = null;
				return;
			}
			double num = this.PartlyVisiblePortion_OverflowToLeft(uieChild);
			double num2 = this.PartlyVisiblePortion_OverflowToRight(uieChild);
			if (num < 1.0 && num2 < 1.0)
			{
				uieChild.OpacityMask = new LinearGradientBrush(ScrollableTabPanel._gscOpacityMaskStops_TransparentOnLeftAndRight, new System.Windows.Point(0.0, 0.0), new System.Windows.Point(1.0, 0.0));
				return;
			}
			if (num < 1.0)
			{
				uieChild.OpacityMask = new LinearGradientBrush(ScrollableTabPanel._gscOpacityMaskStops_TransparentOnLeft, new System.Windows.Point(1.0 - num, 0.0), new System.Windows.Point(1.0, 0.0));
				return;
			}
			if (num2 < 1.0)
			{
				uieChild.OpacityMask = new LinearGradientBrush(ScrollableTabPanel._gscOpacityMaskStops_TransparentOnRight, new System.Windows.Point(0.0, 0.0), new System.Windows.Point(num2, 0.0));
				return;
			}
			uieChild.OpacityMask = null;
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
		{
			System.Windows.Size size = new System.Windows.Size(0.0, availableSize.Height);
			foreach (UIElement uIElement in base.InternalChildren)
			{
				uIElement.Measure(availableSize);
				size.Width += uIElement.DesiredSize.Width;
			}
			this.UpdateMembers(size, availableSize);
			double width = double.IsPositiveInfinity(availableSize.Width) ? size.Width : availableSize.Width;
			size.Width = width;
			return size;
		}
		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
		{
			if (base.InternalChildren == null || base.InternalChildren.Count < 1)
			{
				return finalSize;
			}
			double num = 0.0;
			foreach (UIElement uIElement in base.InternalChildren)
			{
				double width = uIElement.DesiredSize.Width;
				uIElement.Arrange(new System.Windows.Rect(num, 0.0, width, uIElement.DesiredSize.Height));
				num += width;
			}
			return finalSize;
		}
		protected override void OnVisualChildrenChanged(System.Windows.DependencyObject visualAdded, System.Windows.DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			this.UpdateOpacityMasks();
		}
		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			base.OnChildDesiredSizeChanged(child);
			this.UpdateOpacityMasks();
		}
		public void LineDown()
		{
		}
		public void LineLeft()
		{
			this.SetHorizontalOffset(this.HorizontalOffset - (double)this.LineScrollPixelCount);
		}
		public void LineRight()
		{
			this.SetHorizontalOffset(this.HorizontalOffset + (double)this.LineScrollPixelCount);
		}
		public void LineUp()
		{
		}
		public System.Windows.Rect MakeVisible(Visual visual, System.Windows.Rect rectangle)
		{
			if (rectangle.IsEmpty || visual == null || visual == this || !base.IsAncestorOf(visual))
			{
				return System.Windows.Rect.Empty;
			}
			double num = 0.0;
			UIElement uIElement = null;
			for (int i = 0; i < base.InternalChildren.Count; i++)
			{
				if (base.InternalChildren[i] == visual)
				{
					uIElement = base.InternalChildren[i];
					num = this.getLeftEdge(base.InternalChildren[i]);
					break;
				}
			}
			if (uIElement != null)
			{
				if (uIElement == base.InternalChildren[0])
				{
					num = 0.0;
				}
				else
				{
					if (uIElement == base.InternalChildren[base.InternalChildren.Count - 1])
					{
						num = this.ExtentWidth - this.Viewport.Width;
					}
					else
					{
						num = ScrollableTabPanel.CalculateNewScrollOffset(this.HorizontalOffset, this.HorizontalOffset + this.Viewport.Width, num, num + uIElement.DesiredSize.Width);
					}
				}
				this.SetHorizontalOffset(num);
				rectangle = new System.Windows.Rect(this.HorizontalOffset, 0.0, uIElement.DesiredSize.Width, this.Viewport.Height);
			}
			return rectangle;
		}
		public void MouseWheelDown()
		{
		}
		public void MouseWheelLeft()
		{
		}
		public void MouseWheelRight()
		{
		}
		public void MouseWheelUp()
		{
		}
		public void PageDown()
		{
		}
		public void PageLeft()
		{
		}
		public void PageRight()
		{
		}
		public void PageUp()
		{
		}
		public void SetHorizontalOffset(double offset)
		{
			this.RemoveOpacityMasks();
			this.HorizontalOffset = System.Math.Max(0.0, System.Math.Min(this.ExtentWidth - this.Viewport.Width, System.Math.Max(0.0, offset)));
			if (this.ScrollOwner != null)
			{
				this.ScrollOwner.InvalidateScrollInfo();
			}
			DoubleAnimation doubleAnimation = new DoubleAnimation(this._ttScrollTransform.X, -this.HorizontalOffset, new Duration(this.AnimationTimeSpan), FillBehavior.HoldEnd);
			doubleAnimation.AccelerationRatio = 0.5;
			doubleAnimation.DecelerationRatio = 0.5;
			doubleAnimation.Completed += new System.EventHandler(this.daScrollAnimation_Completed);
			this._ttScrollTransform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation, HandoffBehavior.Compose);
			base.InvalidateMeasure();
		}
		public void SetVerticalOffset(double offset)
		{
		}
		private void NotifyPropertyChanged(string strPropertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(strPropertyName));
			}
		}
		private void ScrollOwner_Loaded(object sender, RoutedEventArgs e)
		{
			this.UpdateOpacityMasks();
		}
		private void daScrollAnimation_Completed(object sender, System.EventArgs e)
		{
			this.UpdateOpacityMasks();
			foreach (UIElement uIElement in base.InternalChildren)
			{
				uIElement.InvalidateArrange();
			}
		}
		private void ScrollableTabPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateOpacityMasks();
		}
	}
}
