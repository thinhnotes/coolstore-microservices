using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal static class OpenApiRouteBuilderExtensions
{
    /// <summary>
    ///  Helper method to render Swagger UI view for testing.
    /// </summary>
    public static IEndpointRouteBuilder MapSwaggerUI(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/swagger/{resourceName}/{documentName}", (string resourceName, string documentName) => Results.Content($$"""
    <html>
    <head>
        <meta charset="UTF-8">
        <title>OpenAPI -{{resourceName}}- {{documentName}}</title>
        <link rel="stylesheet" type="text/css" href="https://unpkg.com/swagger-ui-dist/swagger-ui.css">
    </head>
    <body>
        <div id="swagger-ui"></div>

        <script src="https://unpkg.com/swagger-ui-dist/swagger-ui-standalone-preset.js"></script>
        <script src="https://unpkg.com/swagger-ui-dist/swagger-ui-bundle.js"></script>

        <script>
            window.onload = function() {
                const ui = SwaggerUIBundle({
                oauth2RedirectUrl: window.location.origin + "/swagger/{{resourceName}}/{{documentName}}/oauth2-redirect.html",
                url: "/openapi/{{resourceName}}/{{documentName}}.json",
                    dom_id: '#swagger-ui',
                    deepLinking: true,
                    presets: [
                        SwaggerUIBundle.presets.apis,
                        SwaggerUIStandalonePreset
                    ],
                    plugins: [
                        SwaggerUIBundle.plugins.DownloadUrl
                    ],
                    layout: "StandaloneLayout",
                })
                ui.initOAuth({
                  clientId: "{{resourceName}}"
                })
                window.ui = ui
            }
        </script>
    </body>
    </html>
    """, "text/html"));

        endpoints.MapGet("/swagger/{resourceName}/{documentName}/oauth2-redirect.html", (string resourceName, string documentName) => Results.Content($$"""
    <!doctype html>
    <html lang="en-US">
    <head>
        <title>Swagger UI: OAuth2 Redirect</title>
    </head>
    <body>
    <script>
        'use strict';
        function run () {
            var oauth2 = window.opener.swaggerUIRedirectOauth2;
            var sentState = oauth2.state;
            var redirectUrl = oauth2.redirectUrl;
            var isValid, qp, arr;

            if (/code|token|error/.test(window.location.hash)) {
                qp = window.location.hash.substring(1).replace('?', '&');
            } else {
                qp = location.search.substring(1);
            }

            arr = qp.split("&");
            arr.forEach(function (v,i,_arr) { _arr[i] = '"' + v.replace('=', '":"') + '"';});
            qp = qp ? JSON.parse('{' + arr.join() + '}',
                    function (key, value) {
                        return key === "" ? value : decodeURIComponent(value);
                    }
            ) : {};

            isValid = qp.state === sentState;

            if ((
              oauth2.auth.schema.get("flow") === "accessCode" ||
              oauth2.auth.schema.get("flow") === "authorizationCode" ||
              oauth2.auth.schema.get("flow") === "authorization_code"
            ) && !oauth2.auth.code) {
                if (!isValid) {
                    oauth2.errCb({
                        authId: oauth2.auth.name,
                        source: "auth",
                        level: "warning",
                        message: "Authorization may be unsafe, passed state was changed in server. The passed state wasn't returned from auth server."
                    });
                }

                if (qp.code) {
                    delete oauth2.state;
                    oauth2.auth.code = qp.code;
                    oauth2.callback({auth: oauth2.auth, redirectUrl: redirectUrl});
                } else {
                    let oauthErrorMsg;
                    if (qp.error) {
                        oauthErrorMsg = "["+qp.error+"]: " +
                            (qp.error_description ? qp.error_description+ ". " : "no accessCode received from the server. ") +
                            (qp.error_uri ? "More info: "+qp.error_uri : "");
                    }

                    oauth2.errCb({
                        authId: oauth2.auth.name,
                        source: "auth",
                        level: "error",
                        message: oauthErrorMsg || "[Authorization failed]: no accessCode received from the server."
                    });
                }
            } else {
                oauth2.callback({auth: oauth2.auth, token: qp, isValid: isValid, redirectUrl: redirectUrl});
            }
            window.close();
        }

        if (document.readyState !== 'loading') {
            run();
        } else {
            document.addEventListener('DOMContentLoaded', function () {
                run();
            });
        }
    </script>
    </body>
    </html>
    """, "text/html"));

        return endpoints;
    }
}
