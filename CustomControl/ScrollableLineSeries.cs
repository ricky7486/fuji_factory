// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//Rex Modify it for LineSeries
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

using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Globalization;
using System.Collections;

namespace PrinterCenter.CustomControl
{

    public class ScrollableLineSeries : LineSeries /*ColumnSeries*/
    {
        //static ScrollableLineSeries()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollableLineSeries), new FrameworkPropertyMetadata(typeof(ScrollableLineSeries)));
        //}

        private Panel plotArea;

        /// <summary>
        /// Get the plot area.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            plotArea = GetTemplateChild(DataPointSeries.PlotAreaName) as Panel;
        }

        /// <summary>
        /// Acquire a horizontal category axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetAxes(
                firstDataPoint,
                (axis) => axis.Orientation == AxisOrientation.X,
                () =>
                {
                    ScrollableCategoryAxis categoryAxis = new ScrollableCategoryAxis { Orientation = AxisOrientation.X };
                    categoryAxis.ScrollChanged += ScrollableCategoryAxis_ScrollChanged;
                    return categoryAxis;
                },
                (axis) =>
                {
                    IRangeAxis rangeAxis = axis as IRangeAxis;
                    return rangeAxis != null && rangeAxis.Origin != null && axis.Orientation == AxisOrientation.Y;
                },
                () =>
                {
                    IRangeAxis rangeAxis = CreateRangeAxisFromData(firstDataPoint.DependentValue);
                    rangeAxis.Orientation = AxisOrientation.Y;
                    if (rangeAxis == null || rangeAxis.Origin == null)
                    {
                        throw new InvalidOperationException("No suitable axis is available for plotting the dependent value.");
                    }
                    DisplayAxis axis = rangeAxis as DisplayAxis;
                    if (axis != null)
                    {
                        axis.ShowGridLines = true;
                    }
                    return rangeAxis;
                });
        }

        /// <summary>
        /// Updates the data points when the scrollbar has been moved.
        /// </summary>
        /// <param name="sender">ScrollableCategoryAxis object.</param>
        /// <param name="e">Ignored.</param>
        private void ScrollableCategoryAxis_ScrollChanged(object sender, EventArgs e)
        {
            UpdateDataPoints(ActiveDataPoints);
        }

        /// <summary>
        /// Updates each point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        /*
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (SeriesHost == null || plotArea == null)
            {
                return;
            }

            object category = dataPoint.ActualIndependentValue ?? (this.ActiveDataPoints.IndexOf(dataPoint) + 1);
            ICategoryAxis categoryAxis = ActualIndependentAxis as ScrollableCategoryAxis;
            Range<UnitValue> coordinateRange = categoryAxis.GetPlotAreaCoordinateRange(category);

            if (!coordinateRange.HasData)
            {
                return;
            }
            else if (coordinateRange.Maximum.Unit != Unit.Pixels || coordinateRange.Minimum.Unit != Unit.Pixels)
            {
                throw new InvalidOperationException("This series does not support radial axes.");
            }

            double minimum = (double)coordinateRange.Minimum.Value;
            double maximum = (double)coordinateRange.Maximum.Value;

            double plotAreaHeight = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Range.Maximum).Value;
            //IEnumerable<ColumnSeries> columnSeries = SeriesHost.Series.OfType<ColumnSeries>().Where(series => series.ActualIndependentAxis == ActualIndependentAxis);
            IEnumerable<LineSeries> lineSeries = SeriesHost.Series.OfType<LineSeries>().Where(series => series.ActualIndependentAxis == ActualIndependentAxis);

            //int numberOfSeries = columnSeries.Count();
            int numberOfSeries = lineSeries.Count();
            double coordinateRangeWidth = (maximum - minimum);
            double segmentWidth = coordinateRangeWidth * 0.8;
            double columnWidth = segmentWidth / numberOfSeries;
            //int seriesIndex = columnSeries.IndexOf(this);
            int seriesIndex = lineSeries.IndexOf(this);

            double dataPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ValueHelper.ToDouble(dataPoint.ActualDependentValue)).Value;
            double zeroPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Origin).Value;

            double offset = seriesIndex * Math.Round(columnWidth) + coordinateRangeWidth * 0.1;
            double dataPointX = minimum + offset;

            //if (GetIsDataPointGrouped(category))
            //{
            //    // Multiple DataPoints share this category; offset and overlap them appropriately
            //    IGrouping<object, DataPoint> categoryGrouping = GetDataPointGroup(category);
            //    int index = categoryGrouping.IndexOf(dataPoint);
            //    dataPointX += (index * (columnWidth * 0.2)) / (categoryGrouping.Count() - 1);
            //    columnWidth *= 0.8;
            //    Canvas.SetZIndex(dataPoint, -index);
            //}
            //LineAreaBaseSeries
            if (ValueHelper.CanGraph(dataPointY) && ValueHelper.CanGraph(dataPointX) && ValueHelper.CanGraph(zeroPointY))
            {
                dataPoint.Visibility = Visibility.Visible;

                double left = Math.Round(dataPointX);
                double width = Math.Round(columnWidth);

                double top = Math.Round(plotAreaHeight - Math.Max(dataPointY, zeroPointY) + 0.5);
                double bottom = Math.Round(plotAreaHeight - Math.Min(dataPointY, zeroPointY) + 0.5);
                double height = bottom - top + 1;

                Canvas.SetLeft(dataPoint, left);
                Canvas.SetTop(dataPoint, top);
                dataPoint.Width = width;
                dataPoint.Height = height;
            }
            else
            {
                dataPoint.Visibility = Visibility.Collapsed;
            }
        }
        */
    }
    public class ScrollableColumnSeries : ColumnSeries
    {
        private Panel plotArea;

        /// <summary>
        /// Get the plot area.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            plotArea = GetTemplateChild(DataPointSeries.PlotAreaName) as Panel;
        }

        /// <summary>
        /// Acquire a horizontal category axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetAxes(
                firstDataPoint,
                (axis) => axis.Orientation == AxisOrientation.X,
                () =>
                {
                    ScrollableCategoryAxis categoryAxis = new ScrollableCategoryAxis { Orientation = AxisOrientation.X };
                    categoryAxis.ScrollChanged += ScrollableCategoryAxis_ScrollChanged;
                    return categoryAxis;
                },
                (axis) =>
                {
                    IRangeAxis rangeAxis = axis as IRangeAxis;
                    return rangeAxis != null && rangeAxis.Origin != null && axis.Orientation == AxisOrientation.Y;
                },
                () =>
                {
                    IRangeAxis rangeAxis = CreateRangeAxisFromData(firstDataPoint.DependentValue);
                    rangeAxis.Orientation = AxisOrientation.Y;
                    if (rangeAxis == null || rangeAxis.Origin == null)
                    {
                        throw new InvalidOperationException("No suitable axis is available for plotting the dependent value.");
                    }
                    DisplayAxis axis = rangeAxis as DisplayAxis;
                    if (axis != null)
                    {
                        axis.ShowGridLines = true;
                    }
                    return rangeAxis;
                });
        }

        /// <summary>
        /// Updates the data points when the scrollbar has been moved.
        /// </summary>
        /// <param name="sender">ScrollableCategoryAxis object.</param>
        /// <param name="e">Ignored.</param>
        private void ScrollableCategoryAxis_ScrollChanged(object sender, EventArgs e)
        {
            UpdateDataPoints(ActiveDataPoints);
        }

        /// <summary>
        /// Updates each point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (SeriesHost == null || plotArea == null)
            {
                return;
            }

            object category = dataPoint.ActualIndependentValue ?? (this.ActiveDataPoints.IndexOf(dataPoint) + 1);
            ICategoryAxis categoryAxis = ActualIndependentAxis as ScrollableCategoryAxis;
            Range<UnitValue> coordinateRange = categoryAxis.GetPlotAreaCoordinateRange(category);

            if (!coordinateRange.HasData)
            {
                return;
            }
            else if (coordinateRange.Maximum.Unit != Unit.Pixels || coordinateRange.Minimum.Unit != Unit.Pixels)
            {
                throw new InvalidOperationException("This series does not support radial axes.");
            }

            double minimum = (double)coordinateRange.Minimum.Value;
            double maximum = (double)coordinateRange.Maximum.Value;

            double plotAreaHeight = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Range.Maximum).Value;
            IEnumerable<ColumnSeries> columnSeries = SeriesHost.Series.OfType<ColumnSeries>().Where(series => series.ActualIndependentAxis == ActualIndependentAxis);
            int numberOfSeries = columnSeries.Count();
            double coordinateRangeWidth = (maximum - minimum);
            double segmentWidth = coordinateRangeWidth * 0.8;
            double columnWidth = segmentWidth / numberOfSeries;
            int seriesIndex = columnSeries.IndexOf(this);

            double dataPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ValueHelper.ToDouble(dataPoint.ActualDependentValue)).Value;
            double zeroPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Origin).Value;

            double offset = seriesIndex * Math.Round(columnWidth) + coordinateRangeWidth * 0.1;
            double dataPointX = minimum + offset;

            if (GetIsDataPointGrouped(category))
            {
                // Multiple DataPoints share this category; offset and overlap them appropriately
                IGrouping<object, DataPoint> categoryGrouping = GetDataPointGroup(category);
                int index = categoryGrouping.IndexOf(dataPoint);
                dataPointX += (index * (columnWidth * 0.2)) / (categoryGrouping.Count() - 1);
                columnWidth *= 0.8;
                Canvas.SetZIndex(dataPoint, -index);
            }
          
            if (ValueHelper.CanGraph(dataPointY) && ValueHelper.CanGraph(dataPointX) && ValueHelper.CanGraph(zeroPointY))
            {
                dataPoint.Visibility = Visibility.Visible;

                double left = Math.Round(dataPointX);
                double width = Math.Round(columnWidth);

                double top = Math.Round(plotAreaHeight - Math.Max(dataPointY, zeroPointY) + 0.5);
                double bottom = Math.Round(plotAreaHeight - Math.Min(dataPointY, zeroPointY) + 0.5);
                double height = bottom - top + 1;

                Canvas.SetLeft(dataPoint, left);
                Canvas.SetTop(dataPoint, top);
                dataPoint.Width = width;
                dataPoint.Height = height;
            }
            else
            {
                dataPoint.Visibility = Visibility.Collapsed;
            }
        }
    }

    public static class EnumerableFunctions
    {
        /// <summary>
        /// Returns the index of an item in a sequence.
        /// </summary>
        /// <param name="that">The sequence.</param>
        /// <param name="value">The item to search for.</param>
        /// <returns>The index of the item or -1 if not found.</returns>
        public static int IndexOf(this IEnumerable that, object value)
        {
            int index = 0;
            foreach (object item in that)
            {
                if (object.ReferenceEquals(value, item) || value.Equals(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }

    public static class ValueHelper
    {
        /// <summary>
        /// Returns a value indicating whether this value can be graphed on a 
        /// linear axis.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>A value indicating whether this value can be graphed on a 
        /// linear axis.</returns>
        public static bool CanGraph(double value)
        {
            return !double.IsNaN(value) && !double.IsNegativeInfinity(value) && !double.IsPositiveInfinity(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// Converts an object into a double.
        /// </summary>
        /// <param name="value">The value to convert to a double.</param>
        /// <returns>The converted double value.</returns>
        public static double ToDouble(object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
    }
}
