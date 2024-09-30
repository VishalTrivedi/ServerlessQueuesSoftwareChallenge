using Azure.Storage.Queues.Models;
using BusinessLogic.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SVGService;
using System.Text.Json;

namespace UserFunctions;

public class GetUserSVG
{
    private readonly ILogger<GetUserSVG> _logger;
    private readonly SVGInfo _svgInfo;
    private readonly IUserRepository _userRepository;
    private const string _queueName = "my-queue";
    private const string _queueConnection = "AzureWebJobsStorage";

    public GetUserSVG(
        ILogger<GetUserSVG> logger,
        SVGInfo svgInfo,
        IUserRepository userRepository
        )
    {
        _logger = logger;
        _svgInfo = svgInfo;
        _userRepository = userRepository;
    }

    [Function(nameof(GetUserSVG))]
    public async Task<IActionResult> Run(
        [QueueTrigger(_queueName,
            Connection = _queueConnection)] QueueMessage message)
    {
        try
        {
            _logger.LogInformation($"Message receieved from queue {message.MessageText}");

            User? input = JsonSerializer.Deserialize<User>(message.MessageText);

            if (input is null
                || !input.IsUserValid
                || input.Id < 1)
            {
                _logger.LogError($"Message does not contain user First name, Last name or both, aborting request.");

                return new OkObjectResult("OK");
            }

            _logger.LogInformation($"User data found in message: {input.FirstName} {input.LastName}.");

            // Make an HTTP call to the REST API to get SVG Data
            string svgContent = await _svgInfo.GetSVGData($"{input.FirstName}{input.LastName}");

            _logger.LogInformation($"SVG data for user {input.FirstName} {input.LastName} received from API.");

            // Save SVG response the database, associated to the FirstName and LastName.
            await _userRepository.UpdateUser(
                id: input.Id,
                svgData: svgContent);

            _logger.LogInformation($"SVG data for user {input.FirstName} {input.LastName} saved in the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error encountered while getting SVG data for user/saving it: {ex.Message}");
        }

        return new OkObjectResult("OK");
    }
}