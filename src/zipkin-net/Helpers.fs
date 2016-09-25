namespace ZipkinNet

open System
open System.Runtime.InteropServices
open System.Globalization

[<AutoOpen>]
module Helpers =
    let unixBaseTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)
    type DateTimeOffset with
        member self.UnixTimeMicroseconds =
            (self - unixBaseTime).TotalMilliseconds * 1000.0 |> int64

    type TimeSpan with
        member self.TotalMicroseconds =
            self.TotalMilliseconds * 1000.0 |> int64

    type String with
        static member GenerateRandomHexEncodedInt64String() =
            Convert.ToString(BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0), 16)

type ZipkinId private(id) =
    member val Id = id with get
    static member Create() = ZipkinId(String.GenerateRandomHexEncodedInt64String())
    static member TryParse(str, [<OutAttribute>] id : byref<ZipkinId>) =
        let success, _ = Int64.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture)
        if success then
            id <- ZipkinId(str)
            true
        else
            false