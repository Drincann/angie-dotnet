# angie-dotnet

一个 Web 后端开发框架 based on .NET。

```cs
var app = new Angie.Application();

app.get("/hello", (ctx) => {
    ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
}).listen(80);
```

支持动态路由

- 参数路由

    ```cs
    app.get("/hello/:name", (ctx) => {
        ctx.setStatus(200).resHTML($"<h1>Hello {ctx.getRouteParam("name") ?? "World"}</h1>");
    });
    ```

- 通配符路由

    ```cs
    app.get("/hello/*", (ctx) => {
        ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
    });
    ```

- 正则路由

    ```cs
    app.get("/hello/^.*glh.*$", (ctx) => {
        ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
    });
    ```

## 示例

```shell
dotnet run .
```

在浏览器中访问 -> [http://localhost/hello](http://localhost/hello)
