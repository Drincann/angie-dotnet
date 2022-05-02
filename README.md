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

## 示例

```shell
dotnet run .
```

在浏览器中访问 -> [http://localhost/hello](http://localhost/hello)
