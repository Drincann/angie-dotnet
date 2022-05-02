namespace Angie;

class MiddlewareFuncWrapper : IMiddleware {
  public MiddlewareFunc func;
  public MiddlewareFuncWrapper(MiddlewareFunc func) {
    this.func = func;
  }
  public void handle(IContext ctx, NextFunc next) {
    func(ctx, next);
  }
}
public class Application : IApplication {
  private IHTTPServer underlyingServer;
  private List<IMiddleware> middlewares;

  public Application() {
    this.underlyingServer = new HTTPListenerHTTPServer();
    this.middlewares = new();
  }
  public IApplication use(IMiddleware middleware) {
    this.middlewares.Add(middleware);
    return this;
  }
  public IApplication use(MiddlewareFunc middleware) {
    this.middlewares.Add(new MiddlewareFuncWrapper(middleware));
    return this;
  }
  class NotFoundMiddleware : IMiddleware {
    public void handle(IContext ctx, NextFunc next) {
      ctx.setStatus(404);
      ctx.resHTML($"<h1>404 Not Found</h1> <p>The requested URL {ctx.req.path} was not found on this server.</p> <hr> <address>Angie</address>");
    }
  }
  private void handle(IContext ctx) {
    if (this.middlewares.Count == 0) return;
    int mdwIdx = -1;
    NextFunc next = () => { };
    next = () => {
      if (mdwIdx == this.middlewares.Count - 1) {
        new NotFoundMiddleware().handle(ctx, next);
        return;
      };
      this.middlewares[++mdwIdx].handle(ctx, next);
    };
    next();
  }

  public void listen(int port) {
    this.listen(port, null);
  }
  public void listen(int port, Callback<int?>? callback) {
    this.underlyingServer.addHandler(this.handle);
    this.underlyingServer.listen(port, callback);
  }
}