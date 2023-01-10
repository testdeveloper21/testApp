using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using System.Web.UI.WebControls;

namespace PTRC.API.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        /// <summary>
        /// Apply custom operation.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiExplorer">The api explorer.</param>
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/token", new PathItem
            {
                post = new Operation
                {
                    operationId = "Auth_AccessToken",
                    tags = new List<string> { "Auth" },
                    produces = new List<string>
                {
                    "application/json",
                    "text/json",
                    "application/xml",
                    "text/xml"
                },
                    consumes = new List<string>
                {
                    "application/x-www-form-urlencoded"
                },
                    parameters = new List<Swashbuckle.Swagger.Parameter>
                {
                    new Swashbuckle.Swagger.Parameter
                    {
                        type = "string",
                        name = "grant_type",
                        required = true,
                        @in = "formData",
                        @default ="password"
                    },
                    new Swashbuckle.Swagger.Parameter
                    {
                        type = "string",
                        name = "username",
                        required = true,
                        @in = "formData"
                    },
                    new Swashbuckle.Swagger.Parameter
                    {
                        type = "string",
                        name = "password",
                        required = true,
                        @in = "formData"
                    }
                },
                    responses = new Dictionary<string, Response>
                {
                    {"200", new Response{ description = "OK", schema = new Schema{ type = "object"} } }
                }
                }
            });
        }
    }

}