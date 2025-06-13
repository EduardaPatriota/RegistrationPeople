using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ApiResponseSchemaFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (statusCode, response) in operation.Responses)
        {
            
            var mediaType = response.Content
                               .FirstOrDefault(c => c.Key.Contains("json")).Value;
            if (mediaType == null) continue;

            switch (statusCode)
            {
                case "200":
                    mediaType.Example = new OpenApiObject
                    {
                      
                        ["data"] = new OpenApiString("token_exemplo"),
                        ["errors"] = new OpenApiArray()
                    };
                    break;

                case "404":
                    mediaType.Example = new OpenApiObject
                    {
                       
                        ["data"] = new OpenApiNull(),
                        ["errors"] = new OpenApiArray
                        {
                            new OpenApiString("Usuário não encontrado")
                        }
                    };
                    break;

                case "400":
                    mediaType.Example = new OpenApiObject
                    {
                            
                        ["data"] = new OpenApiNull(),
                        ["errors"] = new OpenApiArray
                        {
                            new OpenApiString("Campo obrigatorio: email"),
                            new OpenApiString("Senha deve ter uint 6 caracteres")
                        }
                    };
                    break;

                case "500":
                    mediaType.Example = new OpenApiObject
                    {
                        ["data"] = new OpenApiNull(),
                        ["errors"] = new OpenApiArray
                        {
                            new OpenApiString("Erro interno: Detalhes do erro aqui")
                        }
                    };
                    break;

                case "201":
                    mediaType.Example = new OpenApiObject
                    {
                        
                        ["data"] = new OpenApiString("Pessoa criada com sucesso"),
                        ["errors"] = new OpenApiArray()
                    };
                    break;

                case "401":
                    mediaType.Example = new OpenApiObject
                    {
                        
                        ["data"] = new OpenApiNull(),
                        ["errors"] = new OpenApiArray
                        {
                            new OpenApiString("Unauthorized access")
                        }
                    };
                    break;
            }
        }
    }
}
