# dotnet-test-urlrewrite

A simple dotnet web application for testing URL rewrite rules.

This sample application will accept any URL and will return a JSON object with the request properties and a 200 reponse.

More importantly, you can add rules in `urlrewrite.xml` and test the responses. The aim is to

1. Make sure all redirects work
1. Make sure that a _few_ redirects as possible happen.
