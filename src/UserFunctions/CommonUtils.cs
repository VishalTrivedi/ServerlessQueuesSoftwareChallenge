using Microsoft.AspNetCore.Mvc;

namespace UserFunctions;

public static class CommonUtils
{
    public static ObjectResult GetResult(
        string message,
        int statusCode)
    {
        var objReturn = new ObjectResult(message);

        objReturn.StatusCode = statusCode;

        return objReturn;
    }
}
