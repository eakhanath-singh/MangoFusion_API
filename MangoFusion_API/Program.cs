using MangoFusion_API.Data;
using MangoFusion_API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Adding Db Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Adding identity services
// Adding EntityFrameWorkStores , to use the identity services for Application DB context
// Adding Applciation User as identity USer
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
// Add services to the container.

builder.Services.AddControllers();

// Getting the secert key
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:Secret");
// Adding Authentication configuration here
builder.Services.AddAuthentication(authConfig =>
{
    // Adding Deafult Authentication and Challenge Scheme as JWT Bearer Defaults
    authConfig.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authConfig.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtConfig =>
{
    // adding Add JWT Bearer token configuration, Required HTTP is for Http request check, Save Token for scope
    jwtConfig.RequireHttpsMetadata = false;
    jwtConfig.SaveToken = true;
    // Adding validation parameters of any JWT token 
    jwtConfig.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // to check the signing request
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // encrypt the secret key
        ValidateIssuer = false,// related to check the issuer or get the token form specific issuer
        ValidateAudience = false, // as same as validate issuer
        ValidateLifetime = true // determine the scope of the key
    };
});

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Adding BearerSecuritySchemeTransformer in openAPi service call to make the bearer token configure
builder.Services.AddOpenApi(options=>
{
    // this way we can add the bear security scheme transformer document pnly above .net 8 this will work
    //options.AddDocumentTransformer<BearerSecuritySchemeTransformer>()
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

// adding commands to use www root folder and image globally and this are avaliable in web application as we are using API Core we need to add these
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
// after adding JWT configuration we add this below line.

app.UseCors(o=>o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*"));
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// New internal class which has BearerSecuritySchemeTransformer for set configuration for Bearer Token in openAPI or Scalar
/// </summary>
/// <param name="authenticationSchemeProvider"></param>
internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer (IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        this.authenticationSchemeProvider = authenticationSchemeProvider;
    }


    /// <summary>
    /// Class based on Iopen Api document Transform and Authentication Scheme
    /// </summary>
    /// <param name="document"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticateSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if(authenticateSchemes.Any(authSchemes => authSchemes.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();

            //var requirement = new Dictionary<string, OpenApiSecurityScheme>
            //{
            //    [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            //    {
            //        Type = SecuritySchemeType.Http,
            //        Scheme = "bearer",
            //        In = ParameterLocation.Header,
            //        BearerFormat = "JWT",
            //    }
            //};
            document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
            };
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
        document.Info = new OpenApiInfo()
        {
            Title = "MangoFusion_API",
            Version = "v1",
            Description = "A Sample Example Asp .Net Core"
        };
    }
}