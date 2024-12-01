using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using Amazon.SimpleEmail;
using Amazon;
using Amazon.SimpleEmail.Model;

namespace HangfireSample
    {
        public class ExpiryNotification
        {
            private readonly string _connectionString;

            public ExpiryNotification(string connectionString)
            {
                _connectionString = connectionString;
            }
        public static void CheckAndSendNotificationsStatic(string connectionString)
        {
            var notification = new ExpiryNotification(connectionString);
            notification.CheckAndSendNotifications();  
        }

        public void CheckAndSendNotifications()
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        // Fetch records with an expiration date matching 15 days from today.
                        string query = @"
                        SELECT [ActivationCodes].[ID], 
                        [Info].[ContactEmail],
                        [Info].[ContactPerson]
                        FROM [ActivationCodes] 
                        INNER JOIN [Info] ON [ActivationCodes].[ID] = [Info].[ID]
                        WHERE CAST([ActivationCodes].[ExpiryDate] AS DATE) = @TargetDate
                        ";
                        

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                        
                        DateTime targetDate = DateTime.Today.AddDays(15).Date;
                            command.Parameters.AddWithValue("@TargetDate", targetDate);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                string unitId = reader.GetString(0); 
                                string email = reader.GetString(1); 
                                    string contactPerson = reader.GetString(2);
                                    
                                    using (var client = new AmazonSimpleEmailServiceClient("", "", RegionEndpoint.EUCentral1))
                                    {
                                        var emailRequest = new SendEmailRequest()
                                        {
                                            Source = "test@test.test",
                                            Destination = new Destination(),
                                            Message = new Message()
                                        };

                                        emailRequest.Destination.ToAddresses.Add(email);
                                        emailRequest.Message.Subject = new Amazon.SimpleEmail.Model.Content("Activation Period Expiration Notice");
                                        emailRequest.Message.Body = new Body(new Amazon.SimpleEmail.Model.Content("Dear " + contactPerson + ",\n\nYour activation period is about to expire."));
                                        var resp = client.SendEmail(emailRequest);
                                        
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

        }
    }