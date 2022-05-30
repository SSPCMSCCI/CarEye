using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class EnviaMail
    {
        public static void Enviar(String Destinatario, String Mensaje)
        {
            string correoEmisor = "kariquekeiter@gmail.com";
            string passwordEmisor = "ukamumrravixhect"; //no es mi pass real, solo sirve para enviar correos :v

            SmtpClient cliSmtp = new SmtpClient();
            cliSmtp.Port = 587;
            cliSmtp.EnableSsl = true;
            cliSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            cliSmtp.UseDefaultCredentials = false;
            cliSmtp.Credentials = new NetworkCredential(correoEmisor, passwordEmisor);
            cliSmtp.Host = "smtp.gmail.com";

            MailMessage correo = new MailMessage();
            correo.To.Add(new MailAddress(Destinatario));
            correo.Subject = "Cambio de password";
            correo.BodyEncoding = UTF8Encoding.UTF8;
            correo.IsBodyHtml = false;
            correo.Body = Mensaje;
            correo.From = new MailAddress(correoEmisor, "Car Eye");
            correo.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            try
            {
                Console.WriteLine("Enviando correo a:" + Destinatario);
                cliSmtp.Send(correo);
                Console.WriteLine("Correo enviado a:" + " " + Destinatario + " hora: " + DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error EnviarMail:" + " " + e.Message);
                throw;
            }

        }
    }
}