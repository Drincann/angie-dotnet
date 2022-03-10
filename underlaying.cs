using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
namespace Angie;

class HTTPListenerHTTPServer : IHTTPServer {
    private HttpListener listener = new();
    private List<Handler> handlers = new();
    public void addHandler(Handler HTTPHandler) {
        this.handlers.Add(HTTPHandler);
    }
    public void listen(int port) {
        this.listen(port, null);
    }
    public void listen(int port, Callback<int?>? HTTPHandler) {
        this.listener.Prefixes.Add($"http://*:{port}/");
        this.listener.Start();
        if (HTTPHandler != null) HTTPHandler(null, null);

        while (true) {
            var connection = this.listener.GetContext();
            var req = connection.Request;
            var res = connection.Response;

            Context ctx = new(
                method: (HTTPMethod)Enum.Parse(typeof(HTTPMethod), req.HttpMethod),
                path: req?.Url?.AbsolutePath ?? "",
                reqBody: new(req?.InputStream.ToString()),
                stream: res.OutputStream
            );

            foreach (var handler in this.handlers) {
                handler(ctx);
            }
            res.StatusCode = ctx.res.status;
            foreach (var header in ctx.res.header) {
                res.AddHeader(header.Key, header.Value);
            }
            ctx.res.body.Flush();
            res.Close();
        }

    }
}


public enum HTTPMethod {
    GET,
    POST,
    PUT,
    DELETE
}

public class Context : IContext {
    public class Request : IRequest {
        public HTTPMethod method { get; private set; }
        public string path { get; private set; }
        public string body { get; private set; }
        public Request(HTTPMethod method, string path, string body) {
            this.method = method;
            this.path = path;
            this.body = body;
        }
    }
    public class Response : IResponse {
        public Dictionary<string, string> header { get; private set; }
        public int status { get; set; }
        public StreamWriter body { get; set; }
        public Response(int status, Stream stream) {
            this.status = status;
            this.body = new(stream);
            this.header = new();
        }
    }
    public IRequest req { get; private set; }
    public IResponse res { get; private set; }

    public Context(HTTPMethod method, string path, string reqBody, Stream stream) {
        this.req = new Context.Request(
            method: method,
            path: path,
            body: reqBody
        );
        this.res = new Context.Response(
            status: 200,
            stream: stream
        );
    }

    public IContext setStatus(int status) {
        this.res.status = status;
        return this;
    }

    public IContext setHeader(string key, string value) {
        this.res.header[key] = value;
        return this;
    }

    public IContext resJson(object obj) {
        this.res.header["Content-Type"] = "application/json";
        this.res.body.Write(
            System.Text.Json.JsonSerializer.Serialize(obj)
        );
        return this;
    }

    public IContext resString(string str) {
        this.res.header["Content-Type"] = "text/plain";
        this.res.body.Write(
            str
        );
        return this;
    }

    public IContext resHTML(string html) {
        this.res.header["Content-Type"] = "text/html";
        this.res.body.Write(
            html
        );
        return this;
    }
}
