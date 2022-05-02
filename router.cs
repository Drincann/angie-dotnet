namespace Angie;
class Router {
  private Dictionary<HTTPMethod, Tools.Trie> routes = new()
  {
        { HTTPMethod.GET, new() },
        { HTTPMethod.POST, new() },
        { HTTPMethod.PUT, new() },
        { HTTPMethod.DELETE, new() },
    };
  public Router route(HTTPMethod method, string path, Handler handler) {
    routes[method].insert(path.Split('/').ToList(), handler);
    return this;
  }
  public void handle(IContext ctx) {
    var matchResult = routes[ctx.req.method].matchPath(ctx.req.path);
    if (matchResult.matched?.handler != null) {
      ctx.req.routeParams = matchResult.paramDict;
      matchResult.matched.handler(ctx);
    } else {
      ctx.setStatus(404);
    }
  }
}