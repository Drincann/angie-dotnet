namespace Angie;
public class Router : IMiddleware {
  private Dictionary<HTTPMethod, Tools.Trie> routes = new()
  {
        { HTTPMethod.GET, new() },
        { HTTPMethod.POST, new() },
        { HTTPMethod.PUT, new() },
        { HTTPMethod.DELETE, new() },
    };
  private Router route(HTTPMethod method, string path, Handler handler) {
    routes[method].insert(path.Split('/').ToList(), handler);
    return this;
  }
  public void handle(IContext ctx, NextFunc next) {
    var matchResult = routes[ctx.req.method].matchPath(ctx.req.path);
    if (matchResult.matched?.handler != null) {
      ctx.req.routeParams = matchResult.paramDict;
      matchResult.matched.handler(ctx);
    } else {
      next();
    }
  }

  public Router get(string path, Handler handler) {
    this.route(HTTPMethod.GET, path, handler);
    return this;
  }

  public Router post(string path, Handler handler) {
    this.route(HTTPMethod.POST, path, handler);
    return this;
  }

  public Router put(string path, Handler handler) {
    this.route(HTTPMethod.PUT, path, handler);
    return this;
  }

  public Router delete(string path, Handler handler) {
    this.route(HTTPMethod.DELETE, path, handler);
    return this;
  }
}