using Qdrant.Client.Grpc;
using System;

namespace YAOCr.Core.Mappers;

public static class MapperExtensions {
    public static DateTime GetPayloadDateTimeValue(this RetrievedPoint point, string key)=>
        DateTime.Parse(point.GetPayloadStringValue(key));

    public static string GetPayloadStringValue(this RetrievedPoint point, string key) {
        if (point.Payload.ContainsKey(key)) {
            return point.Payload[key].StringValue;
        }

        throw new Exception($"Key '{key}' not found in payload.");
    }
}