<p><b>YouTubev3DotNet</b></p>

<p>The current project consumes YouTube API v3 using C# .NET</p>

<p><b>The purpose of this API</b></p>

<p>The purpose is to provide a tool to other developers that are interested on consuming YouTube data. Specially for those who use .NET.</p>

<p><b>IMPORTANT: About API Key</b></p>

<p>It's important to take into account that API key requieres server IP to be registered. So, when using in local environments, the developer's public IP must be registered in the API key. This registration can be performed in Developers Console. For more information visit the following <a target="_blank" href="https://developers.google.com/youtube/2.0/deprecation_faq">link</a>.</p> In this example I provide my own certificate and my own credentials, but they are only allowed for my IP. Try registering your account and changing the certificate and credentials with your own.

<p>In v3 exists a global quota pool, which is of 50 millon units per day. Each call to the API consumes a certain amount of quota, depending on the operation.</p>

<p>It's also important to mention that API allows requests using and API Key or OAuth2.0. In the current project, you can find an example for each case.</p>
