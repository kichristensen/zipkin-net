﻿namespace ZipkinNet.Serialization.Json

open ZipkinNet
open Newtonsoft.Json
open System

[<AllowNullLiteralAttribute>]
type JsonEndpoint(endpoint : Endpoint) =
    [<JsonPropertyAttribute("ipv4")>]
    member __.IPv4 = endpoint.IPv4

    [<JsonPropertyAttribute("port")>]
    member __.Port =
        match endpoint.Port with
        | Some x -> x
        | None -> 0s

    [<JsonPropertyAttribute("serviceName")>]
    member __.ServiceName = endpoint.ServiceName

type JsonAnnotation(annotation : Annotation) =
    [<JsonPropertyAttribute("endpoint", NullValueHandling = NullValueHandling.Ignore)>]
    member __.EndPoint : JsonEndpoint =
        match annotation.Endpoint with
        | Some x -> new JsonEndpoint(x)
        | None -> null

    [<JsonPropertyAttribute("value")>]
    member __.Value = annotation.Value

    [<JsonPropertyAttribute("timestamp")>]
    member __.Timestamp = annotation.Timestamp.UnixTimeMicroseconds

type JsonBinaryAnnotation(annotation : BinaryAnnotation) =
    [<JsonPropertyAttribute("endpoint", NullValueHandling = NullValueHandling.Ignore)>]
    member __.Endpoint =
        match annotation.Host with
        | Some x -> new JsonEndpoint(x)
        | None -> null

    [<JsonPropertyAttribute("key")>]
    member __.Key = annotation.Key

    [<JsonPropertyAttribute("value")>]
    member __.Value = annotation.Value

type JsonSpan(span : Span) =
    [<JsonPropertyAttribute("traceId")>]
    member __.TraceId = span.TraceId.Id

    [<JsonPropertyAttribute("name")>]
    member __.Name = span.Name

    [<JsonPropertyAttribute("id")>]
    member __.Id = span.Id.Id

    [<JsonPropertyAttribute("parentId", NullValueHandling = NullValueHandling.Ignore)>]
    member __.ParentId =
        match span.ParentId with
        | Some x -> x.Id
        | None -> null

    [<JsonPropertyAttribute("annotations")>]
    member __.Annotations = span.Annotations |> List.map (fun x -> new JsonAnnotation(x))

    [<JsonPropertyAttribute("binaryAnnotations")>]
    member __.BinaryAnnotations = span.BinaryAnnotations |> List.map (fun x -> new JsonBinaryAnnotation(x))

    [<JsonPropertyAttribute("timestamp", NullValueHandling = NullValueHandling.Ignore)>]
    member __.Timestamp : Nullable<int64> =
        match span.Timestamp with
        | Some x -> new Nullable<int64>(x.UnixTimeMicroseconds)
        | None -> new Nullable<int64>()

    [<JsonPropertyAttribute("duration", NullValueHandling = NullValueHandling.Ignore)>]
    member __.Duration : Nullable<int64> =
        match span.Duration with
        | Some x -> new Nullable<int64>(x.TotalMicroseconds)
        | None -> new Nullable<int64>()

[<AbstractClassAttribute; SealedAttribute>]
type JsonZipkinSerializer() =
    static member Serialize(span : JsonSpan) =
        JsonConvert.SerializeObject(span)
    static member Serialize(spans : JsonSpan seq) =
        JsonConvert.SerializeObject(spans)
    static member Serialize(annotation : JsonAnnotation) =
        JsonConvert.SerializeObject(annotation)
    static member Serialize(binaryAnnotation : JsonBinaryAnnotation) =
        JsonConvert.SerializeObject(binaryAnnotation)
    static member Serialize(endpoint : JsonEndpoint) =
        JsonConvert.SerializeObject(endpoint)