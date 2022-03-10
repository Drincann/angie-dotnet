# angie-dotnet

一个 Web 后端开发框架 based on .NET。

```cs
var app = new Angie.Application();

app.get("/hello", (ctx) => {
    ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
}).listen(80);
```

## 示例

```shell
dotnet run .
```

在浏览器中访问 -> [http://localhost/hello](http://localhost/hello)
