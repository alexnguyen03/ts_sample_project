using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace ReverseProxy.Service
{
    public class ReversePorxy
    {
        public class Proxy
        {
            private readonly HttpListener _listener;
            private readonly int _port;

            public Proxy(int port)
            {
                _port = port;
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://*:{_port}/");
            }

            public void Start()
            {
                _listener.Start();
                Console.WriteLine($"Proxy started on port {_port}");
                while (true)
                {
                    var context = _listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
            }

            private void HandleRequest(HttpListenerContext context)
            {
                try
                {
                    // Get the target host from the request header
                    var host = context.Request.Headers["Host"];
                    Console.WriteLine($"Request for {host}");

                    // Create a TCP client and connect to the target host
                    using (var client = new TcpClient(hostname: host!, 80))
                    {
                        // Get the client stream
                        Stream clientStream = client.GetStream();

                        // If the target host uses SSL, create an SslStream
                        if (host!.StartsWith("https://"))
                        {
                            var sslStream = new SslStream(clientStream);
                            sslStream.AuthenticateAsClient(host);
                            clientStream = sslStream;
                        }

                        // Forward the request to the target host
                        ForwardRequest(context.Request, clientStream);

                        // Forward the response to the browser
                        ForwardResponse(context.Response, clientStream);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    context.Response.Close();
                }
            }

            private void ForwardRequest(HttpListenerRequest request, Stream clientStream)
            {
                // Create a stream writer to write to the client stream
                using (var writer = new StreamWriter(clientStream))
                {
                    // Write the request line
                    writer.WriteLine($"{request.HttpMethod} {request.RawUrl} HTTP/{request.ProtocolVersion}");

                    // Write the request headers
                    foreach (var key in request.Headers.AllKeys)
                    {
                        writer.WriteLine($"{key}: {request.Headers[key]}");
                    }

                    // Write an empty line to indicate the end of the headers
                    writer.WriteLine();

                    // Write the request body if any
                    if (request.HasEntityBody)
                    {
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            writer.Write(reader.ReadToEnd());
                        }
                    }

                    // Flush the writer
                    writer.Flush();
                }
            }

            private void ForwardResponse(HttpListenerResponse response, Stream clientStream)
            {
                // Create a stream reader to read from the client stream
                using (var reader = new StreamReader(clientStream))
                {
                    // Read the response line
                    var responseLine = reader.ReadLine();
                    var responseParts = responseLine!.Split(' ');
                    var statusCode = int.Parse(responseParts[1]);
                    var statusDescription = string.Join(" ", responseParts, 2, responseParts.Length - 2);

                    // Set the response status code and description
                    response.StatusCode = statusCode;
                    response.StatusDescription = statusDescription;

                    // Read the response headers
                    string header;
                    while (!string.IsNullOrEmpty(header = reader.ReadLine()!))
                    {
                        var headerParts = header.Split(new[] { ':' }, 2);
                        var name = headerParts[0].Trim();
                        var value = headerParts[1].Trim();

                        // Set the response header
                        response.Headers[name] = value;
                    }

                    // Copy the response body to the response output stream
                    using (var output = response.OutputStream)
                    {
                        reader.BaseStream.CopyTo(output);
                    }
                }
            }
        }
    }
}
