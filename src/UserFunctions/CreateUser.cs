using System.Text.Json;
using Azure.Storage.Queues;
using BusinessLogic.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserFunctions;

public class CreateUser
{
    private readonly ILogger<CreateUser> _logger;
    private readonly IUserRepository _userRepository;
    private readonly QueueClient _queueClient;
    private const string _queueName = "my-queue";

    public CreateUser(
        ILogger<CreateUser> logger,
        IUserRepository userRepository,
        QueueServiceClient queueServiceClient)
    {
        _logger = logger;
        _userRepository = userRepository;

        _queueClient = queueServiceClient.GetQueueClient(_queueName);
    }

    [Function("CreateUser")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest createUserRequest)
    {
        ObjectResult retval;

        string requestBody = await new StreamReader(createUserRequest.Body).ReadToEndAsync();

        try
        {
            // TODO: Json validation
            User? input = JsonSerializer.Deserialize<User>(requestBody);

            if (!input.IsUserValid)
            {
                _logger.LogError($"Request does not contain FirstName, LastName or both.");

                return CommonUtils.GetResult(
                    message: $"Please ensure request includes FirstName and LastName",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation($"Create User request receieved for user {input.FirstName} {input.LastName}.");

            // Save FirstName and LastName to DB
            User? createdUser = await _userRepository.CreateUser(
                firstName: input.FirstName, 
                lastName: input.LastName);

            if (createdUser is null)
            {
                _logger.LogInformation($"User {input.FirstName} {input.LastName} already exists.");

                return CommonUtils.GetResult(
                    message: $"User {input.FirstName} {input.LastName} already exists, request ignored.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            // If successful, Publish message on the AZ storage queue
            await _queueClient.SendMessageAsync(JsonSerializer.Serialize(createdUser));

            _logger.LogInformation($"Message dropped in the queue for user {input.FirstName} {input.LastName}");

            retval = CommonUtils.GetResult(
                message: $"User {input.FirstName} {input.LastName} created successfully.",
                statusCode: StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error encountered while creating user: {ex.Message}");
            
            retval = CommonUtils.GetResult(
                message: "An unexpected error has occured, please try again.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation($"Create User request receieved processed successfully.");

        // Return status code to user
        return retval;
    }
}
