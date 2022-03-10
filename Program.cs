var app = new Angie.Application();

app.get("/hello", (ctx) => {
    ctx.setStatus(200).resHTML("<h1>Hello World</h1>");
}).listen(80);
