using System;
using System.Globalization;
using AutoMapper;

namespace Schedule.Mapper
{
    public class DateTimeTypeConverter : ITypeConverter<string, TimeSpan?>
    {
        public TimeSpan? Convert(string source, TimeSpan? destination, ResolutionContext context)
        {
            return DateTime.ParseExact(source, new[] { "HH:mm", "HH.mm" }, CultureInfo.InvariantCulture).TimeOfDay;
        }
    }
}