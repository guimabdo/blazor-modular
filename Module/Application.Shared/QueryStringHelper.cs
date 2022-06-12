using System.Collections;

namespace MyApp.Module.Application.Shared;

public static class QueryStringHelper
{
    public static string ToQueryString(object instance)
    {
        var queries = new List<string>();
        var propertiesDictionary = instance.ToPropertyDictionary();

        queries.AddRange(propertiesDictionary.GetQueryStringFromStrings());
        queries.AddRange(propertiesDictionary.GetQueryStringFromNumerics());
        queries.AddRange(propertiesDictionary.GetQueryStringFromEnums());
        queries.AddRange(propertiesDictionary.GetQueryStringFromBool());
        queries.AddRange(propertiesDictionary.GetQueryStringFromDateTime());
        queries.AddRange(propertiesDictionary.GetQueryStringFromIEnumerable());
        
        return queries.JoinQueriesString();
    }
    
    
    private static IDictionary<string, object?> ToPropertyDictionary(this object instance)
    {
        var type = instance.GetType();
        var propertyInfos = type.GetProperties().Where(p => p.CanRead);
        return propertyInfos.ToDictionary(propertyInfo => 
            propertyInfo.Name, 
            propertyInfo => propertyInfo.GetValue(instance, null)
        );
    }

    private static IEnumerable<string> GetQueryStringFromStrings(this IDictionary<string, object?> propertiesDictionary, string? listTag = null)
    {
        return propertiesDictionary
            .Where(p => p.Value is string and not null)
            .Select(p => CreateQueryStringFromPair( listTag ?? p.Key, p.Value as string ))
            .ToList();
    }
    private static IEnumerable<string> GetQueryStringFromEnums(this IDictionary<string, object?> propertiesDictionary, string? listTag = null)
    {
        return  propertiesDictionary
            .Where(p => p.Value is Enum)
            .Select(p => CreateQueryStringFromPair(listTag ?? p.Key, p.Value?.ReadEnumNumber()))
            .ToList();
    }
    private static IEnumerable<string> GetQueryStringFromNumerics(this IDictionary<string, object?> propertiesDictionary, string? listTag = null)
    {
        return propertiesDictionary
            .Where(p => p.Value is int or long or decimal or float or double)
            .Select(p => CreateQueryStringFromPair(listTag ?? p.Key, p.Value?.ToString()))
            .ToList();
    }
    private static IEnumerable<string> GetQueryStringFromBool(this IDictionary<string, object?> propertiesDictionary)
    {
        return propertiesDictionary
            .Where(p => p.Value is bool)
            .Select(p => CreateQueryStringFromPair(p.Key, p.Value?.ToString()))
            .ToList();
    }
    private static IEnumerable<string> GetQueryStringFromDateTime(this IDictionary<string, object?> propertiesDictionary, string? listTag = null)
    {
        return propertiesDictionary
            .Where(p => p.Value is DateTime or DateOnly or TimeOnly or TimeSpan)
            .Select(p => CreateQueryStringFromPair(listTag ?? p.Key, p.Value?.ToString()))
            .ToList();
    }
    private static IEnumerable<string> GetQueryStringFromIEnumerable(this IDictionary<string, object?> propertiesDictionary)
    {
        return propertiesDictionary
            .Where(p => p.Value is IEnumerable)
            .Select(p => p.CreateQueryStringFromList())
            .ToList();
    }
    
    private static string? ReadEnumNumber(this object value)
    {
        var type = value.GetType();
        return Convert.ChangeType(value, Enum.GetUnderlyingType(type)).ToString();
    }

    private static string CreateQueryStringFromList(this KeyValuePair<string, object?> valuePair)
    {
        if (valuePair.Value is string) return string.Empty;
        if (valuePair.Value is not IEnumerable list) return string.Empty;
        var count = 0;
        var dictionary = list.Cast<object?>().ToList().ToDictionary(_ => $"{valuePair.Key}[{count++}]", o => o);
        var queries = new List<string>();
        
        queries.AddRange(dictionary.GetQueryStringFromStrings());
        queries.AddRange(dictionary.GetQueryStringFromNumerics());
        queries.AddRange(dictionary.GetQueryStringFromEnums());
        queries.AddRange(dictionary.GetQueryStringFromDateTime());
        
        return queries.JoinQueriesString();
    }
    private static string CreateQueryStringFromPair(string key, string? value)
    {
        return value is null ? string.Empty : $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
    }
    private static string JoinQueriesString(this IEnumerable<string> queries)
    {
        return string.Join("&", queries.Where(q => q != string.Empty).OrderBy(p=> p));
    }
}