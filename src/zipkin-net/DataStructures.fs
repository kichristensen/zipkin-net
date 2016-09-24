﻿namespace ZipkinNet.DataStructures

[<AbstractClassAttribute; SealedAttribute>]
type ZipkinConstants private() =
    static member ClientSend = "cs"
    static member ClientReceive = "cr"
    static member ServerSend = "ss"
    static member ServerReceive = "sr"
    static member LocalComponent = "lc"


type Endpoint =
    {
        ipv4 : int;
        port : int16;
        service_name : string;
    }

type Annotation =
    {
        timestamp : int64;
        value : string;
        endpoint : Endpoint option;
    }
    static member Create(timestamp, value) = { timestamp = timestamp; value = value; endpoint = None }
    member self.Endpoint(endpoint) = { self with endpoint = Some endpoint }

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
        key : string;
        value : byte array;
        annotation_type : AnnotationType;
        host : Endpoint option;
    }
    static member Create(key, value, annotationType) = { key = key; value = value; annotation_type = annotationType; host = None }
    member self.Endpoint(host) = { self with host = Some host }

type Span =
    {
        trace_id : int64;
        name : string;
        id : int64;
        parent_id : int64 option;
        annotations : Annotation list;
        binary_annotations : BinaryAnnotation list;
        debug : bool;
        timestamp : int64 option;
        duration : int64 option;
    }
    static member Create(traceId, name, id) = { trace_id = traceId; name = name; id = id; parent_id = None; annotations = []; binary_annotations = []; debug = false; timestamp = None; duration = None }
    member self.ParentId(id) = { self with parent_id = Some id }
    member self.AddAnnotation(annotation) = { self with annotations = annotation :: self.annotations }
    member self.AddAnnotation(annotations : Annotation seq) = { self with annotations = self.annotations @ (annotations |> Seq.toList) }
    member self.AddBinaryAnnotation(annotation) = { self with binary_annotations = annotation :: self.binary_annotations }
    member self.AddBinaryAnnotation(annotations : BinaryAnnotation seq) = { self with binary_annotations = self.binary_annotations @ (annotations |> Seq.toList) }
    member self.IsDebug() = { self with debug = true }
    member self.Timestamp(timestamp) = { self with timestamp = Some timestamp }
    member self.Duration(duration) = { self with duration = Some duration }