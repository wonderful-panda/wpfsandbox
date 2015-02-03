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
            DependencyProperty.Register("NormalColumns", typeof(GridViewColumnCollection), 
                                        typeof(ListViewEx), new PropertyMetadata(new GridViewColumnCollection()));
        
        #endregion

        #region FrozenColumnsTotalWidth(ReadOnly)

        /// <summary>
        /// リストの固定列の領域に適用されるマージン(ベースのScollViewerのスクロール量に応じて右にずらす)
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
        #region FrozenColumnsMargin(ReadOnly)

        /// <summary>
        /// リストの固定列の領域に適用されるマージン(ベースのScollViewerのスクロール量に応じて右にずらす)
        /// </summary>
        public Thickness FrozenColumnsMargin
        {
            get { return (Thickness)GetValue(FrozenColumnsMarginProperty); }
            set { SetValue(FrozenColumnsMarginPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey FrozenColumnsMarginPropertyKey =
            DependencyProperty.RegisterReadOnly("FrozenColumnsMargin", typeof(Thickness), typeof(ListViewEx), 
                                                new PropertyMetadata());
        public static readonly DependencyProperty FrozenColumnsMarginProperty = FrozenColumnsMarginPropertyKey.DependencyProperty;
        
        #endregion

        #region NormalColumnsMargin(ReadOnly)

        /// <summary>
        /// リストの固定しない列の領域に適用されるマージン(固定列の幅だけ右にずらす)
        /// </summary>
        public Thickness NormalColumnsMargin
        {
            get { return (Thickness)GetValue(NormalColumnsMarginProperty); }
            set { SetValue(NormalColumnsMarginPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey NormalColumnsMarginPropertyKey =
            DependencyProperty.RegisterReadOnly("NormalColumnsMargin", typeof(Thickness), typeof(ListViewEx), 
                                                new PropertyMetadata());
        public static readonly DependencyProperty NormalColumnsMarginProperty = NormalColumnsMarginPropertyKey.DependencyProperty;
        
        #endregion

        #region NormalHeadersMargin(ReadOnly)

        /// <summary>
        /// ヘッダーの固定しない列の領域に適用されるマージン(ヘッダーはScrollViewの外にあるので、このプロパティを使って自力でScrollViewと連動させる)
        /// </summary>
        public Thickness NormalHeadersMargin
        {
            get { return (Thickness)GetValue(NormalHeadersMarginProperty); }
            set { SetValue(NormalHeadersMarginPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey NormalHeadersMarginPropertyKey =
            DependencyProperty.RegisterReadOnly("NormalHeadersMargin", typeof(Thickness), typeof(ListViewEx), 
                                                new PropertyMetadata());
        public static readonly DependencyProperty NormalHeadersMarginProperty = NormalHeadersMarginPropertyKey.DependencyProperty;
        
        #endregion

        private void frozencol_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActualWidth")
            {
                UpdateMargins();
            }
        }

        private void UpdateMargins()
        {
            var frozenWidth = this.FrozenColumns.Sum(col => col.ActualWidth + 1);
            this.FrozenColumnsTotalWidth = frozenWidth;
            if (_scrollViewer != null)
            {
                var frozenColumnsOffset = _scrollViewer.ViewportWidth < frozenWidth ? 0 : _scrollViewer.HorizontalOffset;
                this.FrozenColumnsMargin = new Thickness(frozenColumnsOffset, 0, 0, 0);
                this.NormalColumnsMargin = new Thickness(frozenWidth, 0, 0, 0);
                this.NormalHeadersMargin = new Thickness(frozenWidth - _scrollViewer.HorizontalOffset, 0, 0, 0);
            }
            else
            {
                this.FrozenColumnsMargin = new Thickness(0);
                this.NormalColumnsMargin = new Thickness(0);
                this.NormalHeadersMargin = new Thickness(0);
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
