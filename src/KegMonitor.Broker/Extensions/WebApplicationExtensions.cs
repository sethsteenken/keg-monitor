using MQTTnet.AspNetCore;
using MQTTnet.Server;
using System.Net;

namespace KegMonitor.Broker
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseMqttWebSocketEndpoint(
            this WebApplication app,
            string path = "/mqtt",
            WebSocketOptions? options = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return app;
            }

            if (options == null)
            {
                options = new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(120)
                };
            }

            app.UseWebSockets(options);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == path)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        string? subProtocol = null;
                        if (context.Request.Headers.TryGetValue("Sec-WebSocket-Protocol", out var requestedSubProtocolValues))
                        {
                            subProtocol = MqttSubProtocolSelector.SelectSubProtocol(requestedSubProtocolValues);
                        }

                        using (var webSocket = await context.WebSockets.AcceptWebSocketAsync(subProtocol).ConfigureAwait(false))
                        {
                            await context.RequestServices.GetRequiredService<MqttWebSocketServerAdapter>()
                                                        .RunWebSocketConnectionAsync(webSocket, context).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    await next().ConfigureAwait(false);
                }
            });

            return app;
        }

        public static WebApplication StartMqttServer(
            this WebApplication app)
        {
            var options = app.Services.GetRequiredService<IMqttServerOptions>();
            app.Services.GetRequiredService<IMqttServer>().StartAsync(options).GetAwaiter().GetResult();

            return app;
        }
    }
}
