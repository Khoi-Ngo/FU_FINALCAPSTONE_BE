using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Responses.Message;

namespace AISEA.ApiService.BAL.Services.Chat
{
    public class MessageService
    {
        private readonly MessageRepository _messageRepository;

        public MessageService(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        //construct Message
        public async Task<Message> CreateMessageAsync(string content, long senderId, long sessionId)
        {
            var newMessage = new Message
            {
                Content = content,
                SenderId = senderId,
                AdvisorySession1to1Id = sessionId
            };
            await _messageRepository.CreateAsync(newMessage);
            return newMessage;
        }

        //query messages
        public async Task<List<MessageItemResponse>> GetMessagesAsync(long chatSessionId)
        {
            return await _messageRepository.GetMessagesAsync(chatSessionId);
        }

    }
}