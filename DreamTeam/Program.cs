using System;
using DreamTeam.Models;
using DreamTeam.Services;

namespace DreamTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            DreamTeamService service;
            var serviceType = Environment.GetEnvironmentVariable("SERVICE_TYPE");

            switch (serviceType)
            {
                case "HRDirector":
                    service = new HRDirectorService();
                    break;

                case "HRManager":
                    service = new HRManagerService();
                    break;

                case "Participant":
                    int participantId = int.Parse(Environment.GetEnvironmentVariable("PARTICIPANT_ID")
                        ?? throw new InvalidOperationException("PARTICIPANT_NAME environment variable is not set."));
                    string participantName = Environment.GetEnvironmentVariable("PARTICIPANT_NAME")
                        ?? throw new InvalidOperationException("PARTICIPANT_NAME environment variable is not set.");
                    string roleString = Environment.GetEnvironmentVariable("PARTICIPANT_ROLE")
                        ?? throw new InvalidOperationException("PARTICIPANT_ROLE environment variable is not set.");

                    EmployeeRole role = roleString == "TeamLead" ? EmployeeRole.TeamLead : EmployeeRole.Junior;

                    var participant = new Participant(participantId, participantName, role);
                    service = new ParticipantService(participant);
                    break;

                default:
                    throw new Exception("Неверный тип сервиса. Укажите SERVICE_TYPE: HRDirector, HRManager или Participant.");
            }

            service.Start();
            Console.WriteLine($"Сервис {service} запущен.");
        }
    }
}
