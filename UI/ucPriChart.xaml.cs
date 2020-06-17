using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

namespace PrinterCenter
{
	/// <summary>
	/// Interaction logic for ucHistogram.xaml
	/// </summary>
	public partial class ucPriChart : UserControl
	{	
		double _minXdata = 0;
		double _maxXdata = 20;

		double _minYdata = -50;
		public double MinYdata
		{
			get { return _minYdata; }
			set { _minYdata = value; }
		}

		double _maxYdata = 50;
		public double MaxYdata
		{
			get { return _maxYdata; }
			set { _maxYdata = value; }
		}

		double _minXpixel;
		double _maxXpixel;
		double _minYpixel;
		double _maxYpixel;

		double _xStep = 20;
		public double xStep
		{
			get { return _xStep; }
			set { _xStep = value; }
		}

		double _yStep = 25;
		public double yStep
		{
			get { return _yStep; }
			set { _yStep = value; }
		}

		int _samplingCount = 10;
		public int SamplingCount
		{
			get { return _samplingCount; }
			set { _samplingCount = value; }
		}

		int _gradeCount = 20;
		public int GradeCount
		{
			get { return _gradeCount; }
			set { _gradeCount = value; }
		}

	
		public ucPriChart()
		{
			InitializeComponent();
		}
	
		double Data_to_X(double x)
		{
			if (x == _minXdata)
				return _minXpixel;
			else if (x == _maxXdata)
				return _maxXpixel;
			else
				return _minXpixel + (x - _minXdata) * (_maxXpixel - _minXpixel) / (_maxXdata - _minXdata);
		}
		double Data_to_Y(double y)//PenNote: flip on Y
		{
			if (y == _minYdata)
				return _maxYpixel;
			else if (y == _maxYdata)
				return _minYpixel;
			else
				return _maxYpixel - (y - _minYdata) * (_maxYpixel - _minYpixel) / (_maxYdata - _minYdata);
		}
	
		private void PlotAxises()
		{
			Brush brush = Brushes.Green;
			double thickness = 2;
			double lineWidth = 2;

			// x axis
			Line lineXAxis = new Line();
			lineXAxis.Stroke = brush;
			lineXAxis.X1 = _minXpixel - lineWidth / 2;
			lineXAxis.Y1 = _maxYpixel;
			lineXAxis.X2 = _maxXpixel;
			lineXAxis.Y2 = _maxYpixel;
			lineXAxis.StrokeThickness = thickness;
			_cvCoor.Children.Add(lineXAxis);

			// y axis
			Line lineYAxis = new Line();
			lineYAxis.Stroke = brush;
			lineYAxis.X1 = _minXpixel - lineWidth / 2;
			lineYAxis.Y1 = _maxYpixel;
			lineYAxis.X2 = _minXpixel - lineWidth / 2;
			lineYAxis.Y2 = _minYpixel;
			lineYAxis.StrokeThickness = thickness;
			_cvCoor.Children.Add(lineYAxis);

			Line lineTick;
			TextBlock textBlock;
			double x, X;

			for (x = _minXdata; x <= _maxXdata; x += _xStep)
			{
				// x: tick mark
				X = Data_to_X(x);
				lineTick = new Line();
				lineTick.Stroke = brush;
				lineTick.X1 = X;
				lineTick.Y1 = _maxYpixel;
				lineTick.X2 = X;
				lineTick.Y2 = _maxYpixel + 6;
				lineTick.StrokeThickness = thickness;

				if (x != _minXdata)
				{
					_cvCoor.Children.Add(lineTick);
				}

				// x: label
				textBlock = GetAxisTextBlock();
				if (x == 0f)
					textBlock.Text = "0";
				else
					textBlock.Text = String.Format("{0}", x);

				Canvas.SetLeft(textBlock, X - 8);

				Canvas.SetTop(textBlock, _maxYpixel + 10);		

				_cvCoor.Children.Add(textBlock);
			}

			double y, Y;
			
			for (y = _minYdata; y <= _maxYdata; y += _yStep)	
			{
				// y: tick mark
				Y = Data_to_Y(y);
				lineTick = new Line();
				lineTick.Stroke = brush;
				lineTick.X1 = _minXpixel - lineWidth / 2;
				lineTick.Y1 = Y;
				lineTick.X2 = _minXpixel - 6 - lineWidth / 2;
				lineTick.Y2 = Y;
				lineTick.StrokeThickness = thickness;

				if (y > _minYdata)  //skip buttom y tick
					_cvCoor.Children.Add(lineTick);

				textBlock = GetAxisTextBlock();
				
				textBlock.Text = y.ToString();
				//textBlock.Text = String.Format("{0:0}", y);		
	
				textBlock.TextAlignment = TextAlignment.Right;
				textBlock.Width = 30;
				Canvas.SetLeft(textBlock, _minXpixel - 43);
				Canvas.SetTop(textBlock, Y - 7);

				_cvCoor.Children.Add(textBlock);
			}
	
		}


		TextBlock GetAxisTextBlock()
		{
			TextBlock tb = new TextBlock();
			tb.FontFamily = new FontFamily("Arial");
			tb.FontSize = 14;
			tb.FontWeight = FontWeights.Normal;
			tb.Foreground = Brushes.Black;
			tb.Opacity = 0.7;
			return tb;
		}

		public void Plot(List<KeyValuePair<int, double>> valueList)
		{
			_maxXdata = ((valueList.Count - 2) <= 0) ? GradeCount : ((valueList.Count - 2) / GradeCount + 1) * GradeCount;
			_xStep = _maxXdata / _samplingCount;

			_cvCoor.Children.Clear();
		
			_minXpixel = 0;
			_maxXpixel = _cvCoor.Width;
			_minYpixel = 0;
			_maxYpixel = _cvCoor.Height;

			_cvCoor.Background = _cvHost.Background;

			PlotAxises();

			PlotValue(valueList);

			ConnectLine(valueList);
		}

		private void PlotValue(List<KeyValuePair<int, double>> data)
		{		
			foreach (KeyValuePair<int, double> item in data)
			{
				Ellipse shape = new Ellipse();
				shape.Width = 3;
				shape.Height = 3;

				Point pt = new Point();
				pt.X = Data_to_X(item.Key);
				pt.Y = Data_to_Y(item.Value);

				shape.Fill = Brushes.Blue;
				_cvCoor.Children.Add(shape);
				Canvas.SetLeft(shape, pt.X - shape.Width / 2);
				Canvas.SetTop(shape, pt.Y - shape.Height / 2);
			}		
		}

		private void ConnectLine(List<KeyValuePair<int, double>> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				if (i == data.Count - 1)
					break;

				KeyValuePair<int, double> item1 = data[i];
				KeyValuePair<int, double> item2 = data[i+1];
				Line line = new Line();
				line.Stroke = Brushes.Red;
				line.StrokeThickness = 0.75;
				line.X1 = Data_to_X(item1.Key);
				line.Y1 = Data_to_Y(item1.Value);
				line.X2 = Data_to_X(item2.Key);
				line.Y2 = Data_to_Y(item2.Value);
				_cvCoor.Children.Add(line);
			}
		}

		public void SaveToPng(string pathname)
		{
			Dispatcher.BeginInvoke(new SaveToPngSubDelegate(SaveToPngSub), 
				                                            System.Windows.Threading.DispatcherPriority.Loaded, 
															pathname);

		}
		delegate void SaveToPngSubDelegate(string pathname);
		private void SaveToPngSub(string pathname)
		{
			try
			{
				if (!Directory.Exists(System.IO.Path.GetDirectoryName(pathname)))
					Directory.CreateDirectory(System.IO.Path.GetDirectoryName(pathname));

				FileStream stream = new FileStream(pathname, FileMode.Create);

				RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
				rtb.Render(this);

				PngBitmapEncoder png = new PngBitmapEncoder();
				png.Interlace = System.Windows.Media.Imaging.PngInterlaceOption.On;
				png.Frames.Add(BitmapFrame.Create(rtb));
				png.Save(stream);

				stream.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception Catched in SaveToPng({0}), \n Reason={1}", pathname, ex.Message);
			}
		}
	


	}
}
