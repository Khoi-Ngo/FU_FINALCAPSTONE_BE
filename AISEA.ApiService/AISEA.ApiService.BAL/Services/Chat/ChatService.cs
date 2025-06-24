using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.Chat
{
    public class ChatService
    {
        private readonly MessageRepository _messageRepository;

        public ChatService(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _messageRepository.CreateAsync(message);
        }
    }
}