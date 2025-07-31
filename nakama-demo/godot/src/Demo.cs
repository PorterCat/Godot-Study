using Godot;
using System;
using System.Net;

public partial class Demo : Node
{
	private ServerConnection _connection;
	
	public override void _Ready()
	{
		_connection = GetNodeOrNull<ServerConnection>("ServerConnection");
		
		var email = "test@test.com";
		var password = "password";
		_connection.AuthenticateAsync(email, password);
	}
	
	private void OnAuthenticationResult(long statusCode)
	{
		GD.Print($"Result: {statusCode}");
		if (statusCode == (long)HttpStatusCode.OK)
			GD.Print("Authentication successful!");
		else
			GD.PrintErr($"Authentication failed (Code: {statusCode})");
	}
}
