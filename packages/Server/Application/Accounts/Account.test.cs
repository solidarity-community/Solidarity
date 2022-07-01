namespace Solidarity.Application.Accounts;

public class AccountTest
{
	[Theory(DisplayName = "Should lowercase the username in setter")]
	[InlineData("Username", "username")]
	public void TestUsername(string username, string expected)
	{
		Account account = new() { Username = username };
		Assert.Equal(expected, account.Username);
	}
}