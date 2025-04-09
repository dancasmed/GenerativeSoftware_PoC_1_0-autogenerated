using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HotelBookingManager : IGeneratedModule
{
    public string Name { get; set; } = "Hotel Booking Manager";

    private string _dataFolder;
    private List<Room> _rooms;
    private List<Booking> _bookings;
    private const string RoomsFileName = "rooms.json";
    private const string BookingsFileName = "bookings.json";

    public HotelBookingManager()
    {
        _rooms = new List<Room>();
        _bookings = new List<Booking>();
    }

    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        Console.WriteLine("Initializing Hotel Booking Manager...");
        
        LoadData();
        
        if (_rooms.Count == 0)
        {
            InitializeSampleRooms();
            SaveRooms();
        }
        
        Console.WriteLine("Hotel Booking Manager is ready.");
        Console.WriteLine("Sample rooms have been initialized if no existing data was found.");
        
        return true;
    }

    private void LoadData()
    {
        try
        {
            string roomsPath = Path.Combine(_dataFolder, RoomsFileName);
            if (File.Exists(roomsPath))
            {
                string roomsJson = File.ReadAllText(roomsPath);
                _rooms = JsonSerializer.Deserialize<List<Room>>(roomsJson);
            }

            string bookingsPath = Path.Combine(_dataFolder, BookingsFileName);
            if (File.Exists(bookingsPath))
            {
                string bookingsJson = File.ReadAllText(bookingsPath);
                _bookings = JsonSerializer.Deserialize<List<Booking>>(bookingsJson);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data: " + ex.Message);
        }
    }

    private void SaveRooms()
    {
        try
        {
            string roomsPath = Path.Combine(_dataFolder, RoomsFileName);
            string roomsJson = JsonSerializer.Serialize(_rooms);
            File.WriteAllText(roomsPath, roomsJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving rooms: " + ex.Message);
        }
    }

    private void SaveBookings()
    {
        try
        {
            string bookingsPath = Path.Combine(_dataFolder, BookingsFileName);
            string bookingsJson = JsonSerializer.Serialize(_bookings);
            File.WriteAllText(bookingsPath, bookingsJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving bookings: " + ex.Message);
        }
    }

    private void InitializeSampleRooms()
    {
        _rooms.Add(new Room { Id = 1, Type = "Single", PricePerNight = 100, IsAvailable = true });
        _rooms.Add(new Room { Id = 2, Type = "Double", PricePerNight = 150, IsAvailable = true });
        _rooms.Add(new Room { Id = 3, Type = "Suite", PricePerNight = 250, IsAvailable = true });
    }

    public bool BookRoom(int roomId, string guestName, DateTime checkInDate, DateTime checkOutDate)
    {
        var room = _rooms.Find(r => r.Id == roomId);
        if (room == null || !room.IsAvailable)
        {
            Console.WriteLine("Room not available or does not exist.");
            return false;
        }

        var booking = new Booking
        {
            Id = _bookings.Count + 1,
            RoomId = roomId,
            GuestName = guestName,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            TotalPrice = room.PricePerNight * (checkOutDate - checkInDate).Days
        };

        _bookings.Add(booking);
        room.IsAvailable = false;
        
        SaveRooms();
        SaveBookings();
        
        Console.WriteLine("Booking successful!");
        return true;
    }

    public List<Room> GetAvailableRooms()
    {
        return _rooms.FindAll(r => r.IsAvailable);
    }

    public List<Booking> GetAllBookings()
    {
        return _bookings;
    }
}

public class Room
{
    public int Id { get; set; }
    public string Type { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
}

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string GuestName { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
}