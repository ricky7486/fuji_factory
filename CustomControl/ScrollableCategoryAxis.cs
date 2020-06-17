// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
using System.Windows.Controls.Primitives;
using System.Windows.Controls.DataVisualization;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace PrinterCenter.CustomControl
{
    [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "AxisLabelStyle", StyleTargetType = typeof(AxisLabel))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [TemplatePart(Name = AxisGridName, Type = typeof(Grid))]
    [TemplatePart(Name = AxisTitleName, Type = typeof(Title))]
    public class ScrollableCategoryAxis : DisplayAxis, ICategoryAxis
    {
        #region public CategorySortOrder SortOrder
        /// <summary>
        /// Gets or sets the sort order used for the categories.
        /// </summary>
        public CategorySortOrder SortOrder
        {
            get { return (CategorySortOrder)GetValue(SortOrderProperty); }
            set { SetValue(SortOrderProperty, value); }
        }

        /// <summary>
        /// Identifies the SortOrder dependency property.
        /// </summary>
        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.Register(
                "SortOrder",
                typeof(CategorySortOrder),
                typeof(ScrollableCategoryAxis),
                new PropertyMetadata(CategorySortOrder.None, OnSortOrderPropertyChanged));

        /// <summary>
        /// SortOrderProperty property changed handler.
        /// </summary>
        /// <param name="d">CategoryAxis that changed its SortOrder.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSortOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollableCategoryAxis source = (ScrollableCategoryAxis)d;
            source.OnSortOrderPropertyChanged();
        }

        /// <summary>
        /// SortOrderProperty property changed handler.
        /// </summary>
        private void OnSortOrderPropertyChanged()
        {
            Invalidate();
        }
        #endregion public CategorySortOrder SortOrder

        /// <summary>
        /// ScrollChanged event.
        /// </summary>
        public event EventHandler ScrollChanged;

        /// <summary>
        /// Main grid.
        /// </summary>
        protected Grid axisGrid;

        /// <summary>
        /// ScrollViewer that wraps the grid containing the category labels.
        /// </summary>
        protected ScrollViewer axisScrollViewer;

        /// <summary>
        /// Grid containing the category labels.
        /// </summary>
        protected Grid categoryLabelsGrid;

        /// <summary>
        /// Actual size of the grid containing the category labels.
        /// </summary>
        protected Size gridSize;

        /// <summary>
        /// Gets or sets a list of categories to display.
        /// </summary>
        protected IList<object> Categories { get; set; }

        /// <summary>
        /// Gets or sets the grid line coordinates to display.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "This is the expected capitalization.")]
        private IList<UnitValue> GridLineCoordinatesToDisplay { get; set; }

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScrollableCategoryAxis()
        {
            this.Categories = new List<object>();
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ScrollableCategoryAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollableCategoryAxis), new FrameworkPropertyMetadata(typeof(ScrollableCategoryAxis)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves template parts and configures layout.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            axisGrid = GetTemplateChild(AxisGridName) as Grid;
            if (axisGrid != null)
            {
                axisGrid.SetValue(Grid.IsSharedSizeScopeProperty, true);

                categoryLabelsGrid = new Grid();
                categoryLabelsGrid.Loaded += CategoryLabelsGrid_Loaded;

                axisScrollViewer = new ScrollViewer();
                axisScrollViewer.Content = categoryLabelsGrid;
                axisScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                axisScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                axisScrollViewer.ScrollChanged += AxisScrollViewer_ScrollChanged;

                axisGrid.Children.Add(axisScrollViewer);
            }
        }

        /// <summary>
        /// Stores the size of the Grid containing the category labels.
        /// </summary>
        /// <param name="sender">Grid.</param>
        /// <param name="e">Ignored.</param>
        private void CategoryLabelsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            gridSize = new Size(categoryLabelsGrid.ActualWidth, categoryLabelsGrid.ActualHeight);

            // Force redraw
            Invalidate();
        }

        /// <summary>        
        /// Triggers a ScrollChanged event.
        /// </summary>
        /// <param name="sender">ScrollViewer.</param>
        /// <param name="e">Ignored.</param>
        private void AxisScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            EventHandler eh = ScrollChanged;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }
        }

        /// <summary>
        /// Returns the major axis grid line coordinates.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of the major grid line coordinates.</returns>        
        protected override IEnumerable<UnitValue> GetMajorGridLineCoordinates(Size availableSize)
        {
            return GridLineCoordinatesToDisplay;
        }

        /// <summary>
        /// Renders the axis labels, tick marks, and other visual elements.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        protected override void Render(Size availableSize)
        {
            if (categoryLabelsGrid != null)
            {
                categoryLabelsGrid.Children.Clear();
                categoryLabelsGrid.ColumnDefinitions.Clear();

                foreach (object category in Categories)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.SharedSizeGroup = "Group";
                    categoryLabelsGrid.ColumnDefinitions.Add(columnDefinition);

                    Label categoryLabel = new Label();
                    categoryLabel.Content = category;
                    categoryLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    categoryLabel.SetValue(Grid.ColumnProperty, Categories.IndexOf(category));

                    categoryLabelsGrid.Children.Add(categoryLabel);
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether a value can be plotted on the
        /// axis.
        /// </summary>
        /// <param name="value">A value which may or may not be able to be
        /// plotted.</param>
        /// <returns>A value indicating whether a value can be plotted on the
        /// axis.</returns>
        public override bool CanPlot(object value)
        {
            return true;
        }

        /// <summary>
        /// The plot area coordinate of a value.
        /// </summary>
        /// <param name="value">The value for which to retrieve the plot area
        /// coordinate.</param>
        /// <returns>The plot area coordinate.</returns>
        public override UnitValue GetPlotAreaCoordinate(object value)
        {
            Range<UnitValue> range = GetPlotAreaCoordinateRange(value);
            if (range.HasData)
            {
                double minimum = range.Minimum.Value;
                double maximum = range.Maximum.Value;
                return new UnitValue(((maximum - minimum) / 2.0) + minimum, range.Minimum.Unit);
            }
            else
            {
                return UnitValue.NaN();
            }
        }

        /// <summary>
        /// Updates categories when a series is registered.
        /// </summary>
        /// <param name="series">The series to be registered.</param>
        protected override void OnObjectRegistered(IAxisListener series)
        {
            base.OnObjectRegistered(series);
            if (series is IDataProvider)
            {
                UpdateCategories();
            }
        }

        /// <summary>
        /// Updates categories when a series is unregistered.
        /// </summary>
        /// <param name="series">The series to be unregistered.</param>
        protected override void OnObjectUnregistered(IAxisListener series)
        {
            base.OnObjectUnregistered(series);
            if (series is IDataProvider)
            {
                UpdateCategories();
            }
        }

        /// <summary>
        /// Updates the list of categories.
        /// </summary>
        private void UpdateCategories()
        {
            IEnumerable<object> categories =
                this.RegisteredListeners
                .OfType<IDataProvider>()
                .SelectMany(infoProvider => infoProvider.GetData(this))
                .Distinct();

            if (SortOrder == CategorySortOrder.Ascending)
            {
                categories = categories.OrderBy(category => category);
            }
            else if (SortOrder == CategorySortOrder.Descending)
            {
                categories = categories.OrderByDescending(category => category);
            }

            this.Categories = categories.ToList();

            Invalidate();
        }

        /// <summary>
        /// Gets the actual length.
        /// </summary>
        protected new double ActualLength
        {
            get
            {
                if (gridSize != null)
                {
                    return GetLength(gridSize);
                }

                return base.ActualLength;
            }
        }

        #endregion

        #region ICategoryAxis Members

        /// <summary>
        /// Returns the category at a given coordinate.
        /// </summary>
        /// <param name="position">The plot area position.</param>
        /// <returns>The category at the given plot area position.</returns>        
        public object GetCategoryAtPosition(UnitValue position)
        {
            if (this.ActualLength == 0.0 || this.Categories.Count == 0)
            {
                return null;
            }
            if (position.Unit == Unit.Pixels)
            {
                double coordinate = position.Value;
                int index = (int)Math.Floor(coordinate / (this.ActualLength / this.Categories.Count));
                if (index >= 0 && index < this.Categories.Count)
                {
                    if (Orientation == AxisOrientation.X)
                    {
                        return this.Categories[index];
                    }
                    else
                    {
                        return this.Categories[(this.Categories.Count - 1) - index];
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return null;
        }

        /// <summary>
        /// Returns range of coordinates for a given category.
        /// </summary>
        /// <param name="category">The category to return the range for.</param>
        /// <returns>The range of coordinates corresponding to the category.
        /// </returns>
        public Range<UnitValue> GetPlotAreaCoordinateRange(object category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("category");
            }
            int index = Categories.IndexOf(category);
            if (index == -1)
            {
                return new Range<UnitValue>();
            }

            if (Orientation == AxisOrientation.X || Orientation == AxisOrientation.Y)
            {
                double offset = 0.0;
                if (axisScrollViewer != null)
                {
                    offset = Orientation == AxisOrientation.X ? axisScrollViewer.HorizontalOffset :
                        axisScrollViewer.VerticalOffset;
                }

                double maximumLength = Math.Max(ActualLength - 1, 0);
                double lower = (index * maximumLength) / Categories.Count - offset;
                double upper = ((index + 1) * maximumLength) / Categories.Count - offset;

                if (Orientation == AxisOrientation.X)
                {
                    return new Range<UnitValue>(new UnitValue(lower, Unit.Pixels), new UnitValue(upper, Unit.Pixels));
                }
                else if (Orientation == AxisOrientation.Y)
                {
                    return new Range<UnitValue>(new UnitValue(maximumLength - upper, Unit.Pixels), new UnitValue(maximumLength - lower, Unit.Pixels));
                }
            }
            else
            {
                double startingAngle = 270.0;
                double angleOffset = 360 / this.Categories.Count;
                double halfAngleOffset = angleOffset / 2.0;
                int categoryIndex = this.Categories.IndexOf(category);
                double angle = startingAngle + (categoryIndex * angleOffset);

                return new Range<UnitValue>(new UnitValue(angle - halfAngleOffset, Unit.Degrees), new UnitValue(angle + halfAngleOffset, Unit.Degrees));
            }

            return new Range<UnitValue>();
        }

        #endregion

        #region IDataConsumer Members

        /// <summary>
        /// Updates the categories in response to an update from a registered
        /// axis data provider.
        /// </summary>
        /// <param name="dataProvider">The category axis information
        /// provider.</param>
        /// <param name="data">A sequence of categories.</param>
        public void DataChanged(IDataProvider dataProvider, IEnumerable<object> data)
        {
            UpdateCategories();
        }

        #endregion
    }
}
