using Firebase.Storage;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhotoGallery.Common;
using PhotoGallery.Entities;
using PhotoGallery.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

JWTConfiguration jwtConfiguration = builder.Configuration.GetSection(PGConstants.JWT).Get<JWTConfiguration>()
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(jwtConfiguration)}");
string issuer = jwtConfiguration.Issuer
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(issuer)}");
string audience = jwtConfiguration.Audience
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(audience)}");
string secretKey = jwtConfiguration.SecretKey
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(secretKey)}");

FirebaseConfiguration firebaseConfiguration = builder.Configuration.GetSection(PGConstants.Firebase).Get<FirebaseConfiguration>()
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(firebaseConfiguration)}");
string firebaseConfigPath = firebaseConfiguration.FirebaseConfigPath
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(firebaseConfigPath)}");
string projectId = firebaseConfiguration.ProjectId
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(projectId)}");
string bucketAddress = firebaseConfiguration.BucketAddress
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(bucketAddress)}");

string cors = builder.Configuration.GetSection(PGConstants.CORS).Get<string>()
    ?? throw new NullReferenceException($"{nameof(Program)}:{nameof(cors)}");

Environment.SetEnvironmentVariable(PGConstants.GoogleApplicationCredentialsEnvironementVariable, firebaseConfigPath);

FirestoreDb firestoreDb = FirestoreDb.Create(projectId);
IImageService imageService = new ImageService(new FirebaseStorage(bucketAddress));
builder.Services.AddSingleton<IAuthenticationService>(s => new AuthenticationService(firestoreDb));
builder.Services.AddSingleton(imageService);
builder.Services.AddSingleton<IPostService>(s => new PostService(firestoreDb, imageService));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});


builder.Services.AddTransient<IJWTManager, JWTManager>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(cors)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
