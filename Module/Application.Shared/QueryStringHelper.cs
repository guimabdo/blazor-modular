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
        queries.AddRange(propertiesDictionary.GetQueryStringFromGuids());
        
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

    private static IEnumerable<string> CreateQueryString(
        IDictionary<string, object?> propertiesDictionary,
        Func<KeyValuePair<string, object?>, bool> filter,
        Func<KeyValuePair<string, object?>, string> buildQueryString)
    {
        return propertiesDictionary
            .Where(filter)
            .Select(buildQueryString)
            .ToList();
    }

    private static IEnumerable<string> GetQueryStringFromStrings(this IDictionary<string, object?> propertiesDictionary)
        =>  CreateQueryString(
            propertiesDictionary,
            o => o.Value is string and not null,
            p => CreateQueryStringFromPair( p.Key, p.Value as string )
        );
    

    private static IEnumerable<string> GetQueryStringFromEnums(this IDictionary<string, object?> propertiesDictionary)
        => CreateQueryString(
            propertiesDictionary,
            o => o.Value is Enum,
            p => CreateQueryStringFromPair(p.Key, p.Value?.ReadEnumNumber())
        );

    private static IEnumerable<string> GetQueryStringFromNumerics(
        this IDictionary<string, object?> propertiesDictionary)
        => CreateQueryString(
            propertiesDictionary,
            o => o.Value is int or long or decimal or float or double,
            p => CreateQueryStringFromPair(p.Key, p.Value?.ToString())
        );
    
    private static IEnumerable<string> GetQueryStringFromBool(this IDictionary<string, object?> propertiesDictionary)
        => CreateQueryString(
            propertiesDictionary,
            o => o.Value is bool,
            p => CreateQueryStringFromPair(p.Key, p.Value?.ToString())
        );
    
    private static IEnumerable<string> GetQueryStringFromDateTime(this IDictionary<string, object?> propertiesDictionary)
        => CreateQueryString(
            propertiesDictionary,
            o => o.Value is DateTime or DateOnly or TimeOnly or TimeSpan,
            p => CreateQueryStringFromPair(p.Key, p.Value?.ToString())
        );
    
    private static IEnumerable<string> GetQueryStringFromGuids(this IDictionary<string, object?> propertiesDictionary)
        => CreateQueryString(
            propertiesDictionary,
            o => o.Value is Guid,
            p => CreateQueryStringFromPair(p.Key, (p.Value as Guid?).ToString())
        );

    private static IEnumerable<string> GetQueryStringFromIEnumerable(this IDictionary<string, object?> propertiesDictionary)
        => CreateQueryString(
            propertiesDictionary,
            o => o.Value is IEnumerable,
            CreateQueryStringFromList
        );
    
    private static string? ReadEnumNumber(this object value)
    {
        var type = value.GetType();
        return Convert.ChangeType(value, Enum.GetUnderlyingType(type)).ToString();
    }

    private static string CreateQueryStringFromList(KeyValuePair<string, object?> valuePair)
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