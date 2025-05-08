
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // Add this using statement
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Repository.Models;
using Repository.Mapper;
using Service.IServices;
using Service.Service;
using Service.IService;
using Repository.Repositories;
using Repository.DBContext;
using Service.SignalR.Server; // Assuming your SWP_KahootContext is in this namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SWP_KahootContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Replace "DefaultConnection" with your actual connection string name

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding
                .UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PRN231", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Please enter a valid JWT token",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("*")  // Allows any origin
               .AllowAnyMethod()
               .AllowAnyHeader();
        //.AllowCredentials(); // Uncomment if you need to handle credentials (e.g., cookies)
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
// Register Repository
builder.Services.AddScoped<QuizRepository>();
builder.Services.AddScoped<QuestionRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PlayerAnswerRepository>();
builder.Services.AddScoped<GroupRepository>();
builder.Services.AddScoped<ServicePackRepository>();
builder.Services.AddScoped<UserServicePackRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<GroupMemberRepository>();
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserMapper)); // Register your mapper profile

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IPlayerAnswerService, PlayerAnswerService>();
builder.Services.AddScoped<IServicePackService, ServicePackService>();
builder.Services.AddScoped<IUserServicePackService, UserServicePackService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ScoreCalculatorService>();
// Register JWTService with Scoped lifetime
builder.Services.AddScoped<IJWTService, JWTService>(); //  Corrected registration
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICommonService, CommonService>();


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer(); // Already added above
// builder.Services.AddSwaggerGen(); // Already added above

//SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("MyPolicy"); // Enable CORS policy

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapHub<KahootSignalR>("/KahootSignalRServer");

app.Run();
