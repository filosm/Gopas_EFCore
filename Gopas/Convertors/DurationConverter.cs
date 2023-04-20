using Gopas.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gopas.Convertors;

class DurationConverter : ValueConverter<Duration, int>
{
    // staré verze .Net (v5 a méně) neumí pracovat s NULL hodnotami
    public DurationConverter() : base(
        duration => (int)duration.Time.TotalMilliseconds, // konverze "DO"
        ms => new Duration(ms)) // konverze "Z"
    {
    }
}