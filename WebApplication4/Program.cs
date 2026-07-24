var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    var request = context.Request;
    var response = context.Response;

    if (request.Path == "/api/order")
    {
        if (request.Method != "POST")
        {
            response.StatusCode = 405;
            await response.WriteAsJsonAsync(new { text = "Wrong Method" });
        }
        else if (!request.HasJsonContentType())
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { text = "Expected a Json File"});
        }
        else {
            try
            {
                var order = await request.ReadFromJsonAsync<Order>();
                
                    if (order.CustomerName == null)
                    {
                        response.StatusCode = 400;
                        await response.WriteAsJsonAsync(new { text = "The \"Name\" field cannot be empty" });
                    }
                    else if (order.Items == null || order.Items.Length == 0)
                    {
                        response.StatusCode = 400;
                        await response.WriteAsJsonAsync(new { text = "Order must contain at least 1 element" });
                    }
                    else if (order.Items.Any(item=> item.Quantity <=0.0))
                    {
                        response.StatusCode = 400;
                        await response.WriteAsJsonAsync(new { text = "The \"Quantity\" must be positive" });
                    }
                    else {
                        
                        response.StatusCode = 200;
                        await response.WriteAsJsonAsync(new { text = "Order is accepted!"});
                    }
            }
            catch {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { text = "Invalid data format"});
            }
        
        }
    }
    else {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }

});
app.Run();

public record Dish(string Name, int Quantity, double Price);
public record Order(string CustomerName, Dish[] Items);
