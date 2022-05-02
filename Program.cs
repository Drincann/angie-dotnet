var app = new Angie.Application();
var router = new Angie.Router();

app.use(

  router.get("/hello/:name", (ctx) => {
    ctx.setStatus(200).resHTML($"<h1>Hello {ctx.getRouteParam("name")}</h1>");
  }).post("/hello", (ctx) => {
    Console.WriteLine(ctx.req.body);
    ctx.resJson(new {
      message = "Hello World"
    });
  })

).use((ctx, next) => {
  ctx.setStatus(404).resHTML("<h1> 404 Not Found </h1>");
}).listen(80);
