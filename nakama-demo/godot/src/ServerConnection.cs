using Godot;
using System;
using System.Net;
using System.Threading.Tasks;
using Nakama;

public partial class ServerConnection : Node
{
	private const string Key = "defaultkey";
	
	[Signal]
	public delegate void AuthenticationResultEventHandler(long statusCode);
	
	private ISession _session;
	private readonly Client _client = new("http", "127.0.0.1", 7350, Key);

	public async void AuthenticateAsync(string email, string password)
	{
		try
		{
			_session = await _client.AuthenticateEmailAsync(email, password);
			GD.Print($"Session: {_session}");
			EmitSignal(SignalName.AuthenticationResult, (long)HttpStatusCode.OK);
		}
		catch (ApiResponseException e)
		{
			EmitSignal(SignalName.AuthenticationResult, e.StatusCode);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Unexpected error: {e}");
			EmitSignal(SignalName.AuthenticationResult, (long)HttpStatusCode.InternalServerError);
		}
	}
}
