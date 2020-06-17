using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrinterCenter.CustomControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PrinterCenter"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PrinterCenter;assembly=PrinterCenter"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:StepTabControl/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_AnimationHost", Type = typeof(Border))]
    [TemplatePart(Name = "PART_GridRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Header", Type = typeof(TabPanel))]
    public class StepTabControl : TabControl
    {
        static Border brHost = new Border();
        static Grid gdRoot = new Grid();
        static TabPanel tpHeader = new TabPanel();
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            brHost = this.GetTemplateChild("PART_AnimationHost") as Border;
            gdRoot = this.GetTemplateChild("PART_GridRoot") as Grid;
            tpHeader = this.GetTemplateChild("PART_Header") as TabPanel;
        }
        public static readonly DependencyProperty EnableSlideAnimationProperty;
        public static readonly DependencyProperty EnableFadeAnimationProperty;
        public static readonly DependencyProperty TabBackgroundProperty;
        public static readonly DependencyProperty SelectedTabBackgroundProperty;
        public static readonly DependencyProperty SelectedTabTextColorProperty;
        public static readonly DependencyProperty TabTextColorProperty;
        public static readonly DependencyProperty TabMarginProperty;
        public static readonly DependencyProperty TabHeightProperty;
        public static readonly DependencyProperty TabWidthProperty;
        public bool EnableSlideAnimation
        {
            get
            {
                return (bool)GetValue(EnableSlideAnimationProperty);
            }
            set
            {
                SetValue(EnableSlideAnimationProperty, value);
            }
        }
        public bool EnableFadeAnimation
        {
            get
            {
                return (bool)GetValue(EnableFadeAnimationProperty);
            }
            set
            {
                SetValue(EnableFadeAnimationProperty, value);
            }
        }
        public Brush TabBackground
        {
            get
            {
                return (Brush)GetValue(TabBackgroundProperty);
            }
            set
            {
                SetValue(TabBackgroundProperty, value);
            }
        }
        public Brush SelectedTabBackground
        {
            get
            {
                return (Brush)GetValue(SelectedTabBackgroundProperty);
            }
            set
            {
                SetValue(SelectedTabBackgroundProperty, value);
            }
        }
        public Brush SelectedTabTextColor
        {
            get
            {
                return (Brush)GetValue(SelectedTabTextColorProperty);
            }
            set
            {
                SetValue(SelectedTabTextColorProperty, value);
            }
        }
        public Brush TabTextColor
        {
            get
            {
                return (Brush)GetValue(TabTextColorProperty);
            }
            set
            {
                SetValue(TabTextColorProperty, value);
            }
        }
        public int TabHeight
        {
            get
            {
                return (int)GetValue(TabHeightProperty);
            }
            set
            {
                SetValue(TabHeightProperty, value);
            }
        }
        public int TabWidth
        {
            get
            {
                return (int)GetValue(TabWidthProperty);
            }
            set
            {
                SetValue(TabWidthProperty, value);
            }
        }

        public Thickness TabMargin
        {
            get
            {
                return (Thickness)GetValue(TabMarginProperty);
            }
            set
            {
                SetValue(TabMarginProperty, value);
            }
        }
        static StepTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepTabControl), new FrameworkPropertyMetadata(typeof(StepTabControl)));
            EnableFadeAnimationProperty = DependencyProperty.Register("EnableFadeAnimation", typeof(bool), typeof(StepTabControl), new PropertyMetadata(true));
            EnableSlideAnimationProperty = DependencyProperty.Register("EnableSlideAnimation", typeof(bool), typeof(StepTabControl), new PropertyMetadata(true));
            TabBackgroundProperty = DependencyProperty.Register("TabBackground", typeof(Brush), typeof(StepTabControl), new PropertyMetadata(Brushes.Gray));

            SelectedTabBackgroundProperty = DependencyProperty.Register("SelectedTabBackground", typeof(Brush), typeof(StepTabControl), new PropertyMetadata(Brushes.Yellow));
            TabTextColorProperty = DependencyProperty.Register("TabTextColor", typeof(Brush), typeof(StepTabControl), new PropertyMetadata(Brushes.White));
            SelectedTabTextColorProperty = DependencyProperty.Register("SelectedTabTextColor", typeof(Brush), typeof(StepTabControl), new PropertyMetadata(Brushes.Red));

            TabMarginProperty = DependencyProperty.Register("TabMargin", typeof(Thickness), typeof(StepTabControl), new PropertyMetadata(new Thickness(2)));

            TabHeightProperty = DependencyProperty.Register("TabHeight", typeof(int), typeof(StepTabControl), new PropertyMetadata(60));
            TabWidthProperty = DependencyProperty.Register("TabWidth", typeof(int), typeof(StepTabControl), new PropertyMetadata(100));

        }


        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }



        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {

            base.OnSelectionChanged(e);
            curr = this.SelectedIndex;



            if (prev != curr)
            {

                if (prev > curr)
                {

                    if (EnableFadeAnimation && EnableSlideAnimation)
                    {
                        Storyboard _sb = this.FindResource("SlideAndFadeRightIn") as Storyboard;
                        if (_sb.IsSealed)
                            _sb = _sb.Clone();
                   
                        Storyboard.SetTarget(_sb, brHost);
                        _sb.Begin();
                    }
                    else if (EnableFadeAnimation && !EnableSlideAnimation)
                    {
                        Storyboard _sb = this.FindResource("Fade") as Storyboard;
                        if (_sb.IsSealed)
                            _sb = _sb.Clone();
                     
                        Storyboard.SetTarget(_sb, brHost);
                        _sb.Begin();
                    }
                    else if (!EnableFadeAnimation && EnableSlideAnimation)
                    {
                        Storyboard _sb = this.FindResource("SlideRight") as Storyboard;
                        if (_sb.IsSealed)
                            _sb = _sb.Clone();
              
                        Storyboard.SetTarget(_sb, brHost);
                        _sb.Begin();
                    }

                }
                else
                {

                    if (EnableFadeAnimation && EnableSlideAnimation)
                    {
                        try
                        {
                            Storyboard _sb = this.FindResource("SlideAndFadeLeftIn") as Storyboard;
                            if (_sb.IsSealed)
                                _sb = _sb.Clone();

                            Storyboard.SetTarget(_sb, brHost);
                            _sb.Begin();
                        }catch{ }
                    }
                    else if (EnableFadeAnimation && !EnableSlideAnimation)
                    {
                        try
                        {
                            Storyboard _sb = this.FindResource("Fade") as Storyboard;
                            if (_sb.IsSealed)
                                _sb = _sb.Clone();

                            Storyboard.SetTarget(_sb, brHost);
                            _sb.Begin();
                        }
                        catch { }
                        
                    }
                    else if (!EnableFadeAnimation && EnableSlideAnimation)
                    {
                        try
                        {
                            Storyboard _sb = this.FindResource("SlideLeft") as Storyboard;
                            if (_sb.IsSealed)
                                _sb = _sb.Clone();

                            Storyboard.SetTarget(_sb, brHost);
                            _sb.Begin();
                        }
                        catch { }
                        
                    }

                }

            }


            prev = curr;
        }
        int prev = -1, curr = -1;


    }
}
