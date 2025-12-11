using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using examen_final.DTOs;
using examen_final.Models;

namespace examen_final.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly IConfiguration _configuration;

        public AuthService(FirebaseServices firebaseService, IConfiguration configuration)
        {
            _firebaseService = firebaseService;
            _configuration = configuration;
        }
        
        // Register
        public async Task<AuthResponseDto> Register(RegisterDto registerdto)
{
            try
            {
            // 1. Verificar si el usuario ya existe
            var existingUser = await GetUserByEmail(registerdto.Email);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists");
            }

            if (existingUser.numeroidentidad == registerdto.numeroidentidad)
                throw new Exception("Ya existe otro usuario con esta identidad");

            // 2. Generar un ID único
            var userId = Guid.NewGuid().ToString();

            // 3. Hashear contraseña
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerdto.Password);

            // 4. Crear objeto User con tus campos reales
            var user = new User
            {
                Id = userId,
                Email = registerdto.Email,
                nombre = registerdto.nombre,
                apellido = registerdto.apellido,
                edad = registerdto.edad,
                numeroidentidad = registerdto.numeroidentidad,
                telefono = registerdto.telefono,
                Role = "user",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                multa = 0
            };

            // 5. Guardar usuario en Firestore con password hasheado
            var usersCollection = _firebaseService.GetCollection("users");

            var userData = new Dictionary<string, object>()
            {
                {"Id", user.Id},
                {"Email", user.Email},
                {"nombre", user.nombre},
                {"apellido", user.apellido},
                {"edad", user.edad},
                {"numeroidentidad", user.numeroidentidad},
                {"telefono", user.telefono},
                {"Role", user.Role},
                {"CreatedAt", user.CreatedAt},
                {"IsActive", user.IsActive},
                {"multa", user.multa},
                {"PasswordHash", passwordHash}
            };

            await usersCollection.Document(user.Id).SetAsync(userData);

            // 6. Generar token
            var token = GenerateJwtToken(user);

            // 7. Retornar DTO de respuesta
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Nombre = user.nombre,
                Apellido = user.apellido,
                Edad = user.edad,
                NumeroIdentidad = user.numeroidentidad,
                Telefono = user.telefono,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Multa = user.multa
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar usuario: {ex.Message}");
        }
}

        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                var userCollection = _firebaseService.GetCollection("users");
                var query = userCollection.WhereEqualTo("Email", email).Limit(1);
                var snapshot = await query.GetSnapshotAsync();
        
                if (snapshot.Count == 0)
                {
                    return null;
                }
        
                var userDoc = snapshot.Documents[0];
                var userData = userDoc.ToDictionary();
        
                return new User
                {
                    Id = userData.ContainsKey("Id") ? userData["Id"].ToString() : userDoc.Id,
                    Email = userData.ContainsKey("Email") ? userData["Email"].ToString() : string.Empty,
                    nombre = userData.ContainsKey("nombre") ? userData["nombre"].ToString() : string.Empty,
                    apellido = userData.ContainsKey("apellido") ? userData["apellido"].ToString() : string.Empty,
                    edad = userData.ContainsKey("edad") ? Convert.ToInt32(userData["edad"]) : 0,
                    numeroidentidad = userData.ContainsKey("numeroidentidad") ? userData["numeroidentidad"].ToString() : string.Empty,
                    telefono = userData.ContainsKey("telefono") ? userData["telefono"].ToString() : string.Empty,
                    Role = userData.ContainsKey("Role") ? userData["Role"].ToString() : "user",
                    CreatedAt = userData.ContainsKey("CreatedAt")
                        ? ((Timestamp)userData["CreatedAt"]).ToDateTime()
                        : DateTime.UtcNow,
                    IsActive = userData.ContainsKey("IsActive") && Convert.ToBoolean(userData["IsActive"]),
                    multa = userData.ContainsKey("multa") ? Convert.ToDouble(userData["multa"]) : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuario por email: {ex.Message}");
            }
        }

        public string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key no configurado"); 
            var jwtIssuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer no configurado");
            var jwtAudience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience no configurado");
            var jwtExpiryInMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "60");
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.nombre),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiryInMinutes),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Login
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            try
            {
                // 1. Obtener usuario de Firebase por correo
                var userCollection = _firebaseService.GetCollection("users");
                var query = userCollection.WhereEqualTo("Email", loginDto.Email).Limit(1);
                var snapshot = await query.GetSnapshotAsync();
                
                if (snapshot.Count == 0)
                {
                    throw new Exception("Credenciales invalidas");
                }
                
                var userDoc = snapshot.Documents[0];
                
                // 2. Extraer campos manualmente
                var userData = userDoc.ToDictionary();
                
                // Verificar que SI existe PasswordHash
                if (!userData.ContainsKey("PasswordHash"))
                {
                    throw new Exception("Usuario sin contraseña configurada");
                }
                
                var passwordHash = userData["PasswordHash"].ToString();
                
                // 3. Crear el objeto User
                var user = new User
                {
                    Id = userData.ContainsKey("Id") ? userData["Id"].ToString() : userDoc.Id,
                    Email = userData.ContainsKey("Email") ? userData["Email"].ToString() : string.Empty,
                    nombre = userData.ContainsKey("nombre") ? userData["nombre"].ToString() : string.Empty,
                    apellido = userData.ContainsKey("apellido") ? userData["apellido"].ToString() : string.Empty,
                    edad = userData.ContainsKey("edad") ? Convert.ToInt32(userData["edad"]) : 0,
                    numeroidentidad = userData.ContainsKey("numeroidentidad") ? userData["numeroidentidad"].ToString() : string.Empty,
                    telefono = userData.ContainsKey("telefono") ? userData["telefono"].ToString() : string.Empty,
                    Role = userData.ContainsKey("Role") ? userData["Role"].ToString() : "user",
                    CreatedAt = userData.ContainsKey("CreatedAt")
                        ? ((Timestamp)userData["CreatedAt"]).ToDateTime()
                        : DateTime.UtcNow,
                    IsActive = userData.ContainsKey("IsActive") && Convert.ToBoolean(userData["IsActive"]),
                    multa = userData.ContainsKey("multa") ? Convert.ToDouble(userData["multa"]) : 0
                };
                
                // 4. Verificar contraseña
                if (string.IsNullOrEmpty(passwordHash) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, passwordHash))
                {
                    throw new Exception("Credenciales invalidas");
                }
                
                // 5. Verificar que el usuario este activo
                if (!user.IsActive)
                {
                    throw new Exception("Usuario inactivo");
                }
                
                // 6. Generar token JWT
                var token = GenerateJwtToken(user);
                
                // 7. Retornar respuesta
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Nombre = user.nombre,
                    Apellido = user.apellido,
                    Edad = user.edad,
                    NumeroIdentidad = user.numeroidentidad,
                    Telefono = user.telefono,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Multa = user.multa
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al iniciar sesion: {ex.Message}");
            }
        }

        public async Task<User?> GetUserById(string userId)
        {
            try
            {
                       var userDoc = await _firebaseService
                            .GetCollection("users")
                            .Document(userId)
                            .GetSnapshotAsync();
                        
                if (!userDoc.Exists)
                {
                    return null;
                }
                
                
                var userData = userDoc.ToDictionary();
                
                
                        return new User
                        {
                            Id = userData.ContainsKey("Id") ? userData["Id"].ToString() : userDoc.Id,
                            Email = userData.ContainsKey("Email") ? userData["Email"].ToString() : string.Empty,
                            nombre = userData.ContainsKey("nombre") ? userData["nombre"].ToString() : string.Empty,
                            apellido = userData.ContainsKey("apellido") ? userData["apellido"].ToString() : string.Empty,
                            edad = userData.ContainsKey("edad") ? Convert.ToInt32(userData["edad"]) : 0,
                            numeroidentidad = userData.ContainsKey("numeroidentidad") ? userData["numeroidentidad"].ToString() : string.Empty,
                            telefono = userData.ContainsKey("telefono") ? userData["telefono"].ToString() : string.Empty,
                            Role = userData.ContainsKey("Role") ? userData["Role"].ToString() : "user",
                            CreatedAt = userData.ContainsKey("CreatedAt")
                                ? ((Timestamp)userData["CreatedAt"]).ToDateTime()
                                : DateTime.UtcNow,
                            IsActive = userData.ContainsKey("IsActive") && Convert.ToBoolean(userData["IsActive"]),
                            multa = userData.ContainsKey("multa") ? Convert.ToDouble(userData["multa"]) : 0
                        };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}