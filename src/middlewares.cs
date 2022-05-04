using Angie.Tools;
namespace Angie.Middlewares;

class Static : IMiddleware {
  private string staticRoot;
  private string urlPerfix;
  private MimeMapping mimeMap = new();
  private string? indexFile;
  public Static(string urlPerfix, string relativeRoot, string? indexFile = null) {
    this.urlPerfix = urlPerfix;
    this.staticRoot = System.IO.Path.GetFullPath(relativeRoot);
    if (!this.staticRoot.EndsWith("/")) this.staticRoot += "/";
    if (!this.staticRoot.StartsWith("/")) this.staticRoot = "/" + this.staticRoot;
    if (!this.urlPerfix.EndsWith("/")) this.urlPerfix += "/";
    if (!this.urlPerfix.StartsWith("/")) this.urlPerfix = "/" + this.urlPerfix;
    this.indexFile = indexFile;
  }
  public void handle(IContext ctx, NextFunc next) {
    if (ctx.res.status != 200) { next(); return; }
    if (ctx.req.path.StartsWith(this.urlPerfix)) {
      string filePath = System.IO.Path.Join(this.staticRoot, ctx.req.path.Substring(this.urlPerfix.Length));
      if (indexFile != null && System.IO.File.Exists(System.IO.Path.Join(filePath, indexFile))) {
        filePath = System.IO.Path.Join(filePath, indexFile);
      }
      // is exists and is file
      if (System.IO.File.Exists(filePath)) {
        string ext = System.IO.Path.GetExtension(filePath);
        ctx.setHeader("Content-Type", mimeMap[ext] ?? "").setStatus(200);
        ctx.res.body.Write(System.IO.File.ReadAllText(filePath));
        return;
      } else {

        ctx.setStatus(404);
        ctx.resHTML($"<h1>404 Not Found</h1> <p>The requested URL {ctx.req.path} was not found on this server.</p> <hr> <address>Angie</address>");
        return;
      }
    } else {
      next();
    }
  }
}