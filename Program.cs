using System.Net.Sockets;
using System.Net;


using Angie;


var app = new Angie.Application();

app.get("/good", (ctx) => {
    ctx.res.status = 200;
    ctx.res.body.Write("Hello World!");
}).listen(80);
// listener.AcceptSocketAsync();

