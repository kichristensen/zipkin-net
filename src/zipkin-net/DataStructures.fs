namespace ZipkinNet.DataStructures

open System
open ZipkinNet

[<AbstractClassAttribute; SealedAttribute>]
type ZipkinConstants private() =
    static member ClientSend = "cs"
    static member ClientReceive = "cr"
    static member ServerSend = "ss"
    static member ServerReceive = "sr"
    static member LocalComponent = "lc"

type Endpoint =
    {
        IPv4 : int;
        Port : int16;
        ServiceName : string;
    }

type Annotation =
    {
        Timestamp : int64;
        Value : string;
        Endpoint : Endpoint option;
    }
    static member Create(timestamp : DateTimeOffset, value) = { Timestamp = timestamp.UnixTimeMicroseconds; Value = value; Endpoint = None }
    member self.WithEndpoint(endpoint) = { self with Endpoint = Some endpoint }

type AnnotationType =
    | Bool = 0
    | ByteArray = 1
    | Int16 = 2
    | Int32 = 3
    | Int64 = 4
    | Double = 5
    | String = 6

type BinaryAnnotation =
    {
        Key : string;
        Value : byte array;
        AnnotationType : AnnotationType;
        Host : Endpoint option;
    }
    static member Create(key, value, annotationType) = { Key = key; Value = value; AnnotationType = annotationType; Host = None }
    member self.WithHost(host) = { self with Host = Some host }

type Span =
    {
        TraceId : ZipkinId;
        Name : string;
        Id : ZipkinId;
        ParentId : ZipkinId option;
        Annotations : Annotation list;
        BinaryAnnotations : BinaryAnnotation list;
        Debug : bool;
        Timestamp : int64 option;
        Duration : int64 option;
    }
    static member Create(traceId, name, id) = { TraceId = traceId; Name = name; Id = id; ParentId = None; Annotations = []; BinaryAnnotations = []; Debug = false; Timestamp = None; Duration = None }
    member self.WithParentId(id) = { self with ParentId = Some id }
    member self.AddAnnotation(annotation) = { self with Annotations = annotation :: self.Annotations }
    member self.AddAnnotation(annotations : Annotation seq) = { self with Annotations = self.Annotations @ (annotations |> Seq.toList) }
    member self.AddBinaryAnnotation(annotation) = { self with BinaryAnnotations = annotation :: self.BinaryAnnotations }
    member self.AddBinaryAnnotation(annotations : BinaryAnnotation seq) = { self with BinaryAnnotations = self.BinaryAnnotations @ (annotations |> Seq.toList) }
    member self.IsDebug() = { self with Debug = true }
    member self.WithTimestamp(timestamp : DateTimeOffset) = { self with Timestamp = Some timestamp.UnixTimeMicroseconds }
    member self.WithDuration(duration : TimeSpan) = { self with Duration = Some duration.TotalMicroseconds }