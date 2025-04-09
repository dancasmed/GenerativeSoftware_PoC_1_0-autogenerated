using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HospitalManagementModule : IGeneratedModule
{
    public string Name { get; set; } = "Hospital Management System";

    private string patientsFilePath;
    private string appointmentsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Hospital Management System...");

        patientsFilePath = Path.Combine(dataFolder, "patients.json");
        appointmentsFilePath = Path.Combine(dataFolder, "appointments.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        if (!File.Exists(patientsFilePath))
        {
            File.WriteAllText(patientsFilePath, "[]");
        }

        if (!File.Exists(appointmentsFilePath))
        {
            File.WriteAllText(appointmentsFilePath, "[]");
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nHospital Management System");
            Console.WriteLine("1. Add Patient");
            Console.WriteLine("2. View Patients");
            Console.WriteLine("3. Schedule Appointment");
            Console.WriteLine("4. View Appointments");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddPatient();
                    break;
                case "2":
                    ViewPatients();
                    break;
                case "3":
                    ScheduleAppointment();
                    break;
                case "4":
                    ViewAppointments();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Hospital Management System is shutting down...");
        return true;
    }

    private void AddPatient()
    {
        Console.Write("Enter patient ID: ");
        string id = Console.ReadLine();

        Console.Write("Enter patient name: ");
        string name = Console.ReadLine();

        Console.Write("Enter patient age: ");
        string age = Console.ReadLine();

        Console.Write("Enter patient gender: ");
        string gender = Console.ReadLine();

        var patient = new Patient
        {
            Id = id,
            Name = name,
            Age = age,
            Gender = gender
        };

        var patients = LoadPatients();
        patients.Add(patient);
        SavePatients(patients);

        Console.WriteLine("Patient added successfully.");
    }

    private void ViewPatients()
    {
        var patients = LoadPatients();
        Console.WriteLine("\nPatient List:");
        foreach (var patient in patients)
        {
            Console.WriteLine("ID: " + patient.Id + ", Name: " + patient.Name + ", Age: " + patient.Age + ", Gender: " + patient.Gender);
        }
    }

    private void ScheduleAppointment()
    {
        Console.Write("Enter appointment ID: ");
        string id = Console.ReadLine();

        Console.Write("Enter patient ID: ");
        string patientId = Console.ReadLine();

        Console.Write("Enter doctor name: ");
        string doctor = Console.ReadLine();

        Console.Write("Enter appointment date (YYYY-MM-DD): ");
        string date = Console.ReadLine();

        Console.Write("Enter appointment time: ");
        string time = Console.ReadLine();

        var appointment = new Appointment
        {
            Id = id,
            PatientId = patientId,
            Doctor = doctor,
            Date = date,
            Time = time
        };

        var appointments = LoadAppointments();
        appointments.Add(appointment);
        SaveAppointments(appointments);

        Console.WriteLine("Appointment scheduled successfully.");
    }

    private void ViewAppointments()
    {
        var appointments = LoadAppointments();
        Console.WriteLine("\nAppointment List:");
        foreach (var appointment in appointments)
        {
            Console.WriteLine("ID: " + appointment.Id + ", Patient ID: " + appointment.PatientId + ", Doctor: " + appointment.Doctor + ", Date: " + appointment.Date + ", Time: " + appointment.Time);
        }
    }

    private List<Patient> LoadPatients()
    {
        string json = File.ReadAllText(patientsFilePath);
        return JsonSerializer.Deserialize<List<Patient>>(json);
    }

    private void SavePatients(List<Patient> patients)
    {
        string json = JsonSerializer.Serialize(patients);
        File.WriteAllText(patientsFilePath, json);
    }

    private List<Appointment> LoadAppointments()
    {
        string json = File.ReadAllText(appointmentsFilePath);
        return JsonSerializer.Deserialize<List<Appointment>>(json);
    }

    private void SaveAppointments(List<Appointment> appointments)
    {
        string json = JsonSerializer.Serialize(appointments);
        File.WriteAllText(appointmentsFilePath, json);
    }
}

public class Patient
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
}

public class Appointment
{
    public string Id { get; set; }
    public string PatientId { get; set; }
    public string Doctor { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
}