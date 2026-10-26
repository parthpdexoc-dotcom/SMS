using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System.Linq;

namespace SMS.Generic.Dtos.Extensions;
public static class SpecificDataExtractor
{
    public static List<Guid> ExtractId<T>(this IEnumerable<T> source,Func<T, Guid?> selector)
    {
        return source
            .Select(selector)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
    }
}