namespace ZipkinNet

open System

[<AutoOpen>]
module Helpers =
    let unixBaseTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)
    type DateTimeOffset with
        member self.UnixTimeMicroseconds =
            (self - unixBaseTime).TotalMilliseconds * 1000.0 |> int64

    type TimeSpan with
        member self.TotalMicroseconds =
            self.TotalMilliseconds * 1000.0 |> int64