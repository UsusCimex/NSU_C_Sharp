using System;
using DreamTeam.Models;

namespace DreamTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceType = Environment.GetEnvironmentVariable("SERVICE_TYPE") ?? "";

            switch (serviceType)
            {
                case "HRDirector":
                    var hrDirectorService = new HRDirectorService();
                    hrDirectorService.Start();
                    break;

                case "HRManager":
                    var hrManagerService = new HRManagerService();
                    hrManagerService.Start();
                    break;

                case "Participant":
                    int participantId = int.Parse(Environment.GetEnvironmentVariable("PARTICIPANT_ID") ?? args[1]);
                    string participantName = Environment.GetEnvironmentVariable("PARTICIPANT_NAME") ?? args[2];
                    string roleString = Environment.GetEnvironmentVariable("PARTICIPANT_ROLE") ?? args[3];

                    EmployeeRole role = roleString == "TeamLead" ? EmployeeRole.TeamLead : EmployeeRole.Junior;

                    var participant = new Participant(participantId, participantName, role);
                    var participantService = new ParticipantService(participant);
                    participantService.Start();

                    Console.WriteLine($"Участник {participantName} запущен и ожидает уведомления о хакатонах.");
                    break;

                default:
                    Console.WriteLine("Неверный тип сервиса. Укажите SERVICE_TYPE: HRDirector, HRManager или Participant.");
                    break;
            }
        }
    }
}
