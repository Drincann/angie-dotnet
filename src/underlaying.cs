using System.Net;
using System.Threading.Tasks;
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
      // async call
      Task.Run(() => {
        Context ctx = new(req, res);

        foreach (var handler in this.handlers) {
          Task.Run(() => handler(ctx));
          Console.WriteLine($"{ctx.req.method} {ctx.req.path}");
        }
        res.StatusCode = ctx.res.status;
        foreach (var header in ctx.res.header) {
          res.AddHeader(header.Key, header.Value);
        }
        ctx.res.body.Flush();
        res.Close();
      });
    }
  }
}


public enum HTTPMethod {
  GET,
  POST,
  PUT,
  DELETE
}
public class Request : IRequest {
  private HttpListenerRequest originReq;
  public HTTPMethod method { get; private set; }
  public string path { get; private set; }
  public string? body { get; private set; }
  public Dictionary<string, List<string>>? query { get; private set; }
  public Dictionary<string, string>? routeParams { get; set; }
  private Dictionary<string, List<string>>? parseQueryString(string? queryString) {
    if (queryString == null || queryString.Length == 0) return null;
    try {
      var queryParts = queryString.Split('&');
      var queryDict = new Dictionary<string, List<string>>();
      foreach (var queryPart in queryParts) {
        var keyValue = queryPart.Split('=');
        var key = keyValue[0];
        var value = keyValue[1];
        if (queryDict.ContainsKey(key)) {
          queryDict[key].Append(value);
        } else {
          queryDict[key] = new List<string>() { value };
        }
        return queryDict;
      }
    } catch {
      return null;
    }
    return null;
  }
  public Request(HttpListenerRequest originReq) {
    this.originReq = originReq;
    this.method = (HTTPMethod)Enum.Parse(typeof(HTTPMethod), originReq.HttpMethod);
    this.path = originReq?.Url?.AbsolutePath ?? "";
    this.body = originReq != null ? new StreamReader(originReq.InputStream).ReadToEnd() : null;
    this.query = this.parseQueryString(originReq?.Url?.Query?.TrimStart('?'));
  }
}
public class Response : IResponse {
  public Dictionary<string, string> header { get; private set; }
  public int status { get; set; }
  public StreamWriter body { get; set; }
  public Response(HttpListenerResponse originRes) {
    this.status = 200;
    this.body = new(originRes.OutputStream);
    this.header = new();
  }
}
public class Context : IContext {

  public IRequest req { get; private set; }
  public IResponse res { get; private set; }

  public Context(HttpListenerRequest originReq, HttpListenerResponse originRes) {
    this.req = new Request(originReq);
    this.res = new Response(originRes);
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

  public string? getQuery(string key) {
    if (this.req.query?.ContainsKey(key) == true) {
      return this.req.query[key][0];
    }
    return null;
  }

  public string? getRouteParam(string key) {
    return this.req.routeParams?[key];
  }
}
