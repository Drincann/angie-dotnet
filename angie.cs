// map
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;

namespace Angie;



public class Application : IApplication {
  private Router router;
  private IHTTPServer underlyingServer;

  public Application() {
    this.router = new Router();
    this.underlyingServer = new HTTPListenerHTTPServer();
  }

  public IApplication route(HTTPMethod method, string path, Handler handler) {
    this.router.route(method, path, handler);
    return this;
  }

  public IApplication get(string path, Handler handler) {
    this.router.route(HTTPMethod.GET, path, handler);
    return this;
  }

  public IApplication post(string path, Handler handler) {
    this.router.route(HTTPMethod.POST, path, handler);
    return this;
  }

  public IApplication put(string path, Handler handler) {
    this.router.route(HTTPMethod.PUT, path, handler);
    return this;
  }

  public IApplication delete(string path, Handler handler) {
    this.router.route(HTTPMethod.DELETE, path, handler);
    return this;
  }

  public void listen(int port) {
    this.listen(port, null);
  }
  public void listen(int port, Callback<int?>? callback) {
    this.underlyingServer.addHandler(this.router.handle);
    this.underlyingServer.listen(port, callback);
  }
}