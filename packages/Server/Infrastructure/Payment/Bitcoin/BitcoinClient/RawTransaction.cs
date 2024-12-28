namespace Solidarity.Infrastructure.Payment.Bitcoin;

#nullable disable

public sealed class RawTransaction
{
	public string txid { get; set; }
	public string hash { get; set; }
	public int version { get; set; }
	public int size { get; set; }
	public int vsize { get; set; }
	public int weight { get; set; }
	public int locktime { get; set; }
	public List<Vin> vin { get; set; }
	public List<Vout> vout { get; set; }
	public string hex { get; set; }
	public string blockhash { get; set; }
	public int confirmations { get; set; }
	public int time { get; set; }
	public int blocktime { get; set; }
}

public sealed class ScriptPubKey
{
	public string asm { get; set; }
	public string hex { get; set; }
	public string address { get; set; }
	public string type { get; set; }
}

public sealed class ScriptSig
{
	public string asm { get; set; }
	public string hex { get; set; }
}

public sealed class Vin
{
	public string txid { get; set; }
	public uint vout { get; set; }
	public ScriptSig scriptSig { get; set; }
	public List<string> txinwitness { get; set; }
	public object sequence { get; set; }
}

public sealed class Vout
{
	public decimal value { get; set; }
	public int n { get; set; }
	public ScriptPubKey scriptPubKey { get; set; }
}
