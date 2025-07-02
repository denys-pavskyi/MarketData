using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Serialization;

namespace MarketData.BLL.Models.Responses;

public class ErrorResponse
{
    public string? Message { get; set; }

    [JsonIgnore]
    public HttpStatusCode? HttpCode { get; set; } = HttpStatusCode.BadRequest;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }
}

public static class ErrorResponseExtensions
{
    public static ActionResult ToActionResult(this ErrorResponse errorResponse)
    {
        var statusCode = errorResponse.HttpCode ?? HttpStatusCode.BadRequest;
        return statusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(errorResponse),
            _ => new ObjectResult(errorResponse) { StatusCode = (int)statusCode }
        };
    }
}