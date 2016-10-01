namespace ZipkinNet

open System
open System.Net
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
        IPv4 : string;
        Port : int16 option;
        ServiceName : string;
    }
    static member private ConvertIpToInt(ipv4 : IPAddress) =
        let ipBytes = ipv4.GetAddressBytes()
        let i = BitConverter.ToInt32(ipBytes, 0)
        IPAddress.HostToNetworkOrder(i)
//        if BitConverter.IsLittleEndian then Array.Reverse(ipBytes)
//        BitConverter.ToInt32(ipBytes, 0)
    static member Create(ipv4 : IPAddress, port, serviceName) = { IPv4 = ipv4.ToString(); ServiceName = serviceName; Port = Some port }
    static member CreateWithoutPort(ipv4 : IPAddress, serviceName) = { IPv4 = ipv4.ToString(); ServiceName = serviceName; Port = None }

type Annotation =
    {
        Timestamp : DateTimeOffset;
        Value : string;
        Endpoint : Endpoint option;
    }
    static member Create(timestamp : DateTimeOffset, value) = { Timestamp = timestamp; Value = value; Endpoint = None }
    static member CreateClientSend(timestamp : DateTimeOffset) = { Timestamp = timestamp; Value = ZipkinConstants.ClientSend; Endpoint = None }
    static member CreateClientReceive(timestamp : DateTimeOffset) = { Timestamp = timestamp; Value = ZipkinConstants.ClientReceive; Endpoint = None }
    static member CreateServerReceive(timestamp : DateTimeOffset) = { Timestamp = timestamp; Value = ZipkinConstants.ServerReceive; Endpoint = None }
    static member CreateServerSend(timestamp : DateTimeOffset) = { Timestamp = timestamp; Value = ZipkinConstants.ServerSend; Endpoint = None }
    static member CreateLocalComponent(timestamp : DateTimeOffset) = { Timestamp = timestamp; Value = ZipkinConstants.LocalComponent; Endpoint = None }
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
        Timestamp : DateTimeOffset option;
        Duration : TimeSpan option;
    }
    static member Create(traceId, name, id) = { TraceId = traceId; Name = name; Id = id; ParentId = None; Annotations = []; BinaryAnnotations = []; Debug = false; Timestamp = None; Duration = None }
    static member CreateRootSpan(name) = Span.Create(ZipkinId.Create(), name, ZipkinId.Create())
    static member CreateChildSpan(name, parentSpan : Span) = Span.Create(parentSpan.TraceId, name, ZipkinId.Create()).WithParentId(parentSpan.Id)
    member self.WithParentId(id) = { self with ParentId = Some id }
    member self.AddAnnotation(annotation) = { self with Annotations = annotation :: self.Annotations }
    member self.AddAnnotation(annotations : Annotation seq) = { self with Annotations = self.Annotations @ (annotations |> Seq.toList) }
    member self.AddBinaryAnnotation(annotation) = { self with BinaryAnnotations = annotation :: self.BinaryAnnotations }
    member self.AddBinaryAnnotation(annotations : BinaryAnnotation seq) = { self with BinaryAnnotations = self.BinaryAnnotations @ (annotations |> Seq.toList) }
    member self.IsDebug() = { self with Debug = true }
    member self.WithTimestamp(timestamp : DateTimeOffset) = { self with Timestamp = Some timestamp }
    member self.WithDuration(duration : TimeSpan) = { self with Duration = Some duration }