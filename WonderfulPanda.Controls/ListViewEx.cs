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

        #region FrozenColumns

        /// <summary>
        /// 固定する列のコレクション
        /// </summary>
        public GridViewColumnCollection FrozenColumns
        {
            get { return (GridViewColumnCollection)GetValue(FrozenColumnsProperty); }
            set { SetValue(FrozenColumnsProperty, value); }
        }

        public static readonly DependencyProperty FrozenColumnsProperty =
            DependencyProperty.Register("FrozenColumns", typeof(GridViewColumnCollection), typeof(ListViewEx), 
                                        new PropertyMetadata(new GridViewColumnCollection(), OnFrozenColumnsChanged));

        private static void OnFrozenColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as ListViewEx;
            if (d == null)
                return;

            if (e.OldValue != null)
            {
                foreach (INotifyPropertyChanged col in (GridViewColumnCollection)e.OldValue)
                    col.PropertyChanged -= target.frozencol_PropertyChanged;
            }
            if (e.NewValue != null)
            {
                foreach (INotifyPropertyChanged col in (GridViewColumnCollection)e.NewValue)
                    col.PropertyChanged += target.frozencol_PropertyChanged;
            }
        }

        private void frozencol_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActualWidth")
            {
                if (this.FrozenColumns.Contains(sender as GridViewColumn))
                    UpdateMargins();
            }
        }

        #endregion

        #region NormalColumns

        /// <summary>
        /// 固定しない(普通にスクロールする)列のコレクション
        /// </summary>
        public GridViewColumnCollection NormalColumns
        {
            get { return (GridViewColumnCollection)GetValue(NormalColumnsProperty); }
            set { SetValue(NormalColumnsProperty, value); }
        }

        public static readonly DependencyProperty NormalColumnsProperty =
            DependencyProperty.Register("NormalColumns", typeof(GridViewColumnCollection), typeof(ListViewEx), 
                                        new PropertyMetadata(new GridViewColumnCollection()));
        
        #endregion

        #region FrozenColumnsTotalWidth(ReadOnly)

        /// <summary>
        /// 固定列の合計幅
        /// </summary>
        public double FrozenColumnsTotalWidth
        {
            get { return (double)GetValue(FrozenColumnsTotalWidthProperty); }
            set { SetValue(FrozenColumnsTotalWidthPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey FrozenColumnsTotalWidthPropertyKey =
            DependencyProperty.RegisterReadOnly("FrozenColumnsTotalWidth", typeof(double), typeof(ListViewEx), 
                                                new PropertyMetadata(0.0));
        public static readonly DependencyProperty FrozenColumnsTotalWidthProperty = FrozenColumnsTotalWidthPropertyKey.DependencyProperty;
        
        #endregion

        #region FrozenColumnsOffset (ReadOnly)

        public double FrozenColumnsOffset
        {
            get { return (double)GetValue(FrozenColumnsOffsetProperty); }
            protected set { SetValue(_FrozenColumnsOffsetPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _FrozenColumnsOffsetPropertyKey =
            DependencyProperty.RegisterReadOnly("FrozenColumnsOffset", typeof(double), typeof(ListViewEx),
                                                new PropertyMetadata(0.0));
        public static readonly DependencyProperty FrozenColumnsOffsetProperty = _FrozenColumnsOffsetPropertyKey.DependencyProperty;
        
        #endregion

        private void UpdateMargins()
        {
            var frozenWidth = this.FrozenColumns.Sum(col => col.ActualWidth + 1);
            this.FrozenColumnsTotalWidth = frozenWidth;
            if (_scrollViewer != null)
            {
                var hOffset = _scrollViewer.HorizontalOffset;
                var viewWidth = _scrollViewer.ViewportWidth;
                if (frozenWidth < viewWidth)
                    this.FrozenColumnsOffset = hOffset;
                else if (hOffset + viewWidth - frozenWidth > 0)
                    this.FrozenColumnsOffset = hOffset + viewWidth - frozenWidth;
                else
                    this.FrozenColumnsOffset = 0;
            }
            else
            {
                this.FrozenColumnsOffset = 0;
            }
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
            this.UpdateMargins();
        }

        private void _scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateMargins();
        }

        void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange == 0)
                return;
            this.UpdateMargins();
        }

    }
}
