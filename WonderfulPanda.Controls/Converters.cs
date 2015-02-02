using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WonderfulPanda.Controls
{
    public class OneWayMultiConverter<T> : IMultiValueConverter
    {
        readonly Func<object[], T> _convert;
        public OneWayMultiConverter(Func<object[], T> convert)
        {
            _convert = convert;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _convert(values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class OneWayValueConverter<T> : IValueConverter
    {

        readonly Func<object, T> _convert;
        public OneWayValueConverter(Func<object, T> convert)
        {
            _convert = convert;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public static class Converters
    {
        public static IMultiValueConverter MinOfInt { get; private set; }
        public static IMultiValueConverter MaxOfInt { get; private set; }
        public static IMultiValueConverter MinOfDouble { get; private set; }
        public static IMultiValueConverter MaxOfDouble { get; private set; }
        public static IMultiValueConverter Rect { get; private set; }
        public static IValueConverter LeftMargin { get; private set; }
        public static IValueConverter LeftMarginNegate { get; private set; }

        static Converters()
        {
            MinOfInt = new OneWayMultiConverter<int>(values => values.Select(ToIntSafe).Min());
            MaxOfInt = new OneWayMultiConverter<int>(values => values.Select(ToIntSafe).Max());
            MinOfDouble = new OneWayMultiConverter<double>(values => values.Select(ToDoubleSafe).Min());
            MaxOfDouble = new OneWayMultiConverter<double>(values => values.Select(ToDoubleSafe).Max());
            Rect = new OneWayMultiConverter<Rect>(values => {
                var vals = values.Select(ToDoubleSafe).Concat(Enumerable.Repeat(0.0, 4)).Take(4).ToArray();
                return new Rect(vals[0], vals[1], vals[2], vals[3]);
            });
            LeftMargin = new OneWayValueConverter<Thickness>(value => new Thickness(ToDoubleSafe(value), 0, 0, 0));
            LeftMarginNegate = new OneWayValueConverter<Thickness>(value => new Thickness(ToDoubleSafe(value) * -1, 0, 0, 0));
        }

        static int ToIntSafe(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        static double ToDoubleSafe(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }
    }
}
