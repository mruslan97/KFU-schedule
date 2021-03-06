﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vk.Bot.Framework.Abstractions;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace Vk.Bot.Framework.Middlewares
{
    /// <summary>
    /// Middleware for handling Telegram bot's webhook requests
    /// </summary>
    /// <typeparam name="TBot">Type of bot</typeparam>
    public class BotMiddleware<TBot>
        where TBot : BotBase<TBot>
    {
        private readonly RequestDelegate _next;

        private readonly IBotManager<TBot> _botManager;

        private readonly ILogger<BotMiddleware<TBot>> _logger;

        /// <summary>
        /// Initializes an instance of middleware
        /// </summary>
        /// <param name="next">Instance of request delegate</param>
        /// <param name="botManager">Bot manager for the bot</param>
        /// <param name="logger">Logger for this middleware</param>
        public BotMiddleware(RequestDelegate next,
            IBotManager<TBot> botManager,
            ILogger<BotMiddleware<TBot>> logger)
        {
            _next = next;
            _botManager = botManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets invoked to handle the incoming request
        /// </summary>
        /// <param name="context"></param>
        public async Task Invoke(HttpContext context)
        {
            if (_botManager.WebhookUrl == null || 
                !_botManager.WebhookUrl.EndsWith(context.Request.Path))
            {
                await _next.Invoke(context);
                return;
            }
            string data;
            using (var reader = new StreamReader(context.Request.Body))
            {
                data = await reader.ReadToEndAsync();
            }

            GroupUpdate update = null;
            try
            {
                var jtoken = JToken.Parse(data);
                var vkRequest = new VkResponse(jtoken);
                update = GroupUpdate.FromJson(vkRequest);
            }
            catch (JsonException e)
            {
                _logger.LogWarning($"Unable to deserialize update payload. {e.Message}");
            }
            if (update == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            try
            {
                await _botManager.HandleUpdateAsync(update);
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes("ok"));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while handling update. {e.Message}");
                _logger.LogWarning($"Отправлено 500 вк");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}