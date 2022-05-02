namespace Angie;
class Router {
  private Dictionary<HTTPMethod, Dictionary<string, Handler>> routes = new()
  {
        { HTTPMethod.GET, new() },
        { HTTPMethod.POST, new() },
        { HTTPMethod.PUT, new() },
        { HTTPMethod.DELETE, new() },
    };
  public Router route(HTTPMethod method, string path, Handler handler) {
    routes[method][path] = handler;
    return this;
  }
  public void handle(IContext ctx) {
    if (!routes[ctx.req.method].ContainsKey(ctx.req.path)) {
      ctx.setStatus(404);
      return;
    }
    this.routes[ctx.req.method][ctx.req.path](ctx);
  }
}