using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace order_service.src.Services
{
    /// <summary>
    /// Servicio encargado de enviar correos electrónicos mediante la API de SendGrid.
    /// Este servicio se utiliza para enviar notificaciones al cliente cuando:
    /// - Se crea una orden
    /// - Se confirma o rechaza por inventario
    /// - Se actualiza su estado (Enviado / Entregado)
    /// - Se cancela una orden
    /// </summary>
    public class SendGridService
    {
        /// <summary>
        /// API Key privada utilizada para autenticar solicitudes a SendGrid.
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Constructor del servicio.
        /// Obtiene la API Key desde las variables de entorno del sistema.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Lanza una excepción si la API Key no está configurada.
        /// </exception>
        public SendGridService()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") 
                      ?? throw new ArgumentNullException("SendGrid API Key is not configured.");
        }

        /// <summary>
        /// Envía un correo electrónico al cliente utilizando SendGrid.
        /// </summary>
        /// <param name="toEmail">Correo del destinatario.</param>
        /// <param name="subject">Asunto del correo.</param>
        /// <param name="htmlContent">Contenido en HTML del mensaje.</param>
        /// <returns>Tarea asincrónica que representa el envío del correo.</returns>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);

            // Correo del remitente (debe coincidir con el remitente verificado en SendGrid)
            var from = new EmailAddress("aromadecantsantofa@gmail.com", "Order Service");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);

            await client.SendEmailAsync(msg);
        }
    }
}