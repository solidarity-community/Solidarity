namespace Solidarity.Infrastructure.Payment.Bitcoin;

public record UTxO(Key PrivateKey, UnspentCoin Coin);