﻿/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityAdmin.Configuration;

namespace IdentityAdmin.Assets
{
    class EmbeddedHtmlResult : IHttpActionResult
    {
        string path;
        string file;
        string authorization_endpoint;
        AdminSecurityConfiguration _adminSecurityConfiguration;

        public EmbeddedHtmlResult(HttpRequestMessage request, string file, AdminSecurityConfiguration securityConfiguration)
        {
            var pathbase = request.GetOwinContext().Request.PathBase;
            this.path = pathbase.Value;
            this.file = file;
            this.authorization_endpoint = pathbase + Constants.AuthorizePath;
            this._adminSecurityConfiguration = securityConfiguration;
        }

        public Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(GetResponseMessage());
        }

        public HttpResponseMessage GetResponseMessage()
        {
            var html = AssetManager.LoadResourceString(this.file,
                new {
                    pathBase = this.path,
                    model = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        PathBase = this.path,
                        ShowLoginButton = this._adminSecurityConfiguration.ShowLoginButton,
                        oauthSettings = new
                        {
                            authorization_endpoint = this.authorization_endpoint,
                            client_id = Constants.IdAdmMgrClientId
                        }
                    })
                });

            return new HttpResponseMessage()
            {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };
        }
    }
}
