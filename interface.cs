namespace Angie;

public interface IApplication {
  public IApplication route(HTTPMethod method, string path, Handler handler);
  public IApplication get(string path, Handler handler);
  public IApplication post(string path, Handler handler);
  public IApplication put(string path, Handler handler);
  public IApplication delete(string path, Handler handler);


  public void listen(int port, Callback<int?>? callback);
  public void listen(int port);

}
public delegate void Handler(IContext ctx);
public delegate void Callback<T>(T data, System.Exception? err);

public interface IRequest {
  public HTTPMethod method { get; }
  public string path { get; }
  public string? body { get; }
  public Dictionary<string, List<string>>? query { get; }
}

public interface IResponse {
  public Dictionary<string, string> header { get; }
  public int status { get; set; }
  public StreamWriter body { get; set; }
}

public interface IContext {
  public IRequest req { get; }
  public IResponse res { get; }
  public IContext setStatus(int status);
  public IContext setHeader(string key, string value);
  public IContext resJson(object obj);
  public IContext resString(string str);
  public IContext resHTML(string html);
  public string? getQuery(string key);
}


interface IHTTPServer {
  public void addHandler(Handler HTTPHandler);
  public void listen(int port, Callback<int?>? callback);
  public void listen(int port);
}