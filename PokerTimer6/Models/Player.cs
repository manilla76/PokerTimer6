using PokerTimer6.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

public class Player : IComparable<Player>, IEquatable<Player>, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private uint _id;
    public uint id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _name;
    [Required]
    [StringLength(40, ErrorMessage = "Name is too long.")]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    private Seat _playerSeat = new();
    public Seat Player_Seat
    {
        get => _playerSeat;
        set => SetProperty(ref _playerSeat, value);
    }

    private int _table;
    public int Table
    {
        get => _table;
        set => SetProperty(ref _table, value);
    }

    private int _seat;
    public int Seat
    {
        get => _seat;
        set => SetProperty(ref _seat, value);
    }

    public Player() { }

    public Player(uint ID, string name)
    {
        _id = ID;
        _name = name;
    }

    public void AssignSeat(Seat seat)
    {
        Player_Seat = seat;
    }

    public override string ToString() => Name;

    public int CompareTo(Player? other) => Name.CompareTo(other?.Name);

    public bool Equals(Player? other) => id == other?.id;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
