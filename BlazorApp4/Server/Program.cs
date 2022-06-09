using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews(configure =>
{
    configure.Conventions.Insert(0, new MyConvention());
});
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();


public class MyConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach(var controller in application.Controllers)
        {
            foreach(var action in controller.Actions)
            {
                string? verb = action.ActionName switch
                {
                    var x when x.StartsWith("Post") => "POST",
                    var x when x.StartsWith("Get") => "GET",
                    var x when x.StartsWith("Delete") => "DELETE",
                    _ => default
                };
                if (verb is not null)
                {
                    action.Selectors.First().ActionConstraints.Add(new HttpMethodActionConstraint(new[]{ verb }));
                }

                if (action.Parameters.Any())
                {
                    switch (verb)
                    {
                        case "DELETE":
                        case "GET":
                            action.Parameters.First().BindingInfo = new BindingInfo
                            {
                                BindingSource = BindingSource.Query
                            };
                            break;
                    }
                }

            }
        }
    }
}