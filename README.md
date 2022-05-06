# angie-dotnet

一个 Web 后端开发框架 based on .NET，基于中间件机制，提供一个动态 Router。

```cs
var app = new Angie.Application();
var router = new Angie.Router();

app.use(
    router
    .get("/hello", (ctx) => {
        ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
    })
    .post("/hello", (ctx) => {
        Console.WriteLine(ctx.req.body);
        ctx.setStatus(200).resJSON({
            message: "Hello World"
        }); 
    });
).listen(80);
```

## 提供一个动态路由实现

- 参数路由

    ```cs
    router.get("/hello/:name", (ctx) => {
        ctx.setStatus(200).resHTML($"<h1>Hello {ctx.getRouteParam("name") ?? "World"}</h1>");
    });
    ```

- 通配符路由

    ```cs
    router.get("/hello/*", (ctx) => {
        ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
    });
    ```

- 正则路由

    ```cs
    router.get("/hello/^.*glh.*$", (ctx) => {
        ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
    });
    ```

## 基于中间件

```cs
app
.use(router)
.use((ctx, next) => {
    ctx.res.setStatus(404).resHTML("<h1>404 NOT FOUND</h1>");
})
.listen(port);
```

## 应用 Application

### app.listen(...)

在 Angie 实例上调用 `listen` 方法会创建一个 HTTP 应用程序。

```csharp
var app = new Angie.Application();
app.listen(3000);
```

该调用是阻塞的，如果想在服务启动后做一些事情，可以使用回调函数：

```csharp
app.listen(3000, (data, err) => {
  // ...
  Console.WriteLine("Do somethong.");
})
```

### app.use(...)

向 Angie 应用程序注册一个中间件，中间件会按顺序调用。

中间件的每个 `next` 调用将会直接控制下游中间件的行为，直到从某个中间件返回后，调用栈会展开，并且每个中间件恢复执行其上游行为。控制权从调用处同步返回。

例如，在同一个作用域里统计中间件响应时间：

```csharp
app.use((ctx, next) => {
  int now = DateTime.Now.Millisecond;
  next();
  ctx.setHeader("X-Response-Time", (DateTime.Now.Millisecond - now).ToString());
});

app.use((ctx, next) => {
  ctx.resJson(new {
    message = "Hello World!"
  });
});
```

`use()` 调用返回接口实例，所以你可以链式调用：

```csharp
app.use(...).use(...).use(...).listen(3000);
```

## 上下文 Context

每个请求将创建一个 Context，在每个中间件中作为第一个参数。

传统的 Request 和 Response 被封装到这一单个对象中，并提供了一些常用的方法。

### ctx.req

Angie 封装的 request 对象。

### ctx.req.originReq

System.Net.HttpListener 的原始 request 对象。

### ctx.req.method

当次 HTTP 请求的方法。

### ctx.req.path

当次 HTTP 请求的路径。

### ctx.req.body

从 `PUT`、`POST` 类似的请求中解析出的请求体。

### ctx.req.query

从请求路径中带有的 querystring 解析出的哈希表 `Dictionary<string, string>`。

### ctx.req.routeParams

从参数路由（若有）中解析出的路由参数哈希表 `Dictionary<string, string>`。

### ctx.res

Angie 封装的 response 对象。

### ctx.res.header

用于储存向客户端响应的响应头哈希表 `Dictionary<string, string>`。

### ctx.res.status

用于设置响应状态码。

### ctx.res.body

一个 `StreamWriter`，用于向一个临时的缓冲区写响应体，请求处理结束后会写到（如果可能）响应体里。

### ctx.state

`Dictionary<string, object>`，用于通过中间件传递信息，推荐的对象。

### ctx.app

Angie.Application 实例。

### ctx.setStatus(int status)

设置响应报文状态码。

### ctx.setHeader(string key, string value)

设置响应报文的响应头。

### ctx.res*(...)

封装的设置响应体的快捷方法。

- ctx.resJson
- ctx.resString
- ctx.resHTML

### ctx.getQuery(string key)

获取由 querystring 解析而来的键值。

### ctx.getRouteParam(string key)

获取路由参数（当启用动态路由时）。

## 工具 Tools

### Angie.Tools.MimeMapping

用于从文件的扩展名得到 Mime 类型，设置 Content-Type。

```csharp
string? mime = new MimeMapping()[System.IO.Path.GetExtension(filePath)];
ctx.setHeader("Content-Type", mimeMap[ext] ?? "").setStatus(200);
```

## 中间件

### Angie.Middlewares.Static

该中间件封装了静态访问服务。

构造函数签名：

 `public Static(string urlPerfix, string relativeRoot, string? indexFile = null)`

- urlPerfix 请求路径前缀
- relativeRoot 静态资源根目录
- indexFile 默认文件名（可选）

```csharp
app.use(new Static("/public", "./frontend/dist", "index.html")).listen(3000);
```

### Angie.Router

被实现为中间件的动态路由。

```csharp
var router = new Angie.Router();
router.get(...)...;
app.use(router).listen(3000);
```

支持三种路由：

- 参数路由

  ```cs
  router.get("/hello/:name", (ctx) => {
      ctx.setStatus(200).resHTML($"<h1>Hello {ctx.getRouteParam("name") ?? "World"}</h1>");
  });
  ```

- 通配符路由

  ```cs
  router.get("/hello/*", (ctx) => {
      ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
  });
  ```

- 正则路由

  ```cs
  router.get("/hello/^.*glh.*$", (ctx) => {
      ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
  });
  ```

## 示例

```shell
dotnet run .
```

在浏览器中访问 -> [http://localhost/hello](http://localhost/hello)
