#if TOOLS
using System;

namespace GdMUT;

public struct Result
{
    public bool IsSuccess;
    public string Message;
    public static readonly Result Success = new(true, String.Empty);
    public static readonly Result Failure = new(false, String.Empty);

    public Result(bool success, string message)
    {
        IsSuccess = success;
        Message = message;
    }
}
#endif
