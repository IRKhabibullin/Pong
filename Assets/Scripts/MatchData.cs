using System;

[Serializable]
public class MatchData : IEquatable<MatchData>
{
    // Technical info
    public string ipAddress;
    public int port;
    public readonly Guid gameId;
    
    public string name;
    public string mode;

    public MatchData(string ipAddress, int port, Guid gameId, string name, string mode)
    {
        this.ipAddress = ipAddress;
        this.port = port;
        this.gameId = gameId;
        this.name = name;
        this.mode = mode;
    }

    public MatchData(Guid gameId, string name, string mode)
    {
        this.gameId = gameId;
        this.name = name;
        this.mode = mode;
    }

    public bool Equals(MatchData other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return gameId.Equals(other.gameId);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MatchData)obj);
    }

    public override int GetHashCode()
    {
        return gameId.GetHashCode();
    }
}