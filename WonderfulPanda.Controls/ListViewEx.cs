using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace WonderfulPanda.Controls
{
    [TemplatePart(Name="PART_ScrollViewer", Type=typeof(ScrollViewer))]
    public class ListViewEx : ListBox
    {
        private ScrollViewer _scrollViewer = null;

        static ListViewEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListViewEx), new FrameworkPropertyMetadata(typeof(ListViewEx)));
        }

        public class HorizontalLayoutInfo
        {
            public double FrozenWidth { get; private set; }
            public Thickness FrozenColumnsMargin { get; private set; }
            public Thickness ScrollableColumnsMargin { get; private set; }
            public Thickness FrozenHeadersMargin { get; private set; }
            public Thickness ScrollableHeadersMargin { get; private set; }

            public HorizontalLayoutInfo(double scrollOffset, double viewportWidth, double frozenWidth)
            {
                this.FrozenWidth = frozenWidth;
                if (viewportWidth < this.FrozenWidth)
                    this.FrozenColumnsMargin = new Thickness(0, 0, 0, 0);
                else
                    this.FrozenColumnsMargin = new Thickness(scrollOffset, 0, 0, 0);
                this.ScrollableColumnsMargin = new Thickness(frozenWidth, 0, 0, 0);
                this.FrozenHeadersMargin = new Thickness(0);
                this.ScrollableHeadersMargin = new Thickness(frozenWidth - scrollOffset, 0, 0, 0);
            }
        }

        #region ColumnsProperty

        public GridViewColumn[] Columns
        {
            get { return (GridViewColumn[])GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumn[]), typeof(ListViewEx),
                                        new PropertyMetadata(null, ColumnsChanged));

        private static void ColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListViewEx)d).ResetColumns();
        }

        #endregion

        #region FrozenColumnIndex

        public int FrozenColumnIndex
        {
            get { return (int)GetValue(FrozenColumnIndexProperty); }
            set { SetValue(FrozenColumnIndexProperty, value); }
        }

        public static readonly DependencyProperty FrozenColumnIndexProperty =
            DependencyProperty.Register("FrozenColumnIndex", typeof(int), typeof(ListViewEx),
                                        new PropertyMetadata(-1, FrozenColumnIndexChanged));

        private static void FrozenColumnIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListViewEx)d).ResetColumns();
        }

        #endregion

        #region FrozenColumns

        public GridViewColumnCollection FrozenColumns
        {
            get { return (GridViewColumnCollection)GetValue(FrozenColumnsProperty); }
            protected set { SetValue(_FrozenColumnsPropertyKey, value); }
        }

        static readonly DependencyPropertyKey _FrozenColumnsPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "FrozenColumns", typeof(GridViewColumnCollection), typeof(ListViewEx), new PropertyMetadata(null));
        public static readonly DependencyProperty FrozenColumnsProperty = _FrozenColumnsPropertyKey.DependencyProperty;

        #endregion

        #region ScrollableColumns

        public GridViewColumnCollection ScrollableColumns
        {
            get { return (GridViewColumnCollection)GetValue(ScrollableColumnsProperty); }
            protected set { SetValue(_ScrollableColumnsPropertyKey, value); }
        }

        static readonly DependencyPropertyKey _ScrollableColumnsPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "ScrollableColumns", typeof(GridViewColumnCollection), typeof(ListViewEx), new PropertyMetadata(null));
        public static readonly DependencyProperty ScrollableColumnsProperty = _ScrollableColumnsPropertyKey.DependencyProperty;

        #endregion

        #region HorizontalLayout

        public HorizontalLayoutInfo HorizontalLayout
        {
            get { return (HorizontalLayoutInfo)GetValue(HorizontalLayoutProperty); }
            protected set { SetValue(_HorizontalLayoutPropertyKey, value); }
        }

        static readonly DependencyPropertyKey _HorizontalLayoutPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "HorizontalLayout", typeof(HorizontalLayoutInfo), typeof(ListViewEx), new PropertyMetadata(new HorizontalLayoutInfo(0, 0, 0)));
        public static readonly DependencyProperty HorizontalLayoutProperty = _HorizontalLayoutPropertyKey.DependencyProperty;

        #endregion

        private void ResetColumns()
        {
            if (this.FrozenColumns != null)
            {
                foreach (var col in this.FrozenColumns.Cast<INotifyPropertyChanged>())
                    col.PropertyChanged -= frozencol_PropertyChanged;
                this.FrozenColumns.Clear();
            }

            if (this.ScrollableColumns != null)
                this.ScrollableColumns.Clear();

            var frozenColumnIndex = this.FrozenColumnIndex;
            var frozenColumns = new GridViewColumnCollection();
            var scrollableColumns = new GridViewColumnCollection();
            var columns = this.Columns;
            if (columns != null)
            {
                foreach (var col in columns.Take(frozenColumnIndex + 1))
                {
                    ((INotifyPropertyChanged)col).PropertyChanged += frozencol_PropertyChanged;
                    frozenColumns.Add(col);
                }
                foreach (var col in columns.Skip(frozenColumnIndex + 1))
                    scrollableColumns.Add(col);
            }
            this.FrozenColumns = frozenColumns;
            this.ScrollableColumns = scrollableColumns;
            ResetHorizontalLayout();
        }

        private void frozencol_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActualWidth")
            {
                ResetHorizontalLayout();
            }
        }
        
        private void ResetHorizontalLayout()
        {
            var frozenColumns = this.FrozenColumns;
            double frozenWidth = frozenColumns != null ? 
                                 frozenColumns.Select(c => c.ActualWidth).Sum()+ 1 : 0;
            double horizontalOffset = _scrollViewer != null ? _scrollViewer.HorizontalOffset : 0;
            double viewportWidth = _scrollViewer != null ? _scrollViewer.ViewportWidth: 0;

            this.HorizontalLayout = new HorizontalLayoutInfo(horizontalOffset, viewportWidth, frozenWidth);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
                _scrollViewer.SizeChanged += _scrollViewer_SizeChanged;
            }
            this.ResetHorizontalLayout();
        }

        private void _scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ResetHorizontalLayout();
        }

        void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange == 0)
                return;
            this.ResetHorizontalLayout();
        }

    }
}
